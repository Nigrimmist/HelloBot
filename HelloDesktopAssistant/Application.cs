using System;
using System.Collections.Generic;
using System.IO;

using System.Text;

using System.Windows.Forms;
using HelloDesktopAssistant.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HelloDesktopAssistant
{
    /// <summary>
    /// Main Application class for communication between forms including business logic. GOD CLASS! HAHHAHA!
    /// </summary>
    public class App
    {
        private const string _confFileName = "conf.json";
        private static object _appInstanceLock = new object();
        private static App _instance;
        private ApplicationConfiguration _appConf;
        private JsonSerializer _serializer = new JsonSerializer();
        private KeyboardHook _hook = new KeyboardHook();
        private object _hookLock = new object();
        /// <summary>
        /// Collection of event delegates for hotkeys. one delegate for one hotkeytype
        /// </summary>
        private Dictionary<HotKeyType, EventHandler<KeyPressedEventArgs>> _hotKeysHadlers = new Dictionary<HotKeyType, EventHandler<KeyPressedEventArgs>>();
        

        public static App Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_appInstanceLock)
                    {
                        if (_instance==null)
                        {
                            _instance = new App();
                        }
                    }
                }
                return _instance;
            }
            
        }

        public static void Init(EventHandler<KeyPressedEventArgs> mainFormOpenHotKeyRaisedHandler)
        {
            //register open program hotkey using incoming (main form) delegate
            Instance._hotKeysHadlers.Add(HotKeyType.OpenProgram, mainFormOpenHotKeyRaisedHandler);

            Instance.ReInitHotKeys();
        }

        public void ReInitHotKeys()
        {
            var openHotKeys = Instance.AppConf.HotKeys.ProgramOpen;
            Instance.RegisterHotKey(KeyboardHook.ToSpecialKeys(openHotKeys), KeyboardHook.ToOrdinalKeys(openHotKeys), HotKeyType.OpenProgram);
        }

        public App()
        {
            //read config
            if (!File.Exists(_confFileName)) throw new Exception("Config missing");
            var json = File.ReadAllText(_confFileName);
            _appConf = _serializer.Deserialize<ApplicationConfiguration>(new JsonTextReader(new StringReader(json)));
            
        }

        private void RegisterHotKey(ModifierHookKeys modifierHook, Keys key, HotKeyType hkType)
        {
            UnRegisterHotKey(hkType);
            lock (_hookLock)
            {
                _hook.KeyPressed += _hotKeysHadlers[hkType];
                _hook.RegisterHotKey(modifierHook, key, (int) HotKeyType.OpenProgram);
            }
        }

        private void UnRegisterHotKey(HotKeyType hkType)
        {
            lock (_hookLock)
            {
                _hook.KeyPressed -= _hotKeysHadlers[hkType];
                _hook.UnregisterHotKey((int) hkType);
            }
        }

        public ApplicationConfiguration AppConf
        {
            get { return _appConf; }
            set
            {
                StringBuilder sb = new StringBuilder();
                _serializer.Serialize(new JsonTextWriter(new StringWriter(sb)), value);
                File.WriteAllText(_confFileName, sb.ToString());
                _appConf = value;
            }
        }
    }
}
