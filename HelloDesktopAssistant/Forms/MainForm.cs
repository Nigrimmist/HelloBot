using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;

using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using HelloBotCommunication;
using HelloBotCore;
using HelloDesktopAssistant.Forms;

namespace HelloDesktopAssistant
{
    
    public partial class MainForm : Form
    {
        private HelloBot bot = new HelloBot(botCommandPrefix:"");
        

        public MainForm()
        {
            
            InitializeComponent();
            this.pictureBox1.Image = Properties.Resources.chimp;
            this.tsExit.Image = Properties.Resources.opened33;
            this.tsSettings.Image = Properties.Resources.settings49;

            App.Init(openFormHotKeyRaised);

            //Process[] processlist = Process.GetProcesses();

            //foreach (Process process in processlist)
            //{
            //    if (!String.IsNullOrEmpty(process.MainWindowTitle))
            //    {
            //        Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
            //    }
            //}

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            bot.OnErrorOccured += BotOnOnErrorOccured;
            var source = new AutoCompleteStringCollection();
            source.AddRange(bot.GetUserDefinedCommandList().ToArray());
            txtCommandSearchField.AutoCompleteCustomSource = source;
            txtCommandSearchField.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCommandSearchField.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCommandSearchField.Visible = true;

            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
        }

        void openFormHotKeyRaised(object sender, KeyPressedEventArgs e)
        {
            SetForeground();
        }

        private void BotOnOnErrorOccured(Exception exception)
        {
           
        }

        private string copyToBufferPostFix = " в буфер";
        [STAThread]
        private void txtCommandSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string question = txtCommandSearchField.Text;
                bool toBuffer = false;
                if (question.Trim().EndsWith(copyToBufferPostFix, StringComparison.InvariantCultureIgnoreCase))
                {
                    question = question.Substring(0, question.Length - copyToBufferPostFix.Length);
                    toBuffer = true;
                }

                bot.HandleMessage(question, delegate(string answer, AnswerBehaviourType answerType)
                {
                    if (toBuffer)
                    {
                        this.Invoke(new MethodInvoker(() => Clipboard.SetText(answer)));
                    }
                    else
                    {
                        if (answerType == AnswerBehaviourType.Link)
                        {
                            if (answer.StartsWith("http://") || answer.StartsWith("https://"))
                                Process.Start(answer);
                        }
                        else if (answerType == AnswerBehaviourType.Text)
                        {
                            if (!string.IsNullOrEmpty(answer)) MessageBox.Show(answer);
                        }
                    }


                },null);
            }
        }


        #region Go to foreground
        public void SetForeground()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
        }

        #endregion

        private void tsExit_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            System.Windows.Forms.Application.Exit();
        }

        private void tsSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm().Show();
        }

        
    }
}
