using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard InstallDir dialog
    /// </summary>
    public partial class InstallDirDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDirDialog"/> class.
        /// </summary>
        public InstallDirDialog()
        {
            InitializeComponent();
        }

        string installDirProperty;

        void InstallDirDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");

            installDirProperty = MsiRuntime.Session.Property("WixSharp_UI_INSTALLDIR");

            string installDirPropertyValue = MsiRuntime.Session.Property(installDirProperty);

            if (installDirPropertyValue.IsEmpty())
            {
                //We are executed before any of the MSI actions are invoked so the INSTALLDIR (if set to absolute path)  
                //is not resolved yet. So we need to do it manually 
                installDir.Text = MsiRuntime.Session.GetDirectoryPath(installDirProperty);

                if (installDir.Text == "ABSOLUTEPATH")
                    installDir.Text = MsiRuntime.Session.Property("INSTALLDIR_ABSOLUTEPATH");
            }
            else
            {
                //INSTALLDIR set either from the command line or by one of the early setup events (e.g. UILoaded)
                installDir.Text = installDirPropertyValue;
            }
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            MsiRuntime.Session[installDirProperty] = installDir.Text;
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void change_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = installDir.Text })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    installDir.Text = dialog.SelectedPath;
                }
            }
        }
    }
}