using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using HelloDesktopAssistant.Entities;
using Microsoft.Win32;

namespace HelloDesktopAssistant.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private string appName = "MonkeyJob tool";

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var regVal = rkApp.GetValue(appName);
            chkIsWithWindowsStart.Checked = regVal != null && regVal.ToString() == System.Windows.Forms.Application.ExecutablePath;

            HotKeysDatabind();
        }

        private void chkIsWithWindowsStart_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private Dictionary<string, Keys> specialKeys = new Dictionary<string, Keys>()
        {
            {"", Keys.None},
            {"CTRL", Keys.Control},
            {"ALT", Keys.Alt},
            {"SHIFT", Keys.Shift}
        };

        private Dictionary<string, Keys> ordinalKeys = new Dictionary<string, Keys>()
        {
            {"", Keys.None},
            {"Q", Keys.Q},
            {"W", Keys.W},
            {"E", Keys.E},
            {"R", Keys.R},
            {"T", Keys.T},
            {"Y", Keys.Y},
            {"U", Keys.U},
            {"I", Keys.I},
            {"O", Keys.O},
            {"P", Keys.P},
            {"A", Keys.A},
            {"S", Keys.S},
            {"D", Keys.D},
            {"F", Keys.F},
            {"G", Keys.G},
            {"H", Keys.H},
            {"J", Keys.J},
            {"K", Keys.K},
            {"L", Keys.L},
            {"Z", Keys.Z},
            {"X", Keys.X},
            {"C", Keys.C},
            {"V", Keys.V},
            {"B", Keys.B},
            {"N", Keys.N},
            {"M", Keys.M},
            {"F1", Keys.F1},
            {"F2", Keys.F2},
            {"F3", Keys.F3},
            {"F4", Keys.F4},
            {"F5", Keys.F5},
            {"F6", Keys.F6},
            {"F7", Keys.F7},
            {"F8", Keys.F8},
            {"F9", Keys.F9},
            {"F10", Keys.F10},
            {"F11", Keys.F11},
            {"F12", Keys.F12},
        };

        private void HotKeysDatabind()
        {
            cmbKey1.DataSource = new BindingSource(specialKeys, null);
            cmbKey1.DisplayMember = "Key";
            cmbKey1.ValueMember = "Value";

            cmbKey2.DataSource = new BindingSource(specialKeys, null);
            cmbKey2.DisplayMember = "Key";
            cmbKey2.ValueMember = "Value";

            cmbKey3.DataSource = new BindingSource(ordinalKeys, null);
            cmbKey3.DisplayMember = "Key";
            cmbKey3.ValueMember = "Value";

            var openAppHotKeys = App.Instance.AppConf.HotKeys.ProgramOpen;
            var oahkParts = openAppHotKeys.Split('+');
            cmbKey3.SelectedIndex = cmbKey3.FindString(oahkParts.Last()); 
            if(oahkParts.Length==3)
                cmbKey2.SelectedIndex = cmbKey2.FindString(oahkParts[1]); 
            cmbKey1.SelectedIndex = cmbKey1.FindString(oahkParts.First());
        }


        private void SaveConfiguration()
        {
            if (chkIsWithWindowsStart.Checked)
            {
                rkApp.SetValue(appName, Application.ExecutablePath);
            }
            else
            {
                rkApp.DeleteValue(appName, false);
            }

            ApplicationConfiguration updatedAppConf = App.Instance.AppConf;
            updatedAppConf.HotKeys.ProgramOpen = string.Join("+", new List<string>() { cmbKey1.Text, cmbKey2.Text, cmbKey3.Text }.Where(x=>!string.IsNullOrEmpty(x)));
            App.Instance.AppConf = updatedAppConf;

            App.Instance.ReInitHotKeys();
            this.Close();
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        
    }
}
