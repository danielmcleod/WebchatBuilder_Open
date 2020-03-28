using System;
using System.Drawing;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The Web Settings Dialog
    /// </summary>
    public partial class LicenseKeyDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSettingsDialog"/> class.
        /// </summary>
        public LicenseKeyDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }

        void LicenseKeyDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            licenseKey.Text = MsiRuntime.Session.Property("WCB_LICENSE_KEY");
        }

        void back_Click(object sender, EventArgs e)
        {
            MsiRuntime.Session["WCB_LICENSE_KEY"] = licenseKey.Text;
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            MsiRuntime.Session["WCB_LICENSE_KEY"] = licenseKey.Text;
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

    }
}