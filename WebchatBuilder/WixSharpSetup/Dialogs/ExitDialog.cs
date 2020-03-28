using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard Exit dialog
    /// </summary>
    public partial class ExitDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitDialog"/> class.
        /// </summary>
        public ExitDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }

        void ExitDialog_Load(object sender, System.EventArgs e)
        {
            image.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Dialog");
            if (Shell.UserInterrupted)
            {
                description.Text = "[UserExitDescription]";
                this.Localize();
            }
            if (Shell.ErrorDetected)
            {
                description.Text = "[ErrorExitDescription]";
                this.Localize();
            }
        }

        void finish_Click(object sender, System.EventArgs e)
        {
            Shell.Exit();
        }

        void viewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string wixSharpDir = Path.Combine(Path.GetTempPath(), @"WixSharp");
                if (!Directory.Exists(wixSharpDir))
                    Directory.CreateDirectory(wixSharpDir);

                string logFile = Path.Combine(wixSharpDir, MsiRuntime.ProductName + ".log");
                System.IO.File.WriteAllText(logFile, Shell.Log);
                Process.Start(logFile);
            }
            catch { }
        }
    }
}
