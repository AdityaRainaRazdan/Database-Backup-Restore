using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Configuration;
using DBRestoreDisplay.Resources;

namespace DBRestoreDisplay
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04D5BDE3F}");
        private static string localConfigPath = Environment.GetEnvironmentVariable("AppData") + @"\IntegrationTool\temp";
        [STAThread]
        static void Main(String[] arg)
        {
           
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    DatabaseRestoreDisplayFrame displayObj = null;
                    String fileName = localConfigPath + @"\DatabaseRestoreState.dat";

                    if (arg.Length > 0 && arg[0].ToUpper() == "AutoRun".ToUpper())
                    {
                        if (File.Exists(fileName))
                        {
                            BinaryFormatter bformatter = new BinaryFormatter();
                            Stream rStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                            RestoreStateObject oldStateObj = (RestoreStateObject)bformatter.Deserialize(rStream);
                            rStream.Close();
                            if (oldStateObj != null)
                            {
                                displayObj = new DatabaseRestoreDisplayFrame(oldStateObj);
                            }
                        }

                    }
                    else
                    {
                        displayObj = new DatabaseRestoreDisplayFrame();
                    }

                    Application.Run(displayObj);
                    mutex.ReleaseMutex();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex+"");
                }
                
            }
            else
            {
               
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.SHOW_RestoreDisplay,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }

    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int SHOW_RestoreDisplay = RegisterWindowMessage("SHOW_RestoreDisplay");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }






    
}
