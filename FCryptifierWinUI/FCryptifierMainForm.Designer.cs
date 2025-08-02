namespace FCryptifierUI
{
    partial class FCryptifierMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cmdSelectFile = new Button();
            lblInfo = new Label();
            txtPWD = new TextBox();
            lblPWD = new Label();
            SuspendLayout();
            // 
            // cmdSelectFile
            // 
            cmdSelectFile.AllowDrop = true;
            cmdSelectFile.BackColor = Color.DarkSeaGreen;
            cmdSelectFile.Cursor = Cursors.Hand;
            cmdSelectFile.FlatAppearance.BorderColor = Color.DarkSeaGreen;
            cmdSelectFile.FlatStyle = FlatStyle.Flat;
            cmdSelectFile.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmdSelectFile.ForeColor = Color.White;
            cmdSelectFile.Location = new Point(39, 12);
            cmdSelectFile.Name = "cmdSelectFile";
            cmdSelectFile.Size = new Size(284, 53);
            cmdSelectFile.TabIndex = 0;
            cmdSelectFile.Text = "Select or drop file(s) here";
            cmdSelectFile.UseVisualStyleBackColor = false;
            cmdSelectFile.Click += cmdSelectFile_Click;
            cmdSelectFile.DragDrop += cmdSelectFile_DragDrop;
            cmdSelectFile.DragOver += cmdSelectFile_DragOver;
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblInfo.Cursor = Cursors.Hand;
            lblInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblInfo.ForeColor = Color.Gray;
            lblInfo.Location = new Point(333, 59);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(36, 24);
            lblInfo.TabIndex = 2;
            lblInfo.Text = "?";
            lblInfo.TextAlign = ContentAlignment.TopCenter;
            lblInfo.Click += lblInfo_Click;
            // 
            // txtPWD
            // 
            txtPWD.BorderStyle = BorderStyle.None;
            txtPWD.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPWD.ForeColor = Color.Black;
            txtPWD.Location = new Point(94, 21);
            txtPWD.Name = "txtPWD";
            txtPWD.PasswordChar = '•';
            txtPWD.Size = new Size(245, 32);
            txtPWD.TabIndex = 3;
            txtPWD.Visible = false;
            txtPWD.KeyUp += txtPWD_KeyUp;
            // 
            // lblPWD
            // 
            lblPWD.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPWD.ForeColor = Color.Gray;
            lblPWD.Location = new Point(7, 26);
            lblPWD.Name = "lblPWD";
            lblPWD.Size = new Size(134, 24);
            lblPWD.TabIndex = 4;
            lblPWD.Text = "Password";
            lblPWD.Visible = false;
            // 
            // FCryptifierMainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(358, 81);
            Controls.Add(txtPWD);
            Controls.Add(lblPWD);
            Controls.Add(lblInfo);
            Controls.Add(cmdSelectFile);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "FCryptifierMainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FCryptifier";
            Load += FCryptifierMainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button cmdSelectFile;
        private Label lblInfo;
        private TextBox txtPWD;
        private Label lblPWD;
    }
}