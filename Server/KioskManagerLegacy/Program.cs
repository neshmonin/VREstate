using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace KioskManager
{
    static public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //Get the running instance.
            Process instance = RunningInstance();
            if (instance == null)
            {
                //There isn't another instance, show our form.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new KioskManager());
            }
            else
            {
                //There is another instance of this process.
                //Make sure the window is not minimized or maximized
                ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);

                //Set the real intance to foreground window
                SetForegroundWindow(instance.MainWindowHandle);
            }
        }

        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //Loop through the running processes in with the same name
            foreach (Process process in processes)
            {
                //Ignore the current process
                if (process.Id != current.Id)
                {
                    //Make sure that the process is running from the exe file.
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\")
                                   == current.MainModule.FileName)
                    {
                        //Return the other process instance.
                        return process;
                    }
                }
            }

            //No other instance was found, return null.
            return null;
        }


        [DllImport("User32.dll")]

        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1;
    }

}
