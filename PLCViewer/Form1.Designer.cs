namespace PLCViewer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            _tlpMain = new TableLayoutPanel();
            _grpConnection = new GroupBox();
            _tlpConnection = new TableLayoutPanel();
            _flpConnection = new FlowLayoutPanel();
            _lblPlcSeries = new Label();
            cboPlcSeries = new ComboBox();
            _lblIp = new Label();
            txtIpAddress = new TextBox();
            _lblPort = new Label();
            txtPort = new TextBox();
            btnConnect = new Button();
            btnDisconnect = new Button();
            lblStatus = new Label();
            _grpDigital = new GroupBox();
            _tlpDigital = new TableLayoutPanel();
            _flpDigitalControls = new FlowLayoutPanel();
            _lblDigDevice = new Label();
            cboDigDevice = new ComboBox();
            _lblDigAddress = new Label();
            txtDigReadAddress = new TextBox();
            btnDigRead = new Button();
            rdoDigContinuous = new RadioButton();
            btnDigWrite = new Button();
            btnDigCopy = new Button();
            btnDigPaste = new Button();
            chkDigBitOrderReversed = new CheckBox();
            lvwDigital = new ListView();
            timerDigContinuous = new System.Windows.Forms.Timer(components);
            _grpAnalog = new GroupBox();
            _tlpAnalog = new TableLayoutPanel();
            _flpAnalogControls = new FlowLayoutPanel();
            _lblAnaDevice = new Label();
            cboAnaDevice = new ComboBox();
            _lblAnaAddress = new Label();
            txtAnaReadAddress = new TextBox();
            btnAnaRead = new Button();
            rdoAnaContinuous = new RadioButton();
            btnAnaWrite = new Button();
            btnAnaCopy = new Button();
            btnAnaPaste = new Button();
            lvwAnalog = new ListView();
            timerAnaContinuous = new System.Windows.Forms.Timer(components);
            _tlpMain.SuspendLayout();
            _grpConnection.SuspendLayout();
            _tlpConnection.SuspendLayout();
            _flpConnection.SuspendLayout();
            _grpDigital.SuspendLayout();
            _tlpDigital.SuspendLayout();
            _flpDigitalControls.SuspendLayout();
            _grpAnalog.SuspendLayout();
            _tlpAnalog.SuspendLayout();
            _flpAnalogControls.SuspendLayout();
            SuspendLayout();
            // 
            // _tlpMain
            // 
            _tlpMain.ColumnCount = 1;
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpMain.Controls.Add(_grpConnection, 0, 0);
            _tlpMain.Controls.Add(_grpDigital, 0, 1);
            _tlpMain.Controls.Add(_grpAnalog, 0, 2);
            _tlpMain.Dock = DockStyle.Fill;
            _tlpMain.Location = new Point(0, 0);
            _tlpMain.Name = "_tlpMain";
            _tlpMain.RowCount = 3;
            _tlpMain.RowStyles.Add(new RowStyle());
            _tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tlpMain.Size = new Size(1150, 780);
            _tlpMain.TabIndex = 0;
            // 
            // _grpConnection
            // 
            _grpConnection.AutoSize = true;
            _grpConnection.Controls.Add(_tlpConnection);
            _grpConnection.Dock = DockStyle.Fill;
            _grpConnection.Location = new Point(3, 3);
            _grpConnection.Name = "_grpConnection";
            _grpConnection.Size = new Size(1144, 86);
            _grpConnection.TabIndex = 0;
            _grpConnection.TabStop = false;
            _grpConnection.Text = "接続設定";
            // 
            // _tlpConnection
            // 
            _tlpConnection.AutoSize = true;
            _tlpConnection.ColumnCount = 1;
            _tlpConnection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpConnection.Controls.Add(_flpConnection, 0, 0);
            _tlpConnection.Controls.Add(lblStatus, 0, 1);
            _tlpConnection.Dock = DockStyle.Fill;
            _tlpConnection.Location = new Point(3, 19);
            _tlpConnection.Name = "_tlpConnection";
            _tlpConnection.RowCount = 2;
            _tlpConnection.RowStyles.Add(new RowStyle());
            _tlpConnection.RowStyles.Add(new RowStyle());
            _tlpConnection.Size = new Size(1138, 64);
            _tlpConnection.TabIndex = 0;
            // 
            // _flpConnection
            // 
            _flpConnection.AutoSize = true;
            _flpConnection.Controls.Add(_lblPlcSeries);
            _flpConnection.Controls.Add(cboPlcSeries);
            _flpConnection.Controls.Add(_lblIp);
            _flpConnection.Controls.Add(txtIpAddress);
            _flpConnection.Controls.Add(_lblPort);
            _flpConnection.Controls.Add(txtPort);
            _flpConnection.Controls.Add(btnConnect);
            _flpConnection.Controls.Add(btnDisconnect);
            _flpConnection.Dock = DockStyle.Fill;
            _flpConnection.Location = new Point(3, 3);
            _flpConnection.Name = "_flpConnection";
            _flpConnection.Size = new Size(1132, 29);
            _flpConnection.TabIndex = 0;
            _flpConnection.WrapContents = false;
            // 
            // _lblPlcSeries
            // 
            _lblPlcSeries.AutoSize = true;
            _lblPlcSeries.Location = new Point(3, 8);
            _lblPlcSeries.Margin = new Padding(3, 8, 3, 3);
            _lblPlcSeries.Name = "_lblPlcSeries";
            _lblPlcSeries.Size = new Size(65, 15);
            _lblPlcSeries.TabIndex = 0;
            _lblPlcSeries.Text = "PLCシリーズ:";
            // 
            // cboPlcSeries
            // 
            cboPlcSeries.AccessibleName = "PLCシリーズ";
            cboPlcSeries.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPlcSeries.Location = new Point(74, 3);
            cboPlcSeries.Margin = new Padding(3, 3, 12, 3);
            cboPlcSeries.Name = "cboPlcSeries";
            cboPlcSeries.Size = new Size(160, 23);
            cboPlcSeries.TabIndex = 1;
            cboPlcSeries.SelectedIndexChanged += CboPlcSeries_SelectedIndexChanged;
            // 
            // _lblIp
            // 
            _lblIp.AutoSize = true;
            _lblIp.Location = new Point(249, 8);
            _lblIp.Margin = new Padding(3, 8, 3, 3);
            _lblIp.Name = "_lblIp";
            _lblIp.Size = new Size(55, 15);
            _lblIp.TabIndex = 2;
            _lblIp.Text = "IPアドレス:";
            // 
            // txtIpAddress
            // 
            txtIpAddress.AccessibleName = "IPアドレス";
            txtIpAddress.Location = new Point(310, 3);
            txtIpAddress.Margin = new Padding(3, 3, 12, 3);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(130, 23);
            txtIpAddress.TabIndex = 3;
            txtIpAddress.Text = "192.168.2.10";
            // 
            // _lblPort
            // 
            _lblPort.AutoSize = true;
            _lblPort.Location = new Point(455, 8);
            _lblPort.Margin = new Padding(3, 8, 3, 3);
            _lblPort.Name = "_lblPort";
            _lblPort.Size = new Size(60, 15);
            _lblPort.TabIndex = 4;
            _lblPort.Text = "ポート番号:";
            // 
            // txtPort
            // 
            txtPort.AccessibleName = "ポート番号";
            txtPort.Location = new Point(521, 3);
            txtPort.Margin = new Padding(3, 3, 12, 3);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(60, 23);
            txtPort.TabIndex = 5;
            txtPort.Text = "10000";
            // 
            // btnConnect
            // 
            btnConnect.AccessibleDescription = "PLCへ接続します";
            btnConnect.AccessibleName = "接続";
            btnConnect.Location = new Point(596, 3);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(90, 23);
            btnConnect.TabIndex = 6;
            btnConnect.Text = "接続";
            btnConnect.Click += BtnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.AccessibleDescription = "PLCから切断します";
            btnDisconnect.AccessibleName = "切断";
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(692, 3);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(90, 23);
            btnDisconnect.TabIndex = 7;
            btnDisconnect.Text = "切断";
            btnDisconnect.Click += BtnDisconnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(3, 41);
            lblStatus.Margin = new Padding(3, 6, 3, 8);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(43, 15);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "未接続";
            // 
            // _grpDigital
            // 
            _grpDigital.Controls.Add(_tlpDigital);
            _grpDigital.Dock = DockStyle.Fill;
            _grpDigital.Location = new Point(3, 95);
            _grpDigital.Name = "_grpDigital";
            _grpDigital.Size = new Size(1144, 338);
            _grpDigital.TabIndex = 1;
            _grpDigital.TabStop = false;
            _grpDigital.Text = "デジタル表示（1ワード ビット単位 / M・B）";
            // 
            // _tlpDigital
            // 
            _tlpDigital.ColumnCount = 1;
            _tlpDigital.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpDigital.Controls.Add(_flpDigitalControls, 0, 0);
            _tlpDigital.Controls.Add(lvwDigital, 0, 1);
            _tlpDigital.Dock = DockStyle.Fill;
            _tlpDigital.Location = new Point(3, 19);
            _tlpDigital.Name = "_tlpDigital";
            _tlpDigital.RowCount = 2;
            _tlpDigital.RowStyles.Add(new RowStyle());
            _tlpDigital.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpDigital.Size = new Size(1138, 316);
            _tlpDigital.TabIndex = 0;
            // 
            // _flpDigitalControls
            // 
            _flpDigitalControls.AutoSize = true;
            _flpDigitalControls.Controls.Add(_lblDigDevice);
            _flpDigitalControls.Controls.Add(cboDigDevice);
            _flpDigitalControls.Controls.Add(_lblDigAddress);
            _flpDigitalControls.Controls.Add(txtDigReadAddress);
            _flpDigitalControls.Controls.Add(btnDigRead);
            _flpDigitalControls.Controls.Add(rdoDigContinuous);
            _flpDigitalControls.Controls.Add(btnDigWrite);
            _flpDigitalControls.Controls.Add(btnDigCopy);
            _flpDigitalControls.Controls.Add(btnDigPaste);
            _flpDigitalControls.Controls.Add(chkDigBitOrderReversed);
            _flpDigitalControls.Dock = DockStyle.Fill;
            _flpDigitalControls.Location = new Point(3, 3);
            _flpDigitalControls.Name = "_flpDigitalControls";
            _flpDigitalControls.Size = new Size(1132, 29);
            _flpDigitalControls.TabIndex = 0;
            _flpDigitalControls.WrapContents = false;
            // 
            // _lblDigDevice
            // 
            _lblDigDevice.AutoSize = true;
            _lblDigDevice.Location = new Point(3, 8);
            _lblDigDevice.Margin = new Padding(3, 8, 3, 3);
            _lblDigDevice.Name = "_lblDigDevice";
            _lblDigDevice.Size = new Size(47, 15);
            _lblDigDevice.TabIndex = 0;
            _lblDigDevice.Text = "デバイス:";
            // 
            // cboDigDevice
            // 
            cboDigDevice.AccessibleName = "デジタル表示 デバイス種別";
            cboDigDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDigDevice.Location = new Point(56, 3);
            cboDigDevice.Margin = new Padding(3, 3, 12, 3);
            cboDigDevice.Name = "cboDigDevice";
            cboDigDevice.Size = new Size(60, 23);
            cboDigDevice.TabIndex = 1;
            // 
            // _lblDigAddress
            // 
            _lblDigAddress.AutoSize = true;
            _lblDigAddress.Location = new Point(131, 8);
            _lblDigAddress.Margin = new Padding(3, 8, 3, 3);
            _lblDigAddress.Name = "_lblDigAddress";
            _lblDigAddress.Size = new Size(115, 15);
            _lblDigAddress.TabIndex = 2;
            _lblDigAddress.Text = "読み込み開始アドレス:";
            // 
            // txtDigReadAddress
            // 
            txtDigReadAddress.AccessibleName = "デジタル表示 読み込み開始アドレス";
            txtDigReadAddress.Location = new Point(252, 3);
            txtDigReadAddress.Margin = new Padding(3, 3, 12, 3);
            txtDigReadAddress.Name = "txtDigReadAddress";
            txtDigReadAddress.Size = new Size(80, 23);
            txtDigReadAddress.TabIndex = 3;
            txtDigReadAddress.Text = "M0";
            txtDigReadAddress.KeyDown += TxtDigReadAddress_KeyDown;
            // 
            // btnDigRead
            // 
            btnDigRead.AccessibleDescription = "デジタル表示エリアへ1ワード分読み込みます";
            btnDigRead.AccessibleName = "デジタル読み込み実行";
            btnDigRead.Location = new Point(347, 3);
            btnDigRead.Name = "btnDigRead";
            btnDigRead.Size = new Size(80, 23);
            btnDigRead.TabIndex = 4;
            btnDigRead.Text = "読込";
            btnDigRead.Click += BtnDigRead_Click;
            // 
            // rdoDigContinuous
            // 
            rdoDigContinuous.AccessibleDescription = "ONの間、1秒間隔でデジタル表示エリアへの読み込みを繰り返します";
            rdoDigContinuous.AccessibleName = "デジタル連続読み込み";
            rdoDigContinuous.AutoSize = true;
            rdoDigContinuous.Location = new Point(433, 6);
            rdoDigContinuous.Margin = new Padding(6, 6, 3, 3);
            rdoDigContinuous.Name = "rdoDigContinuous";
            rdoDigContinuous.Size = new Size(85, 19);
            rdoDigContinuous.TabIndex = 5;
            rdoDigContinuous.Text = "連続読み込み";
            rdoDigContinuous.UseVisualStyleBackColor = true;
            rdoDigContinuous.Click += RdoDigContinuous_Click;
            // 
            // btnDigWrite
            // 
            btnDigWrite.AccessibleDescription = "デジタル表示リストの全行をPLCへ一括書き込みます";
            btnDigWrite.AccessibleName = "デジタルPLCへ書込";
            btnDigWrite.Location = new Point(533, 3);
            btnDigWrite.Margin = new Padding(12, 3, 3, 3);
            btnDigWrite.Name = "btnDigWrite";
            btnDigWrite.Size = new Size(90, 23);
            btnDigWrite.TabIndex = 6;
            btnDigWrite.Text = "PLCへ書込";
            btnDigWrite.Click += BtnDigWrite_Click;
            // 
            // btnDigCopy
            // 
            btnDigCopy.AccessibleDescription = "デジタル表示リストの内容をクリップボードへコピーします";
            btnDigCopy.AccessibleName = "デジタルコピー";
            btnDigCopy.Location = new Point(575, 3);
            btnDigCopy.Margin = new Padding(40, 3, 3, 3);
            btnDigCopy.Name = "btnDigCopy";
            btnDigCopy.Size = new Size(80, 23);
            btnDigCopy.TabIndex = 7;
            btnDigCopy.Text = "コピー";
            btnDigCopy.Click += BtnDigCopy_Click;
            // 
            // btnDigPaste
            // 
            btnDigPaste.AccessibleDescription = "クリップボードの内容をデジタル表示リストへ貼り付けます";
            btnDigPaste.AccessibleName = "デジタル貼り付け";
            btnDigPaste.Location = new Point(661, 3);
            btnDigPaste.Name = "btnDigPaste";
            btnDigPaste.Size = new Size(80, 23);
            btnDigPaste.TabIndex = 8;
            btnDigPaste.Text = "貼り付け";
            btnDigPaste.Click += BtnDigPaste_Click;
            // 
            // chkDigBitOrderReversed
            // 
            chkDigBitOrderReversed.AccessibleDescription = "チェック時はBit0を左端、Bit15を右端に表示します";
            chkDigBitOrderReversed.AccessibleName = "デジタル表示 ビット順反転";
            chkDigBitOrderReversed.AutoSize = true;
            chkDigBitOrderReversed.Location = new Point(747, 6);
            chkDigBitOrderReversed.Margin = new Padding(3, 6, 3, 3);
            chkDigBitOrderReversed.Name = "chkDigBitOrderReversed";
            chkDigBitOrderReversed.Size = new Size(133, 19);
            chkDigBitOrderReversed.TabIndex = 9;
            chkDigBitOrderReversed.Text = "ビット順を反転(0→15)";
            chkDigBitOrderReversed.UseVisualStyleBackColor = true;
            chkDigBitOrderReversed.CheckedChanged += ChkDigBitOrderReversed_CheckedChanged;
            // 
            // lvwDigital
            // 
            lvwDigital.AccessibleName = "デジタル表示リスト";
            lvwDigital.Dock = DockStyle.Fill;
            lvwDigital.FullRowSelect = true;
            lvwDigital.GridLines = true;
            lvwDigital.Location = new Point(3, 41);
            lvwDigital.Margin = new Padding(3, 6, 3, 3);
            lvwDigital.Name = "lvwDigital";
            lvwDigital.OwnerDraw = true;
            lvwDigital.Size = new Size(1132, 272);
            lvwDigital.TabIndex = 1;
            lvwDigital.UseCompatibleStateImageBehavior = false;
            lvwDigital.View = View.Details;
            lvwDigital.DrawColumnHeader += this.ListView_DrawColumnHeader;
            lvwDigital.DrawItem += this.ListView_DrawItem;
            lvwDigital.DrawSubItem += this.ListView_DrawSubItem;
            lvwDigital.MouseDoubleClick += LvwDigital_MouseDoubleClick;
            // 
            // _grpAnalog
            // 
            _grpAnalog.Controls.Add(_tlpAnalog);
            _grpAnalog.Dock = DockStyle.Fill;
            _grpAnalog.Location = new Point(3, 439);
            _grpAnalog.Name = "_grpAnalog";
            _grpAnalog.Size = new Size(1144, 338);
            _grpAnalog.TabIndex = 2;
            _grpAnalog.TabStop = false;
            _grpAnalog.Text = "アナログ表示（連続10ワード / D・W）";
            // 
            // _tlpAnalog
            // 
            _tlpAnalog.ColumnCount = 1;
            _tlpAnalog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpAnalog.Controls.Add(_flpAnalogControls, 0, 0);
            _tlpAnalog.Controls.Add(lvwAnalog, 0, 1);
            _tlpAnalog.Dock = DockStyle.Fill;
            _tlpAnalog.Location = new Point(3, 19);
            _tlpAnalog.Name = "_tlpAnalog";
            _tlpAnalog.RowCount = 2;
            _tlpAnalog.RowStyles.Add(new RowStyle());
            _tlpAnalog.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpAnalog.Size = new Size(1138, 316);
            _tlpAnalog.TabIndex = 0;
            // 
            // _flpAnalogControls
            // 
            _flpAnalogControls.AutoSize = true;
            _flpAnalogControls.Controls.Add(_lblAnaDevice);
            _flpAnalogControls.Controls.Add(cboAnaDevice);
            _flpAnalogControls.Controls.Add(_lblAnaAddress);
            _flpAnalogControls.Controls.Add(txtAnaReadAddress);
            _flpAnalogControls.Controls.Add(btnAnaRead);
            _flpAnalogControls.Controls.Add(rdoAnaContinuous);
            _flpAnalogControls.Controls.Add(btnAnaWrite);
            _flpAnalogControls.Controls.Add(btnAnaCopy);
            _flpAnalogControls.Controls.Add(btnAnaPaste);
            _flpAnalogControls.Dock = DockStyle.Fill;
            _flpAnalogControls.Location = new Point(3, 3);
            _flpAnalogControls.Name = "_flpAnalogControls";
            _flpAnalogControls.Size = new Size(1132, 29);
            _flpAnalogControls.TabIndex = 0;
            _flpAnalogControls.WrapContents = false;
            // 
            // _lblAnaDevice
            // 
            _lblAnaDevice.AutoSize = true;
            _lblAnaDevice.Location = new Point(3, 8);
            _lblAnaDevice.Margin = new Padding(3, 8, 3, 3);
            _lblAnaDevice.Name = "_lblAnaDevice";
            _lblAnaDevice.Size = new Size(47, 15);
            _lblAnaDevice.TabIndex = 0;
            _lblAnaDevice.Text = "デバイス:";
            // 
            // cboAnaDevice
            // 
            cboAnaDevice.AccessibleName = "アナログ表示 デバイス種別";
            cboAnaDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAnaDevice.Location = new Point(56, 3);
            cboAnaDevice.Margin = new Padding(3, 3, 12, 3);
            cboAnaDevice.Name = "cboAnaDevice";
            cboAnaDevice.Size = new Size(60, 23);
            cboAnaDevice.TabIndex = 1;
            // 
            // _lblAnaAddress
            // 
            _lblAnaAddress.AutoSize = true;
            _lblAnaAddress.Location = new Point(131, 8);
            _lblAnaAddress.Margin = new Padding(3, 8, 3, 3);
            _lblAnaAddress.Name = "_lblAnaAddress";
            _lblAnaAddress.Size = new Size(115, 15);
            _lblAnaAddress.TabIndex = 2;
            _lblAnaAddress.Text = "読み込み開始アドレス:";
            // 
            // txtAnaReadAddress
            // 
            txtAnaReadAddress.AccessibleName = "アナログ表示 読み込み開始アドレス";
            txtAnaReadAddress.Location = new Point(252, 3);
            txtAnaReadAddress.Margin = new Padding(3, 3, 12, 3);
            txtAnaReadAddress.Name = "txtAnaReadAddress";
            txtAnaReadAddress.Size = new Size(80, 23);
            txtAnaReadAddress.TabIndex = 3;
            txtAnaReadAddress.Text = "D200";
            txtAnaReadAddress.KeyDown += TxtAnaReadAddress_KeyDown;
            // 
            // btnAnaRead
            // 
            btnAnaRead.AccessibleDescription = "アナログ表示エリアへ連続10ワード分読み込みます";
            btnAnaRead.AccessibleName = "アナログ読み込み実行";
            btnAnaRead.Location = new Point(347, 3);
            btnAnaRead.Name = "btnAnaRead";
            btnAnaRead.Size = new Size(80, 23);
            btnAnaRead.TabIndex = 4;
            btnAnaRead.Text = "読込";
            btnAnaRead.Click += BtnAnaRead_Click;
            // 
            // rdoAnaContinuous
            // 
            rdoAnaContinuous.AccessibleDescription = "ONの間、1秒間隔でアナログ表示エリアへの読み込みを繰り返します";
            rdoAnaContinuous.AccessibleName = "アナログ連続読み込み";
            rdoAnaContinuous.AutoSize = true;
            rdoAnaContinuous.Location = new Point(433, 6);
            rdoAnaContinuous.Margin = new Padding(6, 6, 3, 3);
            rdoAnaContinuous.Name = "rdoAnaContinuous";
            rdoAnaContinuous.Size = new Size(85, 19);
            rdoAnaContinuous.TabIndex = 5;
            rdoAnaContinuous.Text = "連続読み込み";
            rdoAnaContinuous.UseVisualStyleBackColor = true;
            rdoAnaContinuous.Click += RdoAnaContinuous_Click;
            // 
            // btnAnaWrite
            // 
            btnAnaWrite.AccessibleDescription = "アナログ表示リストの全行をPLCへ一括書き込みます";
            btnAnaWrite.AccessibleName = "アナログPLCへ書込";
            btnAnaWrite.Location = new Point(533, 3);
            btnAnaWrite.Margin = new Padding(12, 3, 3, 3);
            btnAnaWrite.Name = "btnAnaWrite";
            btnAnaWrite.Size = new Size(90, 23);
            btnAnaWrite.TabIndex = 6;
            btnAnaWrite.Text = "PLCへ書込";
            btnAnaWrite.Click += BtnAnaWrite_Click;
            // 
            // btnAnaCopy
            // 
            btnAnaCopy.AccessibleDescription = "アナログ表示リストの内容をクリップボードへコピーします";
            btnAnaCopy.AccessibleName = "アナログコピー";
            btnAnaCopy.Location = new Point(575, 3);
            btnAnaCopy.Margin = new Padding(40, 3, 3, 3);
            btnAnaCopy.Name = "btnAnaCopy";
            btnAnaCopy.Size = new Size(80, 23);
            btnAnaCopy.TabIndex = 7;
            btnAnaCopy.Text = "コピー";
            btnAnaCopy.Click += BtnAnaCopy_Click;
            // 
            // btnAnaPaste
            // 
            btnAnaPaste.AccessibleDescription = "クリップボードの内容をアナログ表示リストへ貼り付けます";
            btnAnaPaste.AccessibleName = "アナログ貼り付け";
            btnAnaPaste.Location = new Point(661, 3);
            btnAnaPaste.Name = "btnAnaPaste";
            btnAnaPaste.Size = new Size(80, 23);
            btnAnaPaste.TabIndex = 8;
            btnAnaPaste.Text = "貼り付け";
            btnAnaPaste.Click += BtnAnaPaste_Click;
            // 
            // lvwAnalog
            // 
            lvwAnalog.AccessibleName = "アナログ表示リスト";
            lvwAnalog.Dock = DockStyle.Fill;
            lvwAnalog.FullRowSelect = true;
            lvwAnalog.GridLines = true;
            lvwAnalog.Location = new Point(3, 41);
            lvwAnalog.Margin = new Padding(3, 6, 3, 3);
            lvwAnalog.Name = "lvwAnalog";
            lvwAnalog.OwnerDraw = true;
            lvwAnalog.Size = new Size(1132, 272);
            lvwAnalog.TabIndex = 1;
            lvwAnalog.UseCompatibleStateImageBehavior = false;
            lvwAnalog.View = View.Details;
            lvwAnalog.DrawColumnHeader += this.ListView_DrawColumnHeader;
            lvwAnalog.DrawItem += this.ListView_DrawItem;
            lvwAnalog.DrawSubItem += this.ListView_DrawSubItem;
            lvwAnalog.MouseDoubleClick += LvwAnalog_MouseDoubleClick;
            // 
            // timerDigContinuous
            // 
            timerDigContinuous.Interval = 1000;
            timerDigContinuous.Tick += TimerDigContinuous_Tick;
            // 
            // timerAnaContinuous
            // 
            timerAnaContinuous.Interval = 1000;
            timerAnaContinuous.Tick += TimerAnaContinuous_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1150, 780);
            Controls.Add(_tlpMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1000, 650);
            Name = "Form1";
            Text = "PLC デバイスビューア";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            _tlpMain.ResumeLayout(false);
            _tlpMain.PerformLayout();
            _grpConnection.ResumeLayout(false);
            _grpConnection.PerformLayout();
            _tlpConnection.ResumeLayout(false);
            _tlpConnection.PerformLayout();
            _flpConnection.ResumeLayout(false);
            _flpConnection.PerformLayout();
            _grpDigital.ResumeLayout(false);
            _tlpDigital.ResumeLayout(false);
            _tlpDigital.PerformLayout();
            _flpDigitalControls.ResumeLayout(false);
            _flpDigitalControls.PerformLayout();
            _grpAnalog.ResumeLayout(false);
            _tlpAnalog.ResumeLayout(false);
            _tlpAnalog.PerformLayout();
            _flpAnalogControls.ResumeLayout(false);
            _flpAnalogControls.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _tlpMain;
        private GroupBox _grpConnection;
        private TableLayoutPanel _tlpConnection;
        private FlowLayoutPanel _flpConnection;
        private Label _lblPlcSeries;
        private ComboBox cboPlcSeries;
        private Label _lblIp;
        private TextBox txtIpAddress;
        private Label _lblPort;
        private TextBox txtPort;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;
        private GroupBox _grpDigital;
        private TableLayoutPanel _tlpDigital;
        private FlowLayoutPanel _flpDigitalControls;
        private Label _lblDigDevice;
        private ComboBox cboDigDevice;
        private Label _lblDigAddress;
        private TextBox txtDigReadAddress;
        private Button btnDigRead;
        private RadioButton rdoDigContinuous;
        private Button btnDigWrite;
        private Button btnDigCopy;
        private Button btnDigPaste;
        private CheckBox chkDigBitOrderReversed;
        private ListView lvwDigital;
        private System.Windows.Forms.Timer timerDigContinuous;
        private GroupBox _grpAnalog;
        private TableLayoutPanel _tlpAnalog;
        private FlowLayoutPanel _flpAnalogControls;
        private Label _lblAnaDevice;
        private ComboBox cboAnaDevice;
        private Label _lblAnaAddress;
        private TextBox txtAnaReadAddress;
        private Button btnAnaRead;
        private RadioButton rdoAnaContinuous;
        private Button btnAnaWrite;
        private Button btnAnaCopy;
        private Button btnAnaPaste;
        private ListView lvwAnalog;
        private System.Windows.Forms.Timer timerAnaContinuous;
    }
}
