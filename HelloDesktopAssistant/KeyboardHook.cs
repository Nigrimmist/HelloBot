using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelloDesktopAssistant
{
    public sealed class KeyboardHook : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierHookKeys modifierHook = (ModifierHookKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifierHook, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }

        private Window _window = new Window();
        private List<int> _hookIds = new List<int>();
        private object _hookIdsLocker = new object();

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifierHook">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierHookKeys modifierHook, Keys key, int forId)
        {
            lock (_hookIdsLocker)
            {
                if (_hookIds.Contains(forId))
                {
                    UnregisterHotKey(_window.Handle, forId);
                }

                _hookIds.Add(forId);

                // register the hot key.
                if (!RegisterHotKey(_window.Handle, forId, (uint)modifierHook, (uint)key))
                    throw new InvalidOperationException("Couldn’t register the hot key.");
            }
            
        }

        public void UnregisterHotKey(int id)
        {
            lock (_hookIdsLocker)
            {
                if (_hookIds.Contains(id))
                {
                    _hookIds.Remove(id);
                    UnregisterHotKey(_window.Handle, id);
                }

            }
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            foreach (int hookId in _hookIds)
            {
                UnregisterHotKey(_window.Handle, hookId);
            }
            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion

        #region Extensions
        public static ModifierHookKeys ToSpecialKeys(string value)
        {
            var valParts = value.Split('+');
            ModifierHookKeys toReturn;
            Enum.TryParse(valParts.First(), true, out toReturn);
            if (valParts.Length == 3) //two special + one ordinal
            {
                ModifierHookKeys tempModifier;
                if (Enum.TryParse(valParts[1], true, out tempModifier))
                {
                    toReturn |= tempModifier;
                }
            }
            return toReturn;
        }

        public static Keys ToOrdinalKeys(string value)
        {
            Keys toReturn;
            Enum.TryParse(value.Split('+').Last(), true, out toReturn);
            return toReturn;
        } 
        #endregion
    }
}