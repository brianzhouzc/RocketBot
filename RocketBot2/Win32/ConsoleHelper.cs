using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RocketBot2.Win32
{
    internal static class NativeMethods
    {
        [DllImport("Kernel32.dll")]
        internal static extern bool AttachConsole(int processId);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Allocates a new console for current process.
        /// </summary>
        [DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();

        /// <summary>
        /// Frees the console.
        /// </summary>
        [DllImport("kernel32.dll")]
        internal static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();


        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        internal static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        //********** add testes
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }

    class ConsoleHelper
    {
        //Definitions For Different Window Placement Constants
        const uint SW_HIDE = 0;
        const uint SW_SHOWNORMAL = 1;
        const uint SW_NORMAL = 1;
        const uint SW_SHOWMINIMIZED = 2;
        const uint SW_SHOWMAXIMIZED = 3;
        const uint SW_MAXIMIZE = 3;
        const uint SW_SHOWNOACTIVATE = 4;
        const uint SW_SHOW = 5;
        const uint SW_MINIMIZE = 6;
        const uint SW_SHOWMINNOACTIVE = 7;
        const uint SW_SHOWNA = 8;
        const uint SW_RESTORE = 9;

        internal const uint SC_CLOSE = 0xF060;
        internal const uint MF_GRAYED = 0x00000001;
        internal const uint MF_BYCOMMAND = 0x00000000;

        public static bool AllocConsole()
        {
           return NativeMethods.AllocConsole();
        }

        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp)
        {
            return NativeMethods.SendMessage(hWnd, msg, wp, lp);
        }
        // Find window by Caption
        public static IntPtr FindWindow(string windowName)
        {
            var hWnd = NativeMethods.FindWindow(windowName, null);
            return hWnd;
        }

        public static void HideConsoleWindow()
        {
            //IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            IntPtr hWnd = NativeMethods.GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                NativeMethods.ShowWindow(hWnd, (int)SW_HIDE); // 0 = SW_HIDE
            }
        }

        public static void ShowConsoleWindow()
        {
            IntPtr hWnd = NativeMethods.GetConsoleWindow();

            if (hWnd != IntPtr.Zero)
            {
                IntPtr hSystemMenu = NativeMethods.GetSystemMenu(hWnd, false);
                NativeMethods.EnableMenuItem(hSystemMenu, SC_CLOSE, MF_GRAYED);
                NativeMethods.RemoveMenu(hSystemMenu, SC_CLOSE, MF_BYCOMMAND);

                NativeMethods.ShowWindow(hWnd, (int)SW_SHOW); // 0 = SM_SHOW
            }
        }
    }
}
