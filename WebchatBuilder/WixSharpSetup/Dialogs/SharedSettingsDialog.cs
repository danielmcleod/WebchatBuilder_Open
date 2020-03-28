using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The Web Settings Dialog
    /// </summary>
    public partial class SharedSettingsDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSettingsDialog"/> class.
        /// </summary>
        public SharedSettingsDialog()
        {
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            InitializeComponent();
        }

        void SharedSettingsDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            wcbPort.Text = !String.IsNullOrEmpty(MsiRuntime.Session.Property("WCB_PORT")) ? MsiRuntime.Session.Property("WCB_PORT") : "8089";
            connectionString.Text = MsiRuntime.Session.Property("WCB_CONNECTION_STRING");
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
            MsiRuntime.Session["WCB_PORT"] = wcbPort.Text;
            var connString = connectionString.Text;
            connString = Regex.Replace(connString, @"\t|\n|\r", "");
            connString = connString.Replace(";", "|~|");
            MsiRuntime.Session["WCB_CONNECTION_STRING"] = connString;
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

    }
}