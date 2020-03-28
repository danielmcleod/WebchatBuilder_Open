using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class ProgressDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.progress = new System.Windows.Forms.ProgressBar();
            this.currentAction = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dialogText = new System.Windows.Forms.Label();
            this.banner = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.back = new System.Windows.Forms.Button();
            this.border1 = new System.Windows.Forms.Panel();
            this.next = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.description = new System.Windows.Forms.Label();
            this.currentActionLabel = new System.Windows.Forms.Label();
            this.border2 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(32, 162);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(434, 13);
            this.progress.Step = 1;
            this.progress.TabIndex = 20;
            // 
            // currentAction
            // 
            this.currentAction.AutoSize = true;
            this.currentAction.Location = new System.Drawing.Point(34, 137);
            this.currentAction.Name = "currentAction";
            this.currentAction.Size = new System.Drawing.Size(0, 20);
            this.currentAction.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.dialogText);
            this.panel2.Controls.Add(this.banner);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(494, 58);
            this.panel2.TabIndex = 15;
            // 
            // dialogText
            // 
            this.dialogText.AutoSize = true;
            this.dialogText.BackColor = System.Drawing.Color.White;
            this.dialogText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dialogText.Location = new System.Drawing.Point(11, 22);
            this.dialogText.Name = "dialogText";
            this.dialogText.Size = new System.Drawing.Size(245, 25);
            this.dialogText.TabIndex = 1;
            this.dialogText.Text = "[ProgressDlgTitleInstalling]";
            // 
            // banner
            // 
            this.banner.BackColor = System.Drawing.Color.White;
            this.banner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.banner.Location = new System.Drawing.Point(0, 0);
            this.banner.Name = "banner";
            this.banner.Size = new System.Drawing.Size(494, 58);
            this.banner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.banner.TabIndex = 0;
            this.banner.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.back);
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.next);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 14;
            // 
            // back
            // 
            this.back.Enabled = false;
            this.back.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.Location = new System.Drawing.Point(224, 8);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 33);
            this.back.TabIndex = 0;
            this.back.Text = "[WixUIBack]";
            this.back.UseVisualStyleBackColor = true;
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 21;
            // 
            // next
            // 
            this.next.Enabled = false;
            this.next.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(305, 8);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 33);
            this.next.TabIndex = 0;
            this.next.Text = "[WixUINext]";
            this.next.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            this.cancel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.Location = new System.Drawing.Point(401, 8);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 33);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "[WixUICancel]";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // description
            // 
            this.description.AutoSize = true;
            this.description.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.description.Location = new System.Drawing.Point(29, 83);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(194, 21);
            this.description.TabIndex = 16;
            this.description.Text = "[ProgressDlgTextInstalling]";
            // 
            // currentActionLabel
            // 
            this.currentActionLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentActionLabel.Location = new System.Drawing.Point(29, 124);
            this.currentActionLabel.Name = "currentActionLabel";
            this.currentActionLabel.Size = new System.Drawing.Size(132, 23);
            this.currentActionLabel.TabIndex = 19;
            this.currentActionLabel.Text = "[ProgressDlgStatusLabel]";
            this.currentActionLabel.Visible = false;
            // 
            // border2
            // 
            this.border2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border2.Dock = System.Windows.Forms.DockStyle.Top;
            this.border2.Location = new System.Drawing.Point(0, 58);
            this.border2.Name = "border2";
            this.border2.Size = new System.Drawing.Size(494, 1);
            this.border2.TabIndex = 22;
            // 
            // ProgressDialog
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.border2);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.currentAction);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.description);
            this.Controls.Add(this.currentActionLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgressDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox banner;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label dialogText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label description;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label currentAction;
        private System.Windows.Forms.Label currentActionLabel;
        private System.Windows.Forms.Panel border1;
        private System.Windows.Forms.Panel border2;
    }
}