namespace HelloDesktopAssistant
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtCommandSearchField = new System.Windows.Forms.TextBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tsExit = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCommandSearchField
            // 
            this.txtCommandSearchField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtCommandSearchField.BackColor = System.Drawing.SystemColors.Control;
            this.txtCommandSearchField.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCommandSearchField.Font = new System.Drawing.Font("MS Reference Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommandSearchField.Location = new System.Drawing.Point(43, 12);
            this.txtCommandSearchField.Name = "txtCommandSearchField";
            this.txtCommandSearchField.Size = new System.Drawing.Size(202, 26);
            this.txtCommandSearchField.TabIndex = 0;
            this.txtCommandSearchField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommandSearchField_KeyDown);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "notifyIcon1";
            this.trayIcon.Visible = true;
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsExit});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HelloDesktopAssistant.Properties.Resources.chimp;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(25, 26);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tsExit
            // 
            this.tsExit.Image = global::HelloDesktopAssistant.Properties.Resources.opened33;
            this.tsExit.Name = "tsExit";
            this.tsExit.Size = new System.Drawing.Size(152, 22);
            this.tsExit.Text = "Выход";
            this.tsExit.Click += new System.EventHandler(this.tsExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 51);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtCommandSearchField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MonkeyJob Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.trayMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCommandSearchField;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem tsExit;
    }
}

