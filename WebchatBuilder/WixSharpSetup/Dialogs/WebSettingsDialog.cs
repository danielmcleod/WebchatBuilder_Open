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
    public partial class WebSettingsDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSettingsDialog"/> class.
        /// </summary>
        public WebSettingsDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }

        void WebSettingsDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            siteName.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("IIS_WEBSITENAME")) ? MsiRuntime.Session.Property("IIS_WEBSITENAME") : "WebchatBuilder";
            appPoolName.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("IIS_POOL_NAME")) ? MsiRuntime.Session.Property("IIS_POOL_NAME") : "WebchatBuilder";
            domain.Text = MsiRuntime.Session.Property("WCB_DOMAIN");
            allowedDomains.Text = MsiRuntime.Session.Property("WCB_ALLOWED_DOMAINS");
            ipAddr.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("IIS_WEBSITEIPADDRESS")) ? MsiRuntime.Session.Property("IIS_WEBSITEIPADDRESS") : "*";
            port.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("IIS_WEBSITEPORT")) ? MsiRuntime.Session.Property("IIS_WEBSITEPORT") : "80";
            //runAsCurrent.Checked = MsiRuntime.Session["RUN_POOL_AS_CURRENT"] == "True";
        }

        void back_Click(object sender, EventArgs e)
        {
            SaveSettings();

            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            SaveSettings();

            Shell.GoNext();
        }

        void SaveSettings()
        {
            MsiRuntime.Session["IIS_WEBSITENAME"] = siteName.Text;
            MsiRuntime.Session["IIS_POOL_NAME"] = appPoolName.Text;
            MsiRuntime.Session["WCB_DOMAIN"] = domain.Text;
            MsiRuntime.Session["WCB_ALLOWED_DOMAINS"] = allowedDomains.Text;
            MsiRuntime.Session["IIS_WEBSITEIPADDRESS"] = ipAddr.Text;
            MsiRuntime.Session["IIS_WEBSITEPORT"] = port.Text;
            //MsiRuntime.Session["RUN_POOL_AS_CURRENT"] = runAsCurrent.Checked.ToString();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

    }
}