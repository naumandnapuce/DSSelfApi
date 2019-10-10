using log4net;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;


namespace NISA.DSSelfAPI
{
    public partial class frm_main : Form
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(frm_main));
        private NotifyIcon trayIcon;
        // SerialPort1 perdoret vetem nga scale type 1 (aclas scale) Lidhja hapet ne fillim dhe mundesisht qendron e hapur
        #region Events
        public frm_main()
        {
            InitializeComponent();

        }

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StartWebApi();

                this.Closing += Form1_Closing;
                lbl_versioni.Text = Versioni();
                
                timer.Interval = 2000;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();

            }
            catch (Exception ex)
            {
                Log.Warn("Ndodhi nje gabim ne frm_main_load.", ex);
            }


        }
        private void StartWebApi()
        {
            try
            {
                string baseAddress = @"http://localhost:2896" + @"/";
                WebApp.Start<Startup>(url: baseAddress);
                Log.Info("U startua Local app");

            }
            catch (Exception ex)
            {
                Log.Warn("Nuk startohet WebApi lokal", ex);
            }

        }
        void timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
        void Info(object sender, EventArgs e)
        {
            this.Show();
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Enabled = false;

            if (trayIcon == null)
            {
                trayIcon = new NotifyIcon()
                {
                    Icon = new Icon(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\NISA_digiSign.ico"),
                    BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                    BalloonTipText = "NISA",
                    BalloonTipTitle = "Nisa DigiSign",
                    Text = "NISA certifikimi elektronik",

                    ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Info", Info)
            }),
                    Visible = true

                };
                trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Info);
                trayIcon.ShowBalloonTip(500);
            }

            this.Hide();
            e.Cancel = true;

        }

        private string Versioni()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return version;
        }



        #endregion


    }

}
