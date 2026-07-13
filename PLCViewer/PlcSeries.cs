using System;

namespace PLCViewer
{
    /// <summary>
    ///  接続対象のPLCシリーズ。MCプロトコルのフレーム形式(3E/4E)やデバイス仕様の切り替えに使用する。
    /// </summary>
    public enum PlcSeries
    {
        /// <summary>三菱電機 Qシリーズ。MCプロトコル 3Eフレームを使用する。</summary>
        MitsubishiQ,

        /// <summary>三菱電機 iQ-Rシリーズ。MCプロトコル 4Eフレーム(シリアル番号付き拡張フレーム)を使用する。</summary>
        MitsubishiIqR,

        /// <summary>KEYENCE KVシリーズ。MCプロトコル(三菱互換)3Eフレームを使用する。</summary>
        KeyenceKv,
    }

    /// <summary>
    ///  <see cref="PlcSeries"/> の画面表示用名称を取得する拡張メソッド。
    /// </summary>
    public static class PlcSeriesExtensions
    {
        /// <summary>
        ///  PLCシリーズの画面表示用名称を取得する。
        /// </summary>
        public static string GetDisplayName(this PlcSeries series) => series switch
        {
            PlcSeries.MitsubishiQ => "三菱 Qシリーズ",
            PlcSeries.MitsubishiIqR => "三菱 iQ-Rシリーズ",
            PlcSeries.KeyenceKv => "KEYENCE KVシリーズ",
            _ => throw new ArgumentOutOfRangeException(nameof(series), series, "未対応のPLCシリーズです。"),
        };
    }
}
