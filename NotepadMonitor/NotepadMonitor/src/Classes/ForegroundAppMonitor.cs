using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NotepadMonitor.Classes
{
    /// <summary>
    /// This is a wrapper around SetWinEventHook function. The class simply signals that foreground window has changed
    /// (through ForegroundAppChanged event). To get actual App info create an instance of WindowInfo
    /// </summary>
    public static class ForegroundAppMonitor
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        static WinEventDelegate _dele;

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;


        public static event EventHandler ForegroundAppChanged;

        private static IntPtr m_hhook;
        public static void StartMonitoring()
        {
            _dele = WinEventProc;
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ForegroundAppChanged?.Invoke(null, null);
        }



        public static void StopMonitoring()
        {
            UnhookWinEvent(m_hhook);
        }

    }


    public class WindowInfo
    {
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint proccess);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint thread);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetWindowTitle(IntPtr handle)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return "";
        }



        /// <summary>
        /// Can accept any window handle. No handle means foreground (currently focused) window
        /// </summary>
        /// <param name="windowHandle"></param>
        public WindowInfo(IntPtr windowHandle = default(IntPtr))
        {
            if (windowHandle == default(IntPtr)) windowHandle = GetForegroundWindow();
            WindowHandle = windowHandle;
            ThreadId = GetWindowThreadProcessId(WindowHandle, out ProcessID);
            KeyboardLayoutCode = GetKeyboardLayout(ThreadId).ToInt32() & 0xFFFF;
            WindowTitle = GetWindowTitle(windowHandle);
            Process = Process.GetProcessById((int)ProcessID);
        }

        public IntPtr WindowHandle;
        public uint ProcessID;
        public uint ThreadId;
        public string WindowTitle;
        public int KeyboardLayoutCode;  //make a new CultureInfo(KeyboardLayoutCode) to get a name
        public Process Process; //All kinds of details here
    }


}
