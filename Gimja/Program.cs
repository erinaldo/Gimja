using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gimja
{
    static class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

         //<summary>
         //The main entry point for the application.
         //</summary>
        [STAThread]
        static void Main()
        {
            bool mutexCreated = true;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out mutexCreated))
            {
                if (mutexCreated)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    frmLogin loging = new frmLogin();
                    Application.Run(loging);

                    if (loging.UserID != null)
                    {
                        SplashScreenManager.ShowForm(typeof(WaitForm1), true, true);

                        Application.Run(new frmMainScreen() { UserID = loging.UserID , LoginUser = loging.LoggedInUser });
                    }
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            XtraMessageBox.Show("Another instance of " + Application.ProductName + " is already running.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                            break;
                        }
                    }
                }
            }
        }

        ////<summary>
        ////The main entry point for the application.
        ////</summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new frmUser());
        //}
    }
}
