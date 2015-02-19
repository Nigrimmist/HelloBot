using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using HelloBotCommunication;
using HelloBotCore;

namespace HelloDesktopAssistant
{
    public partial class MainForm : Form
    {
        private static HelloBot bot = new HelloBot(botCommandPrefix:"");
        KeyboardHook hook = new KeyboardHook();
        public MainForm()
        {
            InitializeComponent();

            // register the event that is fired after the key press.
            hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(ModifierHookKeys.Control | ModifierHookKeys.Alt,
                Keys.O);
            
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

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            SetForeground();
        }

        private void BotOnOnErrorOccured(Exception exception)
        {
            MessageBox.Show("Ошибка");
        }

        private void txtCommandSearchField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bot.HandleMessage(txtCommandSearchField.Text, delegate(string answer,AnswerBehaviourType answerType)
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
            Application.Exit();
        }
    }
}
