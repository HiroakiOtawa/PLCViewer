using System.Text;

namespace PLCViewer
{
    public partial class Form1 : Form
    {
        private const int AnalogWordCount = 10;
        private const int DigitalMinColumnCount = 17; // アドレス + Bit15～Bit0
        private const int AnalogMinColumnCount = 11;  // 先頭アドレス + ワード10個
        private const int MaxReadWordsPerRequest = 960; // MCプロトコル一括読出の最大ワード数

        private readonly McProtocolClient _plcClient = new();
        private TextBox? _analogCellEditor;
        private static readonly Color ChangedCellBackColor = Color.Yellow;
        private bool _digContinuousEnabled;
        private bool _anaContinuousEnabled;
        private bool _digContinuousReadBusy;
        private bool _anaContinuousReadBusy;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            InitializePlcSeriesComboBox();
            InitializeDeviceComboBoxes();
            InitializeDigitalListViewColumns();
            InitializeAnalogListViewColumns();
            ApplyDigitalBitColumnDisplayOrder(chkDigBitOrderReversed.Checked);
            UpdateFormTitle();
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _plcClient.Dispose();
        }

        // デジタルはビットデバイス(M/B)、アナログはワードデバイス(D/W)を選択対象とする。
        private void InitializeDeviceComboBoxes()
        {
            cboDigDevice.Items.Add(PlcDeviceType.M);
            cboDigDevice.Items.Add(PlcDeviceType.B);
            cboDigDevice.SelectedIndex = 0;

            cboAnaDevice.Items.Add(PlcDeviceType.D);
            cboAnaDevice.Items.Add(PlcDeviceType.W);
            cboAnaDevice.SelectedIndex = 0;
        }

        // 接続対象のPLCシリーズ(三菱Q/三菱iQ-R/KEYENCE KV)を選択できるようにする。
        private void InitializePlcSeriesComboBox()
        {
            cboPlcSeries.Items.Add(new PlcSeriesComboItem(PlcSeries.MitsubishiQ));
            cboPlcSeries.Items.Add(new PlcSeriesComboItem(PlcSeries.MitsubishiIqR));
            cboPlcSeries.Items.Add(new PlcSeriesComboItem(PlcSeries.KeyenceKv));
            cboPlcSeries.SelectedIndex = 0;
        }

        private PlcSeries SelectedPlcSeries => ((PlcSeriesComboItem)cboPlcSeries.SelectedItem!).Series;

        // ComboBoxの表示名とPlcSeriesを紐付けるためのラッパー。
        private sealed record PlcSeriesComboItem(PlcSeries Series)
        {
            public override string ToString() => Series.GetDisplayName();
        }

        private void CboPlcSeries_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateFormTitle();
        }

        // フォームタイトルに選択中のPLCシリーズ名を反映する。
        private void UpdateFormTitle()
        {
            Text = $"PLC デバイスビューア ({SelectedPlcSeries.GetDisplayName()})";
        }

        private PlcDeviceType SelectedDigitalDevice => (PlcDeviceType)cboDigDevice.SelectedItem!;

        private PlcDeviceType SelectedAnalogDevice => (PlcDeviceType)cboAnaDevice.SelectedItem!;

        // 「アドレス」「Bit15」…「Bit0」「ワード数値」の計18列。
        private void InitializeDigitalListViewColumns()
        {
            lvwDigital.Columns.Add("アドレス", 70);
            for (int bit = 15; bit >= 0; bit--)
            {
                lvwDigital.Columns.Add($"{bit}", 45);
            }
            lvwDigital.Columns.Add("ワード数値", 80);
        }

        private void ChkDigBitOrderReversed_CheckedChanged(object? sender, EventArgs e)
        {
            ApplyDigitalBitColumnDisplayOrder(chkDigBitOrderReversed.Checked);
        }

        // Bit列の表示順を切り替える。SubItemsの列インデックス自体は変更せず、
        // 各列のDisplayIndexのみを変更することで、値の再構成や編集ロジックに影響を与えずに見た目だけ左右反転する。
        // reversed=falseの場合は Bit15(左)～Bit0(右)、trueの場合は Bit0(左)～Bit15(右) の順で表示する。
        private void ApplyDigitalBitColumnDisplayOrder(bool reversed)
        {
            for (int columnIndex = 1; columnIndex <= 16; columnIndex++)
            {
                lvwDigital.Columns[columnIndex].DisplayIndex = reversed ? 17 - columnIndex : columnIndex;
            }

            // OwnerDraw中はDisplayIndexの変更だけでは即座に再描画されないため、明示的に再描画して表示順を反映する。
            lvwDigital.Invalidate();
        }

        // 「先頭アドレス」「+0」…「+9」の計11列。
        private void InitializeAnalogListViewColumns()
        {
            lvwAnalog.Columns.Add("先頭アドレス", 90);
            for (int i = 0; i < AnalogWordCount; i++)
            {
                lvwAnalog.Columns.Add($"+{i}", 65);
            }
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            try
            {
                await ConnectAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
        }

        private void BtnDisconnect_Click(object? sender, EventArgs e)
        {
            Disconnect();
        }

        // PLCへ接続する。IPアドレス・ポート番号の検証、接続処理、および接続成功時の画面状態更新を行う。
        // 接続に失敗した場合は例外をスローするため、呼び出し元でエラー表示・切断処理を行うこと。
        private async Task ConnectAsync()
        {
            btnConnect.Enabled = false;
            try
            {
                string ip = txtIpAddress.Text.Trim();
                if (string.IsNullOrWhiteSpace(ip))
                {
                    throw new FormatException("IPアドレスを入力してください。");
                }

                if (!int.TryParse(txtPort.Text.Trim(), out int port) || port is <= 0 or > 65535)
                {
                    throw new FormatException("ポート番号は1～65535の範囲で入力してください。");
                }

                PlcSeries series = SelectedPlcSeries;
                _plcClient.Series = series;

                lblStatus.Text = "接続中...";
                await _plcClient.ConnectAsync(ip, port);

                lblStatus.Text = $"接続済み ({series.GetDisplayName()} {ip}:{port})";
                btnDisconnect.Enabled = true;
                cboPlcSeries.Enabled = false;
                txtIpAddress.Enabled = false;
                txtPort.Enabled = false;
            }
            finally
            {
                if (!_plcClient.IsConnected)
                {
                    lblStatus.Text = "未接続";
                    btnConnect.Enabled = true;
                }
            }
        }


        //切断処理 
        private void Disconnect()
        {
            _plcClient.Disconnect();
            lblStatus.Text = "未接続";
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            cboPlcSeries.Enabled = true;
            txtIpAddress.Enabled = true;
            txtPort.Enabled = true;
            SetDigContinuousMode(false);
            SetAnaContinuousMode(false);
        }


        private async void BtnDigRead_Click(object? sender, EventArgs e)
        {
            DigRead();
        }

        // 「連続読み込み」ラジオボタンのトグル動作(単独配置のため、クリックのたびにON/OFFを切り替える)。
        private void RdoDigContinuous_Click(object? sender, EventArgs e)
        {
            SetDigContinuousMode(!_digContinuousEnabled);
        }

        // 連続読み込みのON/OFFを切り替える。ON時は読込・書込ボタンをdisableにし、1秒間隔のタイマーを開始する。
        private void SetDigContinuousMode(bool enabled)
        {
            _digContinuousEnabled = enabled;
            rdoDigContinuous.Checked = enabled;
            btnDigRead.Enabled = !enabled;
            btnDigWrite.Enabled = !enabled;
            timerDigContinuous.Enabled = enabled;
        }

        // 連続読み込みタイマーのTickごとにデジタル読み込みを実行する。前回の読み込みが完了していない場合はスキップする。
        private async void TimerDigContinuous_Tick(object? sender, EventArgs e)
        {
            if (_digContinuousReadBusy)
            {
                return;
            }

            _digContinuousReadBusy = true;
            try
            {
                EnsureConnected();
                await ReadDigitalAsync();
            }
            catch (Exception ex)
            {
                SetDigContinuousMode(false);
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                _digContinuousReadBusy = false;
            }
        }

        //　デジタル読み取り処理
        private async void DigRead()
        {
            if (_digContinuousEnabled)
            {
                return;
            }

            btnDigRead.Enabled = false;
            try
            {
                if (!_plcClient.IsConnected)
                {
                    await ConnectAsync();
                }

                EnsureConnected();
                await ReadDigitalAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                btnDigRead.Enabled = true;
            }
        }


        // 現在の読込設定でPLCから読み込み、ListViewをクリアして再表示する(baselineも読込値に更新する)。
        private async Task ReadDigitalAsync()
        {
            PlcDeviceType deviceType = SelectedDigitalDevice;
            int deviceNumber = McProtocolClient.ParseDeviceNumber(deviceType, txtDigReadAddress.Text);

            // ビットデバイスはワード単位で扱うため、先頭アドレスが16点境界でない場合は自動的に切り下げる。
            int alignedDeviceNumber = McProtocolClient.AlignHeadDeviceToWordBoundary(deviceType, deviceNumber);
            if (alignedDeviceNumber != deviceNumber)
            {
                deviceNumber = alignedDeviceNumber;
                txtDigReadAddress.Text = McProtocolClient.FormatDeviceNumber(deviceType, deviceNumber);
            }

            // 画面に表示できる行数分(1行=1ワード)を一度の通信で読み込む。
            int rowCount = Math.Min(CalculateVisibleRowCount(lvwDigital), MaxReadWordsPerRequest);
            int addressStep = McProtocolClient.IsBitDevice(deviceType) ? 16 : 1;

            ushort[] values = await _plcClient.ReadWordsAsync(deviceType, deviceNumber, rowCount);

            lvwDigital.BeginUpdate();
            try
            {
                lvwDigital.Items.Clear();
                for (int i = 0; i < values.Length; i++)
                {
                    UpdateDigitalListView(deviceType, deviceNumber + (i * addressStep), values[i], resetBaseline: true);
                }
            }
            finally
            {
                lvwDigital.EndUpdate();
            }
        }

        private async void BtnAnaRead_Click(object? sender, EventArgs e)
        {
            AnaRead();
        }

        // 「連続読み込み」ラジオボタンのトグル動作(単独配置のため、クリックのたびにON/OFFを切り替える)。
        private void RdoAnaContinuous_Click(object? sender, EventArgs e)
        {
            SetAnaContinuousMode(!_anaContinuousEnabled);
        }

        // 連続読み込みのON/OFFを切り替える。ON時は読込・書込ボタンをdisableにし、1秒間隔のタイマーを開始する。
        private void SetAnaContinuousMode(bool enabled)
        {
            _anaContinuousEnabled = enabled;
            rdoAnaContinuous.Checked = enabled;
            btnAnaRead.Enabled = !enabled;
            btnAnaWrite.Enabled = !enabled;
            timerAnaContinuous.Enabled = enabled;
        }

        // 連続読み込みタイマーのTickごとにアナログ読み込みを実行する。前回の読み込みが完了していない場合はスキップする。
        private async void TimerAnaContinuous_Tick(object? sender, EventArgs e)
        {
            if (_anaContinuousReadBusy)
            {
                return;
            }

            _anaContinuousReadBusy = true;
            try
            {
                EnsureConnected();
                await ReadAnalogAsync();
            }
            catch (Exception ex)
            {
                SetAnaContinuousMode(false);
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                _anaContinuousReadBusy = false;
            }
        }

        // アナログ読み取り処理
        private async void AnaRead()
        {
            if (_anaContinuousEnabled)
            {
                return;
            }

            btnAnaRead.Enabled = false;
            try
            {
                if (!_plcClient.IsConnected)
                {
                    await ConnectAsync();
                }

                EnsureConnected();
                await ReadAnalogAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                btnAnaRead.Enabled = true;

            }
        }


        // 現在の読込設定でPLCから読み込み、ListViewをクリアして再表示する(baselineも読込値に更新する)。
        private async Task ReadAnalogAsync()
        {
            PlcDeviceType deviceType = SelectedAnalogDevice;
            int deviceNumber = McProtocolClient.ParseDeviceNumber(deviceType, txtAnaReadAddress.Text);

            // 画面に表示できる行数分(1行=10ワード)を一度の通信で読み込む。
            int rowCount = Math.Min(CalculateVisibleRowCount(lvwAnalog), MaxReadWordsPerRequest / AnalogWordCount);

            ushort[] values = await _plcClient.ReadWordsAsync(deviceType, deviceNumber, rowCount * AnalogWordCount);

            HideAnalogCellEditor();
            lvwAnalog.BeginUpdate();
            try
            {
                lvwAnalog.Items.Clear();
                for (int i = 0; i < rowCount; i++)
                {
                    ushort[] rowValues = values.AsSpan(i * AnalogWordCount, AnalogWordCount).ToArray();
                    UpdateAnalogListView(deviceType, deviceNumber + (i * AnalogWordCount), rowValues, resetBaseline: true);
                }
            }
            finally
            {
                lvwAnalog.EndUpdate();
            }
        }

        private async void BtnDigWrite_Click(object? sender, EventArgs e)
        {
            btnDigWrite.Enabled = false;
            try
            {
                EnsureConnected();

                if (lvwDigital.Items.Count == 0)
                {
                    MessageBox.Show(this, "書き込む行がありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 全行を先に検証してから書き込む(部分書込防止)。ワード値はBit15～Bit0から再構成する。
                // baseline(前回読込値)と一致する行は書き込み対象から除外し、変更があった行のみを書き込む。
                var writeTargets = new List<(PlcDeviceType Type, int Number, ushort Value)>();
                foreach (ListViewItem item in lvwDigital.Items)
                {
                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(item.Text);
                    ushort value = ReconstructWordFromBitSubItems(item);

                    if (item.Tag is ushort baseline && baseline == value)
                    {
                        continue;
                    }

                    writeTargets.Add((type, number, value));
                }

                if (writeTargets.Count == 0)
                {
                    MessageBox.Show(this, "変更された行がありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ConfirmWrite("デジタル表示リスト", writeTargets.Count))
                {
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort value) in writeTargets)
                {
                    await _plcClient.WriteWordsAsync(type, number, [value]);
                }

                // 書き込み後は実機の最新値を反映するため再読み込みする。
                await ReadDigitalAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                btnDigWrite.Enabled = true;
            }
        }

        private async void BtnAnaWrite_Click(object? sender, EventArgs e)
        {
            btnAnaWrite.Enabled = false;
            try
            {
                EnsureConnected();

                if (lvwAnalog.Items.Count == 0)
                {
                    MessageBox.Show(this, "書き込む行がありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 全行を先に検証してから書き込む(部分書込防止)。baseline(前回読込値)と異なるワードのみを書き込み対象とする。
                var writeTargets = new List<(PlcDeviceType Type, int Number, ushort[] Values)>();
                foreach (ListViewItem item in lvwAnalog.Items)
                {
                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(item.Text);
                    ushort[] values = ParseAnalogRowValues(item);

                    foreach ((int changedNumber, ushort[] changedValues) in GetChangedAnalogRanges(item, number, values))
                    {
                        writeTargets.Add((type, changedNumber, changedValues));
                    }
                }

                if (writeTargets.Count == 0)
                {
                    MessageBox.Show(this, "変更されたセルがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ConfirmWrite("アナログ表示リスト", writeTargets.Count))
                {
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort[] values) in writeTargets)
                {
                    await _plcClient.WriteWordsAsync(type, number, values);
                }

                // 書き込み後は実機の最新値を反映するため再読み込みする。
                await ReadAnalogAsync();

                MessageBox.Show(this, $"書き込みが完了しました。({writeTargets.Count}件)", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
            finally
            {
                btnAnaWrite.Enabled = true;
            }
        }

        // Bit列(SubItems[1]～[16])をダブルクリックしたら0/1を反転し、ワード数値を再計算する。
        private void LvwDigital_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = lvwDigital.HitTest(e.Location);
            if (hit.Item is null || hit.SubItem is null)
            {
                return;
            }

            int columnIndex = hit.Item.SubItems.IndexOf(hit.SubItem);
            if (columnIndex is < 1 or > 16)
            {
                return;
            }

            try
            {
                ListViewItem item = hit.Item;
                ListViewItem.ListViewSubItem bitSubItem = item.SubItems[columnIndex];
                bitSubItem.Text = bitSubItem.Text.Trim() switch
                {
                    "0" => "1",
                    "1" => "0",
                    _ => throw new FormatException($"行 '{item.Text}' のBit値が不正です: '{bitSubItem.Text}' (0または1)"),
                };

                ushort newValue = ReconstructWordFromBitSubItems(item);
                item.SubItems[17].Text = newValue.ToString();
                ApplyDigitalRowHighlight(item);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // +0～+9列(SubItems[1]～[10])をダブルクリックしたらインラインTextBoxを表示し、値の編集を可能にする。
        private void LvwAnalog_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = lvwAnalog.HitTest(e.Location);
            if (hit.Item is null || hit.SubItem is null)
            {
                return;
            }

            int columnIndex = hit.Item.SubItems.IndexOf(hit.SubItem);
            if (columnIndex is < 1 or > 10)
            {
                return;
            }

            ShowAnalogCellEditor(hit.Item, columnIndex);
        }

        // 指定したセルの位置にTextBoxを重ねて表示し、直接編集できるようにする。
        private void ShowAnalogCellEditor(ListViewItem item, int columnIndex)
        {
            HideAnalogCellEditor();

            Rectangle cellBounds = item.SubItems[columnIndex].Bounds;

            var editor = new TextBox
            {
                Text = item.SubItems[columnIndex].Text,
                Bounds = cellBounds,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Right,
                Tag = (Item: item, ColumnIndex: columnIndex)
            };
            editor.KeyDown += AnalogCellEditor_KeyDown;
            editor.LostFocus += AnalogCellEditor_LostFocus;

            lvwAnalog.Controls.Add(editor);
            _analogCellEditor = editor;

            editor.Focus();
            editor.SelectAll();
        }

        private void AnalogCellEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (sender is not TextBox editor)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.SuppressKeyPress = true;
                    CommitAnalogCellEditor(editor);
                    break;
                case Keys.Escape:
                    e.SuppressKeyPress = true;
                    HideAnalogCellEditor();
                    break;
            }
        }

        private void AnalogCellEditor_LostFocus(object? sender, EventArgs e)
        {
            if (sender is TextBox editor)
            {
                CommitAnalogCellEditor(editor);
            }
        }

        // 編集中のTextBoxの内容を検証し、問題なければ対象セルへ反映してエディタを閉じる。
        private void CommitAnalogCellEditor(TextBox editor)
        {
            if (!ReferenceEquals(_analogCellEditor, editor))
            {
                return;
            }

            (ListViewItem item, int columnIndex) = ((ListViewItem, int))editor.Tag!;
            string text = editor.Text.Trim();

            if (!ushort.TryParse(text, out ushort value))
            {
                MessageBox.Show(this, $"値が不正です: '{text}' (0～65535の整数を入力してください)", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                editor.Focus();
                editor.SelectAll();
                return;
            }

            item.SubItems[columnIndex].Text = value.ToString();
            ApplyAnalogRowHighlight(item);
            HideAnalogCellEditor();
        }

        private void HideAnalogCellEditor()
        {
            if (_analogCellEditor is null)
            {
                return;
            }

            TextBox editor = _analogCellEditor;
            _analogCellEditor = null;

            editor.KeyDown -= AnalogCellEditor_KeyDown;
            editor.LostFocus -= AnalogCellEditor_LostFocus;
            lvwAnalog.Controls.Remove(editor);
            editor.Dispose();
        }

        private void BtnDigCopy_Click(object? sender, EventArgs e)
        {
            CopyListViewToClipboard(lvwDigital);
        }

        private void BtnAnaCopy_Click(object? sender, EventArgs e)
        {
            CopyListViewToClipboard(lvwAnalog);
        }

        // ヘッダー行付きタブ区切り形式でコピーする(Excelへの貼り付け用)。
        private void CopyListViewToClipboard(ListView listView)
        {
            try
            {
                if (listView.Items.Count == 0)
                {
                    MessageBox.Show(this, "コピーする行がありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var builder = new StringBuilder();
                builder.AppendLine(string.Join('\t', listView.Columns.Cast<ColumnHeader>().Select(c => c.Text)));

                foreach (ListViewItem item in listView.Items)
                {
                    builder.AppendLine(string.Join('\t', item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(s => s.Text)));
                }

                Clipboard.SetText(builder.ToString());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // クリップボードのタブ区切りテキストを解析し、全行の検証に成功した場合のみリストへ反映する。
        private void BtnDigPaste_Click(object? sender, EventArgs e)
        {
            try
            {
                var parsed = new List<(PlcDeviceType Type, int Number, ushort Value)>();
                foreach (string[] cells in GetClipboardRows())
                {
                    if (IsHeaderRow(cells))
                    {
                        continue;
                    }

                    if (cells.Length < DigitalMinColumnCount)
                    {
                        throw new FormatException(
                            $"貼り付けデータの列数が不足しています(アドレス+15～0の{DigitalMinColumnCount}列必要): '{cells[0]}'");
                    }

                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(cells[0]);
                    ushort value = ReconstructWordFromBitTexts(cells[0], i => cells[16 - i]);
                    parsed.Add((type, number, value));
                }

                if (parsed.Count == 0)
                {
                    MessageBox.Show(this, "貼り付け可能なデータがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort value) in parsed)
                {
                    UpdateDigitalListView(type, number, value);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void BtnAnaPaste_Click(object? sender, EventArgs e)
        {
            try
            {
                var parsed = new List<(PlcDeviceType Type, int Number, ushort[] Values)>();
                foreach (string[] cells in GetClipboardRows())
                {
                    if (IsHeaderRow(cells))
                    {
                        continue;
                    }

                    if (cells.Length < AnalogMinColumnCount)
                    {
                        throw new FormatException(
                            $"貼り付けデータの列数が不足しています(先頭アドレス+10ワードの{AnalogMinColumnCount}列必要): '{cells[0]}'");
                    }

                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(cells[0]);

                    var values = new ushort[AnalogWordCount];
                    for (int i = 0; i < AnalogWordCount; i++)
                    {
                        string text = cells[i + 1].Trim();
                        if (!ushort.TryParse(text, out values[i]))
                        {
                            throw new FormatException($"行 '{cells[0]}' の+{i}の値が不正です: '{text}' (0～65535の整数)");
                        }
                    }
                    parsed.Add((type, number, values));
                }

                if (parsed.Count == 0)
                {
                    MessageBox.Show(this, "貼り付け可能なデータがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort[] values) in parsed)
                {
                    UpdateAnalogListView(type, number, values);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // デジタル表示リスト用「ファイル保存」: 範囲指定ダイアログでPLCから読み込み、TXTファイルへ書き出す。
        private async void BtnDigFileSave_Click(object? sender, EventArgs e)
        {
            try
            {
                EnsureConnected();

                PlcDeviceType deviceType = SelectedDigitalDevice;
                using var rangeDialog = new DeviceRangeDialog(
                    "デジタル ファイル保存範囲指定", deviceType, txtDigReadAddress.Text, "ワード数:", CalculateVisibleRowCount(lvwDigital), MaxReadWordsPerRequest);
                if (rangeDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                int deviceNumber = McProtocolClient.AlignHeadDeviceToWordBoundary(deviceType, rangeDialog.DeviceNumber);
                int addressStep = McProtocolClient.IsBitDevice(deviceType) ? 16 : 1;

                ushort[] values = await _plcClient.ReadWordsAsync(deviceType, deviceNumber, rangeDialog.Count);

                using SaveFileDialog saveDialog = CreateTxtSaveFileDialog("デジタルデバイス.txt");
                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var builder = new StringBuilder();
                builder.AppendLine(string.Join('\t', lvwDigital.Columns.Cast<ColumnHeader>().Select(c => c.Text)));
                for (int i = 0; i < values.Length; i++)
                {
                    int currentDeviceNumber = deviceNumber + (i * addressStep);
                    string addressText = McProtocolClient.FormatDeviceAddress(deviceType, currentDeviceNumber);
                    IEnumerable<string> bitTexts = Enumerable.Range(0, 16).Select(bit => ((values[i] >> (15 - bit)) & 1).ToString());
                    builder.AppendLine(string.Join('\t', new[] { addressText }.Concat(bitTexts).Append(values[i].ToString())));
                }

                await File.WriteAllTextAsync(saveDialog.FileName, builder.ToString(), Encoding.UTF8);
                MessageBox.Show(this, "ファイルへの保存が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
        }

        // アナログ表示リスト用「ファイル保存」: 範囲指定ダイアログでPLCから読み込み、TXTファイルへ書き出す。
        private async void BtnAnaFileSave_Click(object? sender, EventArgs e)
        {
            try
            {
                EnsureConnected();

                PlcDeviceType deviceType = SelectedAnalogDevice;
                int currentRowCount = Math.Min(CalculateVisibleRowCount(lvwAnalog), MaxReadWordsPerRequest / AnalogWordCount);
                using var rangeDialog = new DeviceRangeDialog(
                    "アナログ ファイル保存範囲指定", deviceType, txtAnaReadAddress.Text, "行数(1行=10ワード):", currentRowCount, MaxReadWordsPerRequest / AnalogWordCount);
                if (rangeDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                int deviceNumber = rangeDialog.DeviceNumber;
                int rowCount = rangeDialog.Count;

                ushort[] values = await _plcClient.ReadWordsAsync(deviceType, deviceNumber, rowCount * AnalogWordCount);

                using SaveFileDialog saveDialog = CreateTxtSaveFileDialog("アナログデバイス.txt");
                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var builder = new StringBuilder();
                builder.AppendLine(string.Join('\t', lvwAnalog.Columns.Cast<ColumnHeader>().Select(c => c.Text)));
                for (int i = 0; i < rowCount; i++)
                {
                    string addressText = McProtocolClient.FormatDeviceAddress(deviceType, deviceNumber + (i * AnalogWordCount));
                    IEnumerable<string> wordTexts = values.AsSpan(i * AnalogWordCount, AnalogWordCount).ToArray().Select(v => v.ToString());
                    builder.AppendLine(string.Join('\t', new[] { addressText }.Concat(wordTexts)));
                }

                await File.WriteAllTextAsync(saveDialog.FileName, builder.ToString(), Encoding.UTF8);
                MessageBox.Show(this, "ファイルへの保存が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
        }

        // デジタル表示リスト用「ファイル読込」: TXTファイルを解析し、確認の上でPLCへ書き込む。
        private async void BtnDigFileLoad_Click(object? sender, EventArgs e)
        {
            try
            {
                EnsureConnected();

                using OpenFileDialog openDialog = CreateTxtOpenFileDialog();
                if (openDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string text = await File.ReadAllTextAsync(openDialog.FileName, Encoding.UTF8);

                var writeTargets = new List<(PlcDeviceType Type, int Number, ushort Value)>();
                foreach (string[] cells in ParseTabSeparatedRows(text))
                {
                    if (IsHeaderRow(cells))
                    {
                        continue;
                    }

                    if (cells.Length < DigitalMinColumnCount)
                    {
                        throw new FormatException(
                            $"ファイルの列数が不足しています(アドレス+15～0の{DigitalMinColumnCount}列必要): '{cells[0]}'");
                    }

                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(cells[0]);
                    ushort value = ReconstructWordFromBitTexts(cells[0], i => cells[16 - i]);
                    writeTargets.Add((type, number, value));
                }

                if (writeTargets.Count == 0)
                {
                    MessageBox.Show(this, "書き込み可能なデータがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ConfirmWrite("読み込んだファイル(デジタル)", writeTargets.Count))
                {
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort value) in writeTargets)
                {
                    await _plcClient.WriteWordsAsync(type, number, [value]);
                }

                await ReadDigitalAsync();
                MessageBox.Show(this, $"書き込みが完了しました。({writeTargets.Count}件)", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
        }

        // アナログ表示リスト用「ファイル読込」: TXTファイルを解析し、確認の上でPLCへ書き込む。
        private async void BtnAnaFileLoad_Click(object? sender, EventArgs e)
        {
            try
            {
                EnsureConnected();

                using OpenFileDialog openDialog = CreateTxtOpenFileDialog();
                if (openDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string text = await File.ReadAllTextAsync(openDialog.FileName, Encoding.UTF8);

                var writeTargets = new List<(PlcDeviceType Type, int Number, ushort[] Values)>();
                foreach (string[] cells in ParseTabSeparatedRows(text))
                {
                    if (IsHeaderRow(cells))
                    {
                        continue;
                    }

                    if (cells.Length < AnalogMinColumnCount)
                    {
                        throw new FormatException(
                            $"ファイルの列数が不足しています(先頭アドレス+10ワードの{AnalogMinColumnCount}列必要): '{cells[0]}'");
                    }

                    (PlcDeviceType type, int number) = McProtocolClient.ParseDeviceAddress(cells[0]);

                    var values = new ushort[AnalogWordCount];
                    for (int i = 0; i < AnalogWordCount; i++)
                    {
                        string valueText = cells[i + 1].Trim();
                        if (!ushort.TryParse(valueText, out values[i]))
                        {
                            throw new FormatException($"行 '{cells[0]}' の+{i}の値が不正です: '{valueText}' (0～65535の整数)");
                        }
                    }
                    writeTargets.Add((type, number, values));
                }

                if (writeTargets.Count == 0)
                {
                    MessageBox.Show(this, "書き込み可能なデータがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ConfirmWrite("読み込んだファイル(アナログ)", writeTargets.Count))
                {
                    return;
                }

                foreach ((PlcDeviceType type, int number, ushort[] values) in writeTargets)
                {
                    await _plcClient.WriteWordsAsync(type, number, values);
                }

                await ReadAnalogAsync();
                MessageBox.Show(this, $"書き込みが完了しました。({writeTargets.Count}件)", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Disconnect();
            }
        }

        // TXT保存用のSaveFileDialogを共通生成する。
        private static SaveFileDialog CreateTxtSaveFileDialog(string defaultFileName) => new()
        {
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            DefaultExt = "txt",
            FileName = defaultFileName,
            AddExtension = true,
        };

        // TXT読込用のOpenFileDialogを共通生成する。
        private static OpenFileDialog CreateTxtOpenFileDialog() => new()
        {
            Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            DefaultExt = "txt",
            CheckFileExists = true,
        };

        // 1ワード(16bit)の値をBit15～Bit0に分解し、同一アドレスの行があれば更新、なければ追加する。
        // resetBaselineがtrueの場合(PLC読込時)は今回の値を変更検知の基準(baseline)として保存する。
        private void UpdateDigitalListView(PlcDeviceType deviceType, int deviceNumber, ushort value, bool resetBaseline = false)
        {
            string addressText = McProtocolClient.FormatDeviceAddress(deviceType, deviceNumber);

            ListViewItem? item = FindItemByAddress(lvwDigital, addressText);
            bool isNewItem = item is null;
            item ??= new ListViewItem(addressText) { UseItemStyleForSubItems = false };

            if (!isNewItem)
            {
                item.SubItems.Clear();
                item.Text = addressText;
            }

            for (int bit = 15; bit >= 0; bit--)
            {
                int bitValue = (value >> bit) & 1;
                item.SubItems.Add(bitValue.ToString());
            }
            item.SubItems.Add(value.ToString());

            if (isNewItem)
            {
                lvwDigital.Items.Add(item);
            }

            if (resetBaseline || isNewItem)
            {
                item.Tag = value;
            }

            ApplyDigitalRowHighlight(item);
        }

        // 先頭アドレスから連続10ワード分の値を横に並べ、同一アドレスの行があれば更新、なければ追加する。
        // resetBaselineがtrueの場合(PLC読込時)は今回の値を変更検知の基準(baseline)として保存する。
        private void UpdateAnalogListView(PlcDeviceType deviceType, int headDeviceNumber, ushort[] values, bool resetBaseline = false)
        {
            string addressText = McProtocolClient.FormatDeviceAddress(deviceType, headDeviceNumber);

            ListViewItem? item = FindItemByAddress(lvwAnalog, addressText);
            bool isNewItem = item is null;
            item ??= new ListViewItem(addressText) { UseItemStyleForSubItems = false };

            if (!isNewItem)
            {
                item.SubItems.Clear();
                item.Text = addressText;
            }

            foreach (ushort value in values)
            {
                item.SubItems.Add(value.ToString());
            }

            if (isNewItem)
            {
                lvwAnalog.Items.Add(item);
            }

            if (resetBaseline || isNewItem)
            {
                item.Tag = (ushort[])values.Clone();
            }

            ApplyAnalogRowHighlight(item);
        }

        // OwnerDraw用: 列ヘッダーは既定の描画に任せる。
        private void ListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        // OwnerDraw用: Details表示の実際の描画はDrawSubItemで行うため既定描画に任せる。
        private void ListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        // 選択中の行の背景色(未変更セル用)。既定の選択色だと変更セルの黄色と紛らわしいため薄い水色にする。
        private static readonly Color SelectedCellBackColor = Color.LightSkyBlue;

        // 行選択時にSubItem.BackColor(変更セルの黄色)が選択色で塗り潰され、
        // どのセルが変更されたか分からなくなる問題を解消する。
        // 変更セルは常に黄色のまま描画し、未変更セルは選択中のみ薄い水色で塗る。
        private void ListView_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            if (e.SubItem is null)
            {
                e.DrawDefault = true;
                return;
            }

            ListView? listView = e.Item.ListView;
            bool isSelected = e.Item.Selected && (listView?.Focused == true || listView?.HideSelection == false);
            bool isChanged = e.SubItem.BackColor == ChangedCellBackColor;

            Color fillColor = isChanged
                ? ChangedCellBackColor
                : isSelected ? SelectedCellBackColor : e.SubItem.BackColor;

            using (var backBrush = new SolidBrush(fillColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            TextFormatFlags alignFlags = e.Header?.TextAlign switch
            {
                HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
                HorizontalAlignment.Right => TextFormatFlags.Right,
                _ => TextFormatFlags.Left,
            };

            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, e.Bounds, e.SubItem.ForeColor, alignFlags | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        // baseline(前回読込値)と現在の表示値を比較し、異なるBit列とワード数値列の背景色を変更する。
        private static void ApplyDigitalRowHighlight(ListViewItem item)
        {
            if (item.Tag is not ushort baseline)
            {
                return;
            }

            Color defaultBackColor = item.ListView?.BackColor ?? SystemColors.Window;
            bool wordChanged = false;

            for (int bit = 15; bit >= 0; bit--)
            {
                ListViewItem.ListViewSubItem bitSubItem = item.SubItems[16 - bit];
                int baselineBit = (baseline >> bit) & 1;
                bool changed = bitSubItem.Text.Trim() != baselineBit.ToString();
                bitSubItem.BackColor = changed ? ChangedCellBackColor : defaultBackColor;
                wordChanged |= changed;
            }

            item.SubItems[17].BackColor = wordChanged ? ChangedCellBackColor : defaultBackColor;
        }

        // baseline(前回読込値)と現在の表示値を比較し、異なる+0～+9列の背景色を変更する。
        private static void ApplyAnalogRowHighlight(ListViewItem item)
        {
            if (item.Tag is not ushort[] baseline)
            {
                return;
            }

            Color defaultBackColor = item.ListView?.BackColor ?? SystemColors.Window;
            for (int i = 0; i < baseline.Length && i + 1 < item.SubItems.Count; i++)
            {
                ListViewItem.ListViewSubItem subItem = item.SubItems[i + 1];
                bool changed = subItem.Text.Trim() != baseline[i].ToString();
                subItem.BackColor = changed ? ChangedCellBackColor : defaultBackColor;
            }
        }

        private static ListViewItem? FindItemByAddress(ListView listView, string address)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (string.Equals(item.Text, address, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        // ListViewのクライアント領域の高さと行の高さから、スクロールなしで表示できる行数を算出する。
        private static int CalculateVisibleRowCount(ListView listView)
        {
            Rectangle itemRect;
            if (listView.Items.Count > 0)
            {
                ListViewItem reference = listView.TopItem ?? listView.Items[0];
                itemRect = listView.GetItemRect(reference.Index);
            }
            else
            {
                // 空の場合は一時アイテムを追加して行の高さを実測する。
                ListViewItem temp = listView.Items.Add(string.Empty);
                itemRect = listView.GetItemRect(temp.Index);
                listView.Items.Remove(temp);
            }

            if (itemRect.Height <= 0)
            {
                return 1;
            }

            // itemRect.Topは列ヘッダー直下の位置のため、ヘッダー分を除いた表示領域で計算できる。
            int availableHeight = listView.ClientSize.Height - itemRect.Top;
            return Math.Max(1, availableHeight / itemRect.Height);
        }

        // ListView行のBit15～Bit0列(SubItems[1]～[16])からワード値を再構成する。
        private static ushort ReconstructWordFromBitSubItems(ListViewItem item)
        {
            if (item.SubItems.Count < DigitalMinColumnCount)
            {
                throw new FormatException($"行 '{item.Text}' の列数が不足しています。");
            }

            return ReconstructWordFromBitTexts(item.Text, bit => item.SubItems[16 - bit].Text);
        }

        // Bit15～Bit0の文字列("0"/"1")からビットORでワード値を組み立てる。
        private static ushort ReconstructWordFromBitTexts(string rowName, Func<int, string> bitTextSelector)
        {
            ushort value = 0;
            for (int bit = 15; bit >= 0; bit--)
            {
                string bitText = bitTextSelector(bit).Trim();
                value = bitText switch
                {
                    "0" => value,
                    "1" => (ushort)(value | (1 << bit)),
                    _ => throw new FormatException($"行 '{rowName}' のBit{bit}の値が不正です: '{bitText}' (0または1)"),
                };
            }
            return value;
        }

        // ListView行の+0～+9列(SubItems[1]～[10])からワード値配列を取得する。
        private static ushort[] ParseAnalogRowValues(ListViewItem item)
        {
            if (item.SubItems.Count < AnalogMinColumnCount)
            {
                throw new FormatException($"行 '{item.Text}' の列数が不足しています。");
            }

            var values = new ushort[AnalogWordCount];
            for (int i = 0; i < AnalogWordCount; i++)
            {
                string text = item.SubItems[i + 1].Text.Trim();
                if (!ushort.TryParse(text, out values[i]))
                {
                    throw new FormatException($"行 '{item.Text}' の+{i}の値が不正です: '{text}' (0～65535の整数)");
                }
            }
            return values;
        }

        // baseline(前回読込値)と現在値をワード単位で比較し、変更があった連続ワード範囲のみを抽出する。
        // baselineが無い(PLCから未読込の新規行)場合は、安全側に倒して全ワードを書き込み対象とする。
        private static List<(int Number, ushort[] Values)> GetChangedAnalogRanges(ListViewItem item, int headDeviceNumber, ushort[] currentValues)
        {
            var ranges = new List<(int Number, ushort[] Values)>();

            if (item.Tag is not ushort[] baseline || baseline.Length != currentValues.Length)
            {
                ranges.Add((headDeviceNumber, currentValues));
                return ranges;
            }

            int rangeStart = -1;
            for (int i = 0; i <= currentValues.Length; i++)
            {
                bool changed = i < currentValues.Length && currentValues[i] != baseline[i];
                if (changed)
                {
                    rangeStart = rangeStart < 0 ? i : rangeStart;
                }
                else if (rangeStart >= 0)
                {
                    ranges.Add((headDeviceNumber + rangeStart, currentValues[rangeStart..i]));
                    rangeStart = -1;
                }
            }

            return ranges;
        }

        // クリップボードのテキストを行×タブ区切りセルの一覧に分解する。
        private static List<string[]> GetClipboardRows()
        {
            if (!Clipboard.ContainsText())
            {
                throw new FormatException("クリップボードにテキストがありません。");
            }

            return ParseTabSeparatedRows(Clipboard.GetText());
        }

        // タブ区切りテキストを行×セルの一覧に分解する(クリップボード貼り付け・ファイル読み込み共通)。
        private static List<string[]> ParseTabSeparatedRows(string text)
        {
            return text
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('\t'))
                .ToList();
        }

        /// <summary>
        /// コピー機能が出力するヘッダー行(先頭セルが列名)を判定する。 
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private static bool IsHeaderRow(string[] cells)
        {
            string first = cells[0].Trim();
            return first is "アドレス" or "先頭アドレス";
        }

        /// <summary>
        /// PLC接続済みであることを確認し、未接続の場合は例外をスローする。
        /// </summary>
        /// <exception cref="PlcCommunicationException"></exception>
        private void EnsureConnected()
        {
            if (!_plcClient.IsConnected)
            {
                throw new PlcCommunicationException("PLCに接続されていません。先に接続を行ってください。");
            }
        }

        /// <summary>
        /// 書き込み前に確認ダイアログを表示
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="changedCount"></param>
        /// <returns></returns>
        private bool ConfirmWrite(string targetName, int changedCount)
        {
            string sMessage = $"{targetName}の変更された{changedCount}件をPLCへ書き込みます。よろしいですか？";
            DialogResult result = MessageBox.Show(this,sMessage,"書き込み確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            return result == DialogResult.OK;
        }

        // 例外種別に応じてメッセージボックスの表題・アイコンを切り替える。
        private void ShowError(Exception ex)
        {
            switch (ex)
            {
                case FormatException or ArgumentException:
                    MessageBox.Show(this, ex.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case PlcCommunicationException:
                    MessageBox.Show(this, ex.Message, "通信エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                default:
                    MessageBox.Show(this, $"予期しないエラーが発生しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // PLCのデジタル読込アドレス入力欄でEnterキーが押されたら読込ボタンを押したことにする。
        private void TxtDigReadAddress_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    DigRead();
                    break;
            }
        }

        // PLCのアナログ読込アドレス入力欄でEnterキーが押されたら読込ボタンを押したことにする。
        private void TxtAnaReadAddress_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    AnaRead();
                    break;
            }
        }

    }
}
