namespace PLCViewer
{
    /// <summary>
    ///  ファイル書き出し/読み込み時に、PLCから読み込む(または読み込んだファイルの妥当性を検証する)
    ///  デバイスの開始アドレスと点数(ワード数)を指定させるための汎用ダイアログ。
    /// </summary>
    public sealed partial class DeviceRangeDialog : Form
    {
        private readonly PlcDeviceType _deviceType;
        private readonly int _maxCount;

        /// <summary>
        ///  OKで確定した開始デバイス番号。
        /// </summary>
        public int DeviceNumber { get; private set; }

        /// <summary>
        ///  OKで確定した読み込み点数(ワード数)。
        /// </summary>
        public int Count { get; private set; }

        public DeviceRangeDialog(string title, PlcDeviceType deviceType, string initialAddress, string countLabelText, int initialCount, int maxCount)
        {
            InitializeComponent();

            _deviceType = deviceType;
            _maxCount = maxCount;

            Text = title;
            _txtAddress.Text = initialAddress;
            _lblCount.Text = countLabelText;
            _txtCount.Text = initialCount.ToString();
        }

        // OKクリック時にアドレス・件数を検証する。不正な場合はダイアログを閉じずにエラー表示のみ行う。
        private void BtnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                DeviceNumber = McProtocolClient.ParseDeviceNumber(_deviceType, _txtAddress.Text);

                if (!int.TryParse(_txtCount.Text.Trim(), out int count) || count is < 1 || count > _maxCount)
                {
                    throw new FormatException($"件数は1～{_maxCount}の範囲で入力してください。");
                }

                Count = count;
                DialogResult = DialogResult.OK;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(this, ex.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
