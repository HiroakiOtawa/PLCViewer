namespace PLCViewer
{
    /// <summary>
    ///  MCプロトコルでアクセスするPLCデバイスの種別。
    /// </summary>
    public enum PlcDeviceType
    {
        /// <summary>データレジスタ(D)。ワードデバイス、10進アドレス。</summary>
        D,

        /// <summary>リンクレジスタ(W)。ワードデバイス、16進アドレス。</summary>
        W,

        /// <summary>内部リレー(M)。ビットデバイス、10進アドレス。</summary>
        M,

        /// <summary>リンクリレー(B)。ビットデバイス、16進アドレス。</summary>
        B,
    }
}
