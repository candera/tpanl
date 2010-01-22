using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TPanl
{
    public class KeyEvent
    {
        public KeyDirection Direction { get; set; }
        public bool Extended { get; set; }
        public Keys Key { get; set; }
        public ushort? ScanCode { get; set; }

        public override string ToString()
        {
            string result = Key.ToString(); 

            if (ScanCode.HasValue)
            {
                result += string.Format("(0x{0:x2})", ScanCode); 
            }

            if (Extended)
            {
                result += "(ext)"; 
            }

            result += "-" + Direction.ToString();

            return result; 
        }

        internal Win32.INPUT32 ToInput32()
        {
            Win32.INPUT32 input = new Win32.INPUT32();
            input.type = Win32.INPUT_KEYBOARD;

            input.ki.wVk = (ushort)Key;
            uint flags = Win32.KEYEVENTF_SCANCODE;
            if (ScanCode.HasValue)
            {
                input.ki.wScan = ScanCode.Value;
            }
            else
            {
                input.ki.wScan = (ushort)Win32.MapVirtualKey(input.ki.wVk, Win32.MAPVK_VK_TO_VSC);
            }
            input.ki.time = 0;

            if (Direction == KeyDirection.Up)
            {
                flags |= Win32.KEYEVENTF_KEYUP; 
            }

            if (Extended)
            {
                flags |= Win32.KEYEVENTF_EXTENDEDKEY; 
            }

            input.ki.dwFlags = flags; 
            input.ki.dwExtraInfo = Win32.GetMessageExtraInfo();

            return input;
        }

        internal Win32.INPUT64 ToInput64()
        {
            Win32.INPUT64 input = new Win32.INPUT64();
            input.type = Win32.INPUT_KEYBOARD;

            input.ki.wVk = (ushort)Key;
            uint flags = Win32.KEYEVENTF_SCANCODE;
            if (ScanCode.HasValue)
            {
                input.ki.wScan = ScanCode.Value;
            }
            else
            {
                input.ki.wScan = (ushort)Win32.MapVirtualKey(input.ki.wVk, Win32.MAPVK_VK_TO_VSC);
            }
            input.ki.time = 0;

            if (Direction == KeyDirection.Up)
            {
                flags |= Win32.KEYEVENTF_KEYUP;
            }

            if (Extended)
            {
                flags |= Win32.KEYEVENTF_EXTENDEDKEY;
            }

            input.ki.dwFlags = flags;
            input.ki.dwExtraInfo = Win32.GetMessageExtraInfo();

            return input;
        }
    }
}
