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
    public partial class CicSettingsDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSettingsDialog"/> class.
        /// </summary>
        public CicSettingsDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }

        void CicSettingsDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            cicUser.Text = MsiRuntime.Session.Property("CIC_USER");
            cicPass.Text = MsiRuntime.Session.Property("CIC_PASS");
            cicPrimary.Text = MsiRuntime.Session.Property("CIC_PRIMARY");
            cicSecondary.Text = MsiRuntime.Session.Property("CIC_SECONDARY");
            cicPort.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("CIC_PORT")) ? MsiRuntime.Session.Property("CIC_PORT") : "8114";
            cicProtocol.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("CIC_PROTOCOL")) ? MsiRuntime.Session.Property("CIC_PROTOCOL") : "http";
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
            MsiRuntime.Session["CIC_USER"] = cicUser.Text;
            MsiRuntime.Session["CIC_PASS"] = cicPass.Text;
            MsiRuntime.Session["CIC_PRIMARY"] = cicPrimary.Text;
            MsiRuntime.Session["CIC_SECONDARY"] = cicSecondary.Text;
            MsiRuntime.Session["CIC_PORT"] = cicPort.Text;
            MsiRuntime.Session["CIC_PROTOCOL"] = cicProtocol.Text;
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

    }
}