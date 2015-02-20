using System;
using System.Collections.Generic;

using System.Text;

using System.Windows.Forms;

namespace HelloDesktopAssistant
{
    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierHookKeys _modifierHook;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierHookKeys modifierHook, Keys key)
        {
            _modifierHook = modifierHook;
            _key = key;
        }

        public ModifierHookKeys ModifierHook
        {
            get { return _modifierHook; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierHookKeys : uint
    {
        Alt = 1,
        CTRL = 2,
        Shift = 4,
        Win = 8
    }
}
