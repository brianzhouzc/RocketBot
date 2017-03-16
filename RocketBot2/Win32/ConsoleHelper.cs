using System;
using System.Runtime.InteropServices;

namespace RocketBot2.Win32
{
    class ConsoleHelper
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Allocates a new console for current process.
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();

        /// <summary>
        /// Frees the console.
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        internal const uint SC_CLOSE = 0xF060;
        internal const uint MF_GRAYED = 0x00000001;
        internal const uint MF_BYCOMMAND = 0x00000000;

        public static void HideConsoleWindow()
        {
            //IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            IntPtr hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 0); // 0 = SW_HIDE
            }
        }
        public static void ShowConsoleWindow()
        {
            IntPtr hWnd = GetConsoleWindow();

            if (hWnd != IntPtr.Zero)
            {
                IntPtr hSystemMenu = GetSystemMenu(hWnd, false);
                EnableMenuItem(hSystemMenu, SC_CLOSE, MF_GRAYED);
                RemoveMenu(hSystemMenu, SC_CLOSE, MF_BYCOMMAND);

                ShowWindow(hWnd, 5); // 0 = SM_SHOW
            }
        }

    }
}
