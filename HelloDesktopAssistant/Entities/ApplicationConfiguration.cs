using System;
using System.Collections.Generic;

using System.Text;

using System.Windows.Forms;

namespace HelloDesktopAssistant.Entities
{
    public class ApplicationConfiguration
    {
        public AppConfHotkeys HotKeys { get; set; }
    }

    public class AppConfHotkeys
    {
        public string ProgramOpen { get; set; }
    }
}
