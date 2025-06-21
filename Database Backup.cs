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

namespace DBBackup.Resources
{
    
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        private static string configurationPath = ConfigurationManager.AppSettings["CONFIGURATIONS"];
        [STAThread]
        static void Main(String[] arg)
        {
           
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DatabaseBackupDisplayFrame displayObj = null;
                String fileName = configurationPath + @"\temp\DatabaseBackupState.dat";

                if(arg.Length > 0 && arg[0].ToUpper() == "AutoRun".ToUpper())
                {
                    if(File.Exists(fileName))
                    {
                        BinaryFormatter bformatter = new BinaryFormatter();
                        Stream rStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                        BackupStateObject oldStateObj = (BackupStateObject)bformatter.Deserialize(rStream);
                        rStream.Close();
                        if (oldStateObj!=null)
                        {
                            displayObj = new DatabaseBackupDisplayFrame(oldStateObj);
                        }
                    }
                    
                }
                else{
                    displayObj = new DatabaseBackupDisplayFrame();
                }
                
                Application.Run(displayObj);
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.SHOW_BackupDisplay,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }

    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int SHOW_BackupDisplay = RegisterWindowMessage("SHOW_BackupDisplay");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }

}
