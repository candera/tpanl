using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace TPanl
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", EntryPoint = "SendInput", SetLastError = true)]
        public static extern uint SendInput32(uint nInputs, INPUT32[] pInputs, int cbSize);

        [DllImport("user32.dll", EntryPoint="SendInput", SetLastError = true)]
        public static extern uint SendInput64(uint nInputs, INPUT64[] pInputs, int cbSize);

        public static uint SendInput(KeyEventSequence keys, bool sendKeysIndividually, Nullable<TimeSpan> pauseBetweenKeys)
        {
            if (IntPtr.Size == 4)
            {
                INPUT32[] inputs = keys.ToInputArray32();
                if (sendKeysIndividually)
                {
                    INPUT32[] chunkedInput = new INPUT32[1];
                    uint total = 0;
                    for (int i = 0; i < inputs.Length; ++i)
                    {
                        chunkedInput[0] = inputs[i];
                        total += Win32.SendInput32(1, chunkedInput, Marshal.SizeOf(typeof(INPUT32)));
                        if (pauseBetweenKeys.HasValue)
                        {
                            Thread.Sleep(pauseBetweenKeys.Value);
                        }
                    }
                    return total; 
                }
                else
                {
                    return Win32.SendInput32((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT32)));
                }
            }
            else if (IntPtr.Size == 8)
            {
                INPUT64[] inputs = keys.ToInputArray64();
                if (sendKeysIndividually)
                {
                    INPUT64[] chunkedInput = new INPUT64[1];
                    uint total = 0;
                    for (int i = 0; i < inputs.Length; ++i)
                    {
                        chunkedInput[0] = inputs[i];
                        total += Win32.SendInput64(1, chunkedInput, Marshal.SizeOf(typeof(INPUT64)));
                        if (pauseBetweenKeys.HasValue)
                        {
                            Thread.Sleep(pauseBetweenKeys.Value);
                        }
                    }
                    return total;
                }
                else
                {
                    return Win32.SendInput64((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT64)));
                }
            }
            else
            {
                throw new NotSupportedException("Can't figure out platform bitness. Pointers have this width: " + IntPtr.Size.ToString());
            }
        }


        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT32
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT64
        {
            [FieldOffset(0)]
            public int type;

            [FieldOffset(8)]
            public MOUSEINPUT mi;
            
            [FieldOffset(8)]
            public KEYBDINPUT ki;
            
            [FieldOffset(8)]
            public HARDWAREINPUT hi;
        }

        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;
        public const uint KEYEVENTF_SCANCODE = 0x0008;
        public const uint XBUTTON1 = 0x0001;
        public const uint XBUTTON2 = 0x0002;
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const uint MOUSEEVENTF_XDOWN = 0x0080;
        public const uint MOUSEEVENTF_XUP = 0x0100;
        public const uint MOUSEEVENTF_WHEEL = 0x0800;
        public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const uint MAPVK_VK_TO_VSC = 0x00;
        public const uint MAPVK_VSC_TO_VK = 0x01;
        public const uint MAPVK_VK_TO_CHAR = 0x02;
        public const uint MAPVK_VSC_TO_VK_EX = 0x03;

    }
}
