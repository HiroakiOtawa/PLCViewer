namespace PLCViewer
{
    partial class DeviceRangeDialog
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
            _tlpMain = new TableLayoutPanel();
            _lblAddress = new Label();
            _txtAddress = new TextBox();
            _lblCount = new Label();
            _txtCount = new TextBox();
            _flpButtons = new FlowLayoutPanel();
            _btnCancel = new Button();
            _btnOK = new Button();
            _tlpMain.SuspendLayout();
            _flpButtons.SuspendLayout();
            SuspendLayout();
            // 
            // _tlpMain
            // 
            _tlpMain.ColumnCount = 2;
            _tlpMain.ColumnStyles.Add(new ColumnStyle());
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpMain.Controls.Add(_lblAddress, 0, 0);
            _tlpMain.Controls.Add(_txtAddress, 1, 0);
            _tlpMain.Controls.Add(_lblCount, 0, 1);
            _tlpMain.Controls.Add(_txtCount, 1, 1);
            _tlpMain.Controls.Add(_flpButtons, 0, 2);
            _tlpMain.SetColumnSpan(_flpButtons, 2);
            _tlpMain.Dock = DockStyle.Fill;
            _tlpMain.Location = new Point(0, 0);
            _tlpMain.Name = "_tlpMain";
            _tlpMain.Padding = new Padding(12);
            _tlpMain.RowCount = 3;
            _tlpMain.RowStyles.Add(new RowStyle());
            _tlpMain.RowStyles.Add(new RowStyle());
            _tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpMain.Size = new Size(340, 150);
            _tlpMain.TabIndex = 0;
            // 
            // _lblAddress
            // 
            _lblAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblAddress.AutoSize = true;
            _lblAddress.Location = new Point(15, 20);
            _lblAddress.Margin = new Padding(3, 8, 3, 3);
            _lblAddress.Name = "_lblAddress";
            _lblAddress.Size = new Size(74, 15);
            _lblAddress.TabIndex = 0;
            _lblAddress.Text = "開始アドレス:";
            // 
            // _txtAddress
            // 
            _txtAddress.AccessibleName = "開始アドレス";
            _txtAddress.Dock = DockStyle.Fill;
            _txtAddress.Location = new Point(98, 15);
            _txtAddress.Margin = new Padding(3, 3, 3, 6);
            _txtAddress.Name = "_txtAddress";
            _txtAddress.Size = new Size(215, 23);
            _txtAddress.TabIndex = 1;
            // 
            // _lblCount
            // 
            _lblCount.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblCount.AutoSize = true;
            _lblCount.Location = new Point(15, 51);
            _lblCount.Margin = new Padding(3, 8, 3, 3);
            _lblCount.Name = "_lblCount";
            _lblCount.Size = new Size(38, 15);
            _lblCount.TabIndex = 2;
            _lblCount.Text = "件数:";
            // 
            // _txtCount
            // 
            _txtCount.AccessibleName = "件数";
            _txtCount.Dock = DockStyle.Fill;
            _txtCount.Location = new Point(98, 47);
            _txtCount.Margin = new Padding(3, 3, 3, 6);
            _txtCount.Name = "_txtCount";
            _txtCount.Size = new Size(215, 23);
            _txtCount.TabIndex = 3;
            // 
            // _flpButtons
            // 
            _flpButtons.AutoSize = true;
            _flpButtons.Controls.Add(_btnCancel);
            _flpButtons.Controls.Add(_btnOK);
            _flpButtons.Dock = DockStyle.Fill;
            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Location = new Point(15, 82);
            _flpButtons.Margin = new Padding(3, 12, 3, 3);
            _flpButtons.Name = "_flpButtons";
            _flpButtons.Size = new Size(298, 33);
            _flpButtons.TabIndex = 4;
            // 
            // _btnCancel
            // 
            _btnCancel.AccessibleName = "キャンセル";
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(215, 3);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(80, 27);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "キャンセル";
            // 
            // _btnOK
            // 
            _btnOK.AccessibleName = "OK";
            _btnOK.DialogResult = DialogResult.OK;
            _btnOK.Location = new Point(129, 3);
            _btnOK.Margin = new Padding(3, 3, 6, 3);
            _btnOK.Name = "_btnOK";
            _btnOK.Size = new Size(80, 27);
            _btnOK.TabIndex = 0;
            _btnOK.Text = "OK";
            _btnOK.Click += BtnOK_Click;
            // 
            // DeviceRangeDialog
            // 
            AcceptButton = _btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _btnCancel;
            ClientSize = new Size(340, 150);
            Controls.Add(_tlpMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DeviceRangeDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "範囲指定";
            _tlpMain.ResumeLayout(false);
            _tlpMain.PerformLayout();
            _flpButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _tlpMain;
        private Label _lblAddress;
        private TextBox _txtAddress;
        private Label _lblCount;
        private TextBox _txtCount;
        private FlowLayoutPanel _flpButtons;
        private Button _btnCancel;
        private Button _btnOK;
    }
}
