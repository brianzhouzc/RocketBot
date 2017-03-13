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
                ShowWindow(hWnd, 5); // 0 = SM_SHOW
            }
        }

    }
}
