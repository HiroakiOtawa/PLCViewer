using System.Buffers.Binary;
using System.Globalization;
using System.Net.Sockets;

namespace PLCViewer
{
    /// <summary>
    ///  三菱電機Q/iQ-RシリーズおよびKEYENCE KVシリーズPLCとMCプロトコル(3Eフレーム、バイナリコード)でTCP/IP通信を行うクライアント。
    ///  D/W(ワードデバイス)およびM/B(ビットデバイス)のワード単位読み書きに対応する。
    /// </summary>
    public sealed class McProtocolClient : IDisposable
    {
        // --- 3Eフレーム 固定値 ---
        private const byte SubHeaderRequestLow = 0x50;
        private const byte SubHeaderRequestHigh = 0x00;

        private const byte NetworkNumber = 0x00;
        private const byte PcNumber = 0xFF;
        private const byte RequestModuleIoNoLow = 0xFF;
        private const byte RequestModuleIoNoHigh = 0x03;
        private const byte RequestModuleStationNo = 0x00;

        private const ushort CpuMonitoringTimer = 0x0010; // 250ms単位 (0x10 = 16 -> 4秒)

        private const ushort CommandBatchRead = 0x0401;
        private const ushort CommandBatchWrite = 0x1401;
        private const ushort SubCommandWordAccess = 0x0000; // 標準デバイス指定(先頭アドレス3バイト+デバイスコード1バイト)。Q/iQ-R/KV共通

        private const int FixedHeaderLength = 7; // subheader(2)+net(1)+pc(1)+io(2)+station(1)
        private const int TimerLength = 2;
        private const int RequestDataOffset = FixedHeaderLength + 2 + TimerLength; // = 11

        private const int ResponseHeaderLength = 9; // subheader(2)+net(1)+pc(1)+io(2)+station(1)+datalen(2)
        private const int EndCodeLength = 2;

        private TcpClient? _tcpClient;
        private NetworkStream? _stream;

        /// <summary>
        ///  通信のタイムアウト時間。既定値は3秒。
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        ///  接続対象のPLCシリーズ。既定値は<see cref="PlcSeries.MitsubishiQ"/>。
        ///  三菱Q/iQ-R/KEYENCE KVのいずれも同一のMCプロトコル3Eフレームで通信するため、
        ///  通信処理自体には影響しないが、画面表示や将来の拡張のために保持する。
        /// </summary>
        public PlcSeries Series { get; set; } = PlcSeries.MitsubishiQ;

        /// <summary>
        ///  現在PLCに接続中かどうかを取得する。
        /// </summary>
        public bool IsConnected => _tcpClient?.Connected ?? false;

        /// <summary>
        ///  指定したIPアドレス・ポート番号のPLCへ非同期に接続する。
        /// </summary>
        public async Task ConnectAsync(string ipAddress, int port, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);

            Disconnect();

            var client = new TcpClient();
            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(Timeout);

                await client.ConnectAsync(ipAddress, port, timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                client.Dispose();
                throw new PlcCommunicationException($"接続がタイムアウトしました。({ipAddress}:{port})");
            }
            catch (SocketException ex)
            {
                client.Dispose();
                throw new PlcCommunicationException($"PLCへの接続に失敗しました。({ipAddress}:{port})", ex);
            }

            _tcpClient = client;
            _stream = client.GetStream();
        }

        /// <summary>
        ///  PLCとの接続を切断する。
        /// </summary>
        public void Disconnect()
        {
            _stream?.Dispose();
            _stream = null;

            _tcpClient?.Dispose();
            _tcpClient = null;
        }

        /// <summary>
        ///  指定したデバイスの先頭番号から連続する<paramref name="count"/>ワード分を一括読み込みする。
        ///  ビットデバイス(M/B)の場合は1ワード=16点として読み込む(先頭は16点境界であること)。
        /// </summary>
        public async Task<ushort[]> ReadWordsAsync(PlcDeviceType deviceType, int headDevice, int count, CancellationToken cancellationToken = default)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "読み込みワード数は1以上を指定してください。");
            }

            ValidateHeadDevice(deviceType, headDevice);

            byte[] request = BuildReadRequest(GetDeviceCode(deviceType), headDevice, count);
            byte[] response = await SendAndReceiveAsync(request, cancellationToken).ConfigureAwait(false);

            return ParseReadResponse(response, count);
        }

        /// <summary>
        ///  指定したデバイスの先頭番号から連続する複数ワードを一括書き込みする。
        ///  ビットデバイス(M/B)の場合は1ワード=16点として書き込む(先頭は16点境界であること)。
        /// </summary>
        public async Task WriteWordsAsync(PlcDeviceType deviceType, int headDevice, ushort[] values, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values.Length == 0)
            {
                throw new ArgumentException("書き込みデータが指定されていません。", nameof(values));
            }

            ValidateHeadDevice(deviceType, headDevice);

            byte[] request = BuildWriteRequest(GetDeviceCode(deviceType), headDevice, values);
            byte[] response = await SendAndReceiveAsync(request, cancellationToken).ConfigureAwait(false);

            ValidateEndCode(response);
        }

        private async Task<byte[]> SendAndReceiveAsync(byte[] request, CancellationToken cancellationToken)
        {
            if (_stream is null || _tcpClient is null || !_tcpClient.Connected)
            {
                throw new PlcCommunicationException("PLCに接続されていません。先に接続を行ってください。");
            }

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(Timeout);

            try
            {
                await _stream.WriteAsync(request, timeoutCts.Token).ConfigureAwait(false);

                byte[] header = await ReadExactAsync(ResponseHeaderLength, timeoutCts.Token).ConfigureAwait(false);

                ushort dataLength = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(FixedHeaderLength, 2));
                if (dataLength < EndCodeLength)
                {
                    throw new PlcCommunicationException("PLCからの応答フレームが不正です(応答データ長が不正)。");
                }

                byte[] footer = await ReadExactAsync(dataLength, timeoutCts.Token).ConfigureAwait(false);

                byte[] full = new byte[header.Length + footer.Length];
                header.CopyTo(full, 0);
                footer.CopyTo(full, header.Length);
                return full;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new PlcCommunicationException("PLCとの通信がタイムアウトしました。");
            }
            catch (SocketException ex)
            {
                throw new PlcCommunicationException("PLCとの通信中に通信エラーが発生しました。", ex);
            }
            catch (IOException ex)
            {
                throw new PlcCommunicationException("PLCとの通信中に切断が検出されました。", ex);
            }
        }

        private async Task<byte[]> ReadExactAsync(int length, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[length];
            int offset = 0;
            while (offset < length)
            {
                int read = await _stream!.ReadAsync(buffer.AsMemory(offset, length - offset), cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    throw new PlcCommunicationException("PLCとの接続が切断されました。");
                }
                offset += read;
            }
            return buffer;
        }

        private byte[] BuildReadRequest(ushort deviceCode, int headDevice, int count)
        {
            // D/W/M/Bはアドレス範囲が3バイトに収まるため、Q/iQ-R/KV共通の標準デバイス指定(計10バイト)を使用する。
            // 拡張デバイス指定(先頭アドレス4バイト+デバイスコード2バイト)は3バイト範囲を超える特殊デバイス専用のため使用しない。
            const int requestDataLength = 10;

            byte[] frame = CreateFrameBuffer(requestDataLength);
            Span<byte> requestData = frame.AsSpan(RequestDataOffset, requestDataLength);

            // コマンドの書き込み
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(0, 2), CommandBatchRead);
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(2, 2), SubCommandWordAccess);  // 0x0000
            WriteDeviceNumber(requestData.Slice(4, 3), headDevice);                                    // 先頭アドレス(3バイト)
            requestData[7] = (byte)deviceCode;                                                         // デバイスコード(1バイト)
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(8, 2), (ushort)count);          // 点数(2バイト)

            return frame;
        }

        private byte[] BuildWriteRequest(ushort deviceCode, int headDevice, ushort[] values)
        {
            // D/W/M/Bはアドレス範囲が3バイトに収まるため、Q/iQ-R/KV共通の標準デバイス指定(先頭10バイト)を使用する。
            const int baseLength = 10;
            int requestDataLength = baseLength + (values.Length * 2);

            byte[] frame = CreateFrameBuffer(requestDataLength);
            Span<byte> requestData = frame.AsSpan(RequestDataOffset, requestDataLength);

            // コマンドの書き込み
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(0, 2), CommandBatchWrite);
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(2, 2), SubCommandWordAccess);
            WriteDeviceNumber(requestData.Slice(4, 3), headDevice);
            requestData[7] = (byte)deviceCode;
            BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(8, 2), (ushort)values.Length);

            // 値データの書き込み
            for (int i = 0; i < values.Length; i++)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(requestData.Slice(baseLength + (i * 2), 2), values[i]);
            }

            return frame;
        }

        private static byte[] CreateFrameBuffer(int requestDataLength)
        {
            int totalLength = RequestDataOffset + requestDataLength;
            byte[] frame = new byte[totalLength];
            Span<byte> span = frame;

            span[0] = SubHeaderRequestLow;
            span[1] = SubHeaderRequestHigh;
            span[2] = NetworkNumber;
            span[3] = PcNumber;
            span[4] = RequestModuleIoNoLow;
            span[5] = RequestModuleIoNoHigh;
            span[6] = RequestModuleStationNo;

            ushort dataLengthField = (ushort)(TimerLength + requestDataLength);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(FixedHeaderLength, 2), dataLengthField);
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(FixedHeaderLength + 2, 2), CpuMonitoringTimer);

            return frame;
        }

        private static void WriteDeviceNumber(Span<byte> destination, int deviceNumber)
        {
            destination[0] = (byte)(deviceNumber & 0xFF);
            destination[1] = (byte)((deviceNumber >> 8) & 0xFF);
            destination[2] = (byte)((deviceNumber >> 16) & 0xFF);
        }

        private static ushort[] ParseReadResponse(byte[] response, int expectedCount)
        {
            ValidateEndCode(response);

            var result = new ushort[expectedCount];
            int dataOffset = ResponseHeaderLength + EndCodeLength;
            for (int i = 0; i < expectedCount; i++)
            {
                result[i] = BinaryPrimitives.ReadUInt16LittleEndian(response.AsSpan(dataOffset + (i * 2), 2));
            }
            return result;
        }

        private static void ValidateEndCode(byte[] response)
        {
            if (response.Length < ResponseHeaderLength + EndCodeLength)
            {
                throw new PlcCommunicationException("PLCからの応答フレームが短すぎます。");
            }

            ushort endCode = BinaryPrimitives.ReadUInt16LittleEndian(response.AsSpan(ResponseHeaderLength, 2));
            if (endCode != 0x0000)
            {
                string? detail = GetEndCodeDescription(endCode);
                string message = detail is null
                    ? $"PLCからエラー応答を受信しました。(終了コード: 0x{endCode:X4})"
                    : $"PLCからエラー応答を受信しました。(終了コード: 0x{endCode:X4} {detail})";
                throw new PlcCommunicationException(message);
            }
        }

        /// <summary>
        ///  MCプロトコルの代表的な異常終了コードに対する原因・対処の説明を取得する。該当がない場合はnull。
        /// </summary>
        private static string? GetEndCodeDescription(ushort endCode) => endCode switch
        {
            0x0055 => "CPUがRUN中のためオンライン書き込みが拒否されました。GX Works等でCPUパラメータの「RUN中書き込みを許可する」を有効にするか、CPUをSTOPにしてから書き込んでください。",
            0xC050 => "モニタ登録が行われていません。",
            0xC051 or 0xC052 or 0xC053 or 0xC054 => "要求データの点数や範囲がデバイスの指定範囲外です。アドレスや読み書き点数を確認してください。",
            0xC056 => "アクセス先のデバイス範囲を超えています。先頭アドレスや点数を確認してください。",
            0xC058 => "要求データ数とデータ長が一致していません。",
            0xC059 => "コマンド・サブコマンドの指定が不正です。PLCシリーズ設定を確認してください。",
            0xC05B => "PLCがそのデバイスへのアクセス権を持っていません。",
            0xC05C => "要求内容が不正です。",
            0xC05F => "PLCが要求を実行できない状態です(他ユニットとの通信中等)。",
            0xC060 => "デバイスへのアクセスができません。デバイス種別やアドレス指定を確認してください。",
            0xC061 => "要求データ長が不正です。",
            _ => null,
        };

        private static ushort GetDeviceCode(PlcDeviceType deviceType) => deviceType switch
        {
            PlcDeviceType.D => 0x00A8, // データレジスタ
            PlcDeviceType.W => 0x00B4, // リンクレジスタ
            PlcDeviceType.M => 0x0090, // 内部リレー
            PlcDeviceType.B => 0x00A0, // リンクリレー
            _ => throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, "未対応のデバイス種別です。"),
        };

        /// <summary>
        ///  指定したデバイス種別が16進アドレス表記かどうかを取得する。
        /// </summary>
        public static bool IsHexAddress(PlcDeviceType deviceType) => deviceType is PlcDeviceType.W or PlcDeviceType.B;

        /// <summary>
        ///  指定したデバイス種別がビットデバイスかどうかを取得する。
        /// </summary>
        public static bool IsBitDevice(PlcDeviceType deviceType) => deviceType is PlcDeviceType.M or PlcDeviceType.B;

        /// <summary>
        ///  ビットデバイス(M/B)の場合、先頭アドレスを16点境界に切り下げる。ワードデバイスの場合はそのまま返す。
        /// </summary>
        public static int AlignHeadDeviceToWordBoundary(PlcDeviceType deviceType, int headDevice)
            => IsBitDevice(deviceType) ? headDevice - (headDevice % 16) : headDevice;

        private static void ValidateHeadDevice(PlcDeviceType deviceType, int headDevice)
        {
            if (headDevice < 0)
            {
                throw new FormatException("デバイス番号は0以上を指定してください。");
            }

            if (IsBitDevice(deviceType) && (headDevice % 16) != 0)
            {
                string example = IsHexAddress(deviceType) ? "B0, B10, B20..." : "M0, M16, M32...";
                throw new FormatException(
                    $"ビットデバイスをワード単位で扱うため、先頭アドレスは16点の倍数にしてください。(例: {example})");
            }
        }

        /// <summary>
        ///  "D100" "W1A0" のようなデバイス表記文字列を、指定した種別のデバイス番号として解析する。
        ///  W/Bは16進、D/Mは10進として解釈する。種別プレフィックスの有無は許容する。
        /// </summary>
        public static int ParseDeviceNumber(PlcDeviceType deviceType, string deviceAddress)
        {
            if (string.IsNullOrWhiteSpace(deviceAddress))
            {
                throw new FormatException("アドレスが入力されていません。");
            }

            string trimmed = deviceAddress.Trim();
            string prefix = deviceType.ToString();
            string numberPart = trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? trimmed[prefix.Length..]
                : trimmed;

            bool isHex = IsHexAddress(deviceType);
            bool parsed = isHex
                ? int.TryParse(numberPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int deviceNumber)
                : int.TryParse(numberPart, NumberStyles.None, CultureInfo.InvariantCulture, out deviceNumber);

            if (!parsed || deviceNumber < 0)
            {
                string example = isHex ? $"{prefix}1A0" : $"{prefix}100";
                throw new FormatException($"アドレスの形式が不正です: '{deviceAddress}' (例: {example})");
            }

            return deviceNumber;
        }

        /// <summary>
        ///  "D100" "W1A0" "M16" "B10" のようなプレフィックス付きデバイス表記文字列から、
        ///  デバイス種別と番号を自動判定して解析する。
        /// </summary>
        public static (PlcDeviceType DeviceType, int DeviceNumber) ParseDeviceAddress(string deviceAddress)
        {
            if (string.IsNullOrWhiteSpace(deviceAddress))
            {
                throw new FormatException("アドレスが入力されていません。");
            }

            string trimmed = deviceAddress.Trim();
            PlcDeviceType deviceType = char.ToUpperInvariant(trimmed[0]) switch
            {
                'D' => PlcDeviceType.D,
                'W' => PlcDeviceType.W,
                'M' => PlcDeviceType.M,
                'B' => PlcDeviceType.B,
                _ => throw new FormatException($"アドレスの形式が不正です: '{deviceAddress}' (例: D100, W1A0, M16, B10)"),
            };

            return (deviceType, ParseDeviceNumber(deviceType, trimmed));
        }

        /// <summary>
        ///  デバイス種別と番号を "D100" "W1A0" 形式の表示用文字列に変換する。W/Bは16進表記とする。
        /// </summary>
        public static string FormatDeviceAddress(PlcDeviceType deviceType, int deviceNumber)
            => IsHexAddress(deviceType)
                ? $"{deviceType}{deviceNumber:X}"
                : $"{deviceType}{deviceNumber}";

        /// <summary>
        ///  デバイス種別プレフィックスを付けず、番号のみを表示用文字列に変換する。W/Bは16進表記とする。
        ///  デバイス種別をコンボボックス等で別途選択している入力欄の表示更新に使用する。
        /// </summary>
        public static string FormatDeviceNumber(PlcDeviceType deviceType, int deviceNumber)
            => IsHexAddress(deviceType)
                ? deviceNumber.ToString("X", CultureInfo.InvariantCulture)
                : deviceNumber.ToString(CultureInfo.InvariantCulture);

        public void Dispose()
        {
            Disconnect();
        }
    }
}
