using NISA.DSSelfAPI.Properties;
using log4net;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NISA.DSSelfAPI
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static string appGuid = "bbc73f34-0873-44bc-bf2b-68d2bf22164b";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        private static Mutex mutex = null;
        [STAThread]
        static void Main()
        {


            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                using (Mutex mutex = new Mutex(false, @"Global\" + appGuid))
                {
                    if (!mutex.WaitOne(0, false))
                    {
                        MessageBox.Show("Aplikimi eshte hapur nje here!");
                        return;
                    }

                    Application.Run(new frm_main());
                }
            }
            catch (Exception ex)
            {

                Log.Error("Ndodhi nje gabim i bere handle globalisht", ex);
            }

        }
    }

}
