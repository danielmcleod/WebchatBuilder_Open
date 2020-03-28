using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard Installation Progress dialog
    /// </summary>
    public partial class ProgressDialog : ManagedForm, IManagedDialog, IProgressDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }

        void ProgressDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            Shell.StartExecute();
        }

        /// <summary>
        /// Called when Shell is changed. It is a good place to initialize the dialog to reflect the MSI session 
        /// (e.g. localize the view).
        /// </summary>
        protected override void OnShellChanged()
        {
            if (MsiRuntime.Session.IsUninstalling())
            {
                dialogText.Text =
                Text = "[ProgressDlgTitleRemoving]";
                description.Text = "[ProgressDlgTextRemoving]";
            }
            else if (MsiRuntime.Session.IsRepairing())
            {
                dialogText.Text =
                Text = "[ProgressDlgTextRepairing]";
                description.Text = "[ProgressDlgTitleRepairing]";
            }
            else if (MsiRuntime.Session.IsInstalling())
            {
                dialogText.Text =
                Text = "[ProgressDlgTitleInstalling]";
                description.Text = "[ProgressDlgTextInstalling]";
            }

            this.Localize();
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        public override MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            switch (messageType)
            {
                case InstallMessage.ActionStart:
                    {

                        try
                        {
                            //messageRecord[0] - is reserved for FormatString value

                            string message = null;

                            bool simple = true;
                            if (simple)
                            {
                                for (int i = messageRecord.FieldCount - 1; i > 0; i--)
                                {
                                    message = messageRecord[i].ToString();
                                }
                            }
                            else
                            {
                                message = messageRecord.FormatString;
                                if (message.IsNotEmpty())
                                {
                                    for (int i = 1; i < messageRecord.FieldCount; i++)
                                    {
                                        message = message.Replace("[" + i + "]", messageRecord[i].ToString());
                                    }
                                }
                                else
                                {
                                    message = messageRecord[messageRecord.FieldCount - 1].ToString();
                                }
                            }


                            if (message.IsNotEmpty())
                                currentAction.Text = currentActionLabel.Text + " " + message;
                        }
                        catch { }
                    }
                    break;
            }
            return MessageResult.OK;
        }

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        public override void OnProgress(int progressPercentage)
        {
            progress.Value = progressPercentage;
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        public override void OnExecuteComplete()
        {
            currentAction.Text = null;
            Shell.GoNext();
        }

        /// <summary>
        /// Handles the Click event of the cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void cancel_Click(object sender, EventArgs e)
        {
            if (Shell.IsDemoMode)
                Shell.GoNext();
            else
                Shell.Cancel();
        }
    }
}