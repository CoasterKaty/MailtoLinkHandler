namespace MailtoHandler
{
    partial class frmPick
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.rad365 = new System.Windows.Forms.RadioButton();
            this.rad2016 = new System.Windows.Forms.RadioButton();
            this.label365 = new System.Windows.Forms.Label();
            this.labelClient = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkDontAsk = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(233, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Please select how you wish to open e-mail links:";
            // 
            // rad365
            // 
            this.rad365.AutoSize = true;
            this.rad365.Checked = true;
            this.rad365.Location = new System.Drawing.Point(15, 10);
            this.rad365.Name = "rad365";
            this.rad365.Size = new System.Drawing.Size(83, 17);
            this.rad365.TabIndex = 1;
            this.rad365.TabStop = true;
            this.rad365.Text = "Outlook 365";
            this.rad365.UseVisualStyleBackColor = true;
            // 
            // rad2016
            // 
            this.rad2016.AutoSize = true;
            this.rad2016.Location = new System.Drawing.Point(15, 59);
            this.rad2016.Name = "rad2016";
            this.rad2016.Size = new System.Drawing.Size(91, 17);
            this.rad2016.TabIndex = 2;
            this.rad2016.Text = "Outlook Client";
            this.rad2016.UseVisualStyleBackColor = true;
            // 
            // label365
            // 
            this.label365.Location = new System.Drawing.Point(31, 28);
            this.label365.Name = "label365";
            this.label365.Size = new System.Drawing.Size(309, 28);
            this.label365.TabIndex = 3;
            this.label365.Text = "For most people, or if you are not sure, select this option";
            this.label365.Click += new System.EventHandler(this.label2_Click);
            // 
            // labelClient
            // 
            this.labelClient.Location = new System.Drawing.Point(31, 79);
            this.labelClient.Name = "labelClient";
            this.labelClient.Size = new System.Drawing.Size(309, 32);
            this.labelClient.TabIndex = 4;
            this.labelClient.Text = "For some people in fixed offices or with multiple mailboxes";
            this.labelClient.Click += new System.EventHandler(this.label3_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(140, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rad365);
            this.panel1.Controls.Add(this.rad2016);
            this.panel1.Controls.Add(this.labelClient);
            this.panel1.Controls.Add(this.label365);
            this.panel1.Location = new System.Drawing.Point(1, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(353, 120);
            this.panel1.TabIndex = 6;
            // 
            // chkDontAsk
            // 
            this.chkDontAsk.AutoSize = true;
            this.chkDontAsk.Checked = true;
            this.chkDontAsk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDontAsk.Location = new System.Drawing.Point(16, 152);
            this.chkDontAsk.Name = "chkDontAsk";
            this.chkDontAsk.Size = new System.Drawing.Size(100, 17);
            this.chkDontAsk.TabIndex = 7;
            this.chkDontAsk.Text = "Don\'t ask again";
            this.chkDontAsk.UseVisualStyleBackColor = true;
            // 
            // frmPick
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 202);
            this.ControlBox = false;
            this.Controls.Add(this.chkDontAsk);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPick";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Mail Link";
            this.Load += new System.EventHandler(this.frmPick_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.RadioButton rad365;
        public System.Windows.Forms.RadioButton rad2016;
        public System.Windows.Forms.CheckBox chkDontAsk;
        public System.Windows.Forms.Label label365;
        public System.Windows.Forms.Label labelClient;
        public System.Windows.Forms.Label labelTitle;
    }
}

