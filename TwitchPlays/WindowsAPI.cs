using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// 
/// </summary>
public class WindowsAPI
{
    /// <summary>
    /// 
    /// </summary>
    public const uint WM_KEYDOWN = 0x100;

    /// <summary>
    /// 
    /// </summary>
    public const uint WM_KEYUP = 0x101;

    /// <summary>
    /// 
    /// </summary>
    public const uint WM_LBUTTONDOWN = 0x201;

    /// <summary>
    /// 
    /// </summary>
    public const uint WM_LBUTTONUP = 0x202;

    public const uint WM_CHAR = 0x102;

    /// <summary>
    /// 
    /// </summary>
    public const int MK_LBUTTON = 0x01;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_RETURN = 0x0d;

    public const int VK_ESCAPE = 0x1b;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_TAB = 0x09;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_LEFT = 0x25;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_UP = 0x26;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_RIGHT = 0x27;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_DOWN = 0x28;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_F5 = 0x74;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_F6 = 0x75;

    /// <summary>
    /// 
    /// </summary>
    public const int VK_F7 = 0x76;

    /// <summary>
    /// The GetForegroundWindow function returns a handle to the foreground window.
    /// </summary>
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      [Out()] byte[] lpBuffer,
      int dwSize,
      out int lpNumberOfBytesRead
     );

    public static void SwitchWindow(IntPtr windowHandle)
    {
        if (GetForegroundWindow() == windowHandle)
            return;

        IntPtr foregroundWindowHandle = GetForegroundWindow();
        uint currentThreadId = GetCurrentThreadId();
        uint temp;
        uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindowHandle, out temp);
        AttachThreadInput(currentThreadId, foregroundThreadId, true);
        SetForegroundWindow(windowHandle);
        AttachThreadInput(currentThreadId, foregroundThreadId, false);

        while (GetForegroundWindow() != windowHandle)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hwndParent"></param>
    /// <param name="hwndChildAfter"></param>
    /// <param name="lpszClass"></param>
    /// <param name="lpszWindow"></param>
    /// <returns></returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
    public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern byte VkKeyScan(char ch);

    [DllImport("user32.dll")]
    public static extern uint MapVirtualKey(uint uCode, uint uMapType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IntPtr FindWindow(string name)
    {
        Process[] procs = Process.GetProcesses();

        foreach (Process proc in procs)
        {
            if (proc.MainWindowTitle == name)
            {
                return proc.MainWindowHandle;
            }
        }

        return IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="low"></param>
    /// <param name="high"></param>
    /// <returns></returns>
    public static int MakeLong(int low, int high)
    {
        return (high << 16) | (low & 0xffff);
    }

    [DllImport("User32.dll")]
    public static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] input, int structSize);

    [DllImport("user32.dll")]
    public static extern IntPtr GetMessageExtraInfo();

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
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT
{
    int dx;
    int dy;
    uint mouseData;
    uint dwFlags;
    uint time;
    IntPtr dwExtraInfo;
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
    uint uMsg;
    ushort wParamL;
    ushort wParamH;
}

[StructLayout(LayoutKind.Explicit)]
public struct INPUT
{
    [FieldOffset(0)]
    public int type;
    [FieldOffset(4)] //*
    public MOUSEINPUT mi;
    [FieldOffset(4)] //*
    public KEYBDINPUT ki;
    [FieldOffset(4)] //*
    public HARDWAREINPUT hi;
}