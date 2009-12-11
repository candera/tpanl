using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TPanl
{
    internal delegate void MidiInProc(IntPtr hMidiIn, MidiMessage wMsg, int instance, int param1, int param2);

    internal enum MidiInOpenFlags : uint
    {
        CALLBACK_FUNCTION = 0x00030000,
        //CALLBACK_NULL = 0, 
        //CALLBACK_THREAD = ??,
        //CALLBACK_WINDOW = ??, 
        //MIDI_IO_STATUS = ??,
    }

    internal enum MidiMessage : uint
    {
        MIM_OPEN = 0x3C1,
        MIM_CLOSE = 0x3C2,
        MIM_DATA = 0x3C3,
        MIM_LONGDATA = 0x3C4,
        MIM_ERROR = 0x3C5,
        MIM_LONGERROR = 0x3C6,
        MIM_MOREDATA = 0x3CC,
    }

    internal enum MMSysErr : int
    {
        BASE = 0,
        NOERROR = 0,                    /* no error */
        ERROR = (BASE + 1),  /* unspecified error */
        BADDEVICEID = (BASE + 2),  /* device ID out of range */
        NOTENABLED = (BASE + 3),  /* driver failed enable */
        ALLOCATED = (BASE + 4),  /* device already allocated */
        INVALHANDLE = (BASE + 5),  /* device handle is invalid */
        NODRIVER = (BASE + 6),  /* no device driver present */
        NOMEM = (BASE + 7),  /* memory allocation error */
        NOTSUPPORTED = (BASE + 8),  /* function isn't supported */
        BADERRNUM = (BASE + 9),  /* error value out of range */
        INVALFLAG = (BASE + 10), /* invalid flag passed */
        INVALPARAM = (BASE + 11), /* invalid parameter passed */
        HANDLEBUSY = (BASE + 12), /* handle being used */
        /* simultaneously on another */
        /* thread (eg callback) */
        INVALIDALIAS = (BASE + 13), /* specified alias not found */
        BADDB = (BASE + 14), /* bad registry database */
        KEYNOTFOUND = (BASE + 15), /* registry key not found */
        READERROR = (BASE + 16), /* registry read error */
        WRITEERROR = (BASE + 17), /* registry write error */
        DELETEERROR = (BASE + 18), /* registry delete error */
        VALNOTFOUND = (BASE + 19), /* registry value not found */
        NODRIVERCB = (BASE + 20), /* driver does not call DriverCallback */
        MOREDATA = (BASE + 21), /* more data to be returned */
        LASTERROR = (BASE + 21),/* last error in range */

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MidiInCaps
    {
        internal short wMid;
        internal short wPid;
        internal int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string szPname;
        internal int dwSupport;
    }

    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

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
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
#if IS64BIT
            [FieldOffset(8)]
#else 
            [FieldOffset(4)]
#endif
            public MOUSEINPUT mi;
#if IS64BIT
            [FieldOffset(8)]
#else
            [FieldOffset(4)]
#endif
            public KEYBDINPUT ki;
#if IS64BIT
            [FieldOffset(8)]
#else
            [FieldOffset(4)]
#endif
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
