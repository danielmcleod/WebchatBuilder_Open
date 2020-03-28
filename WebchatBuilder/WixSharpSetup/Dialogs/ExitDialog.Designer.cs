using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class ExitDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExitDialog));
            this.description = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.border1 = new System.Windows.Forms.Panel();
            this.viewLog = new System.Windows.Forms.LinkLabel();
            this.finish = new System.Windows.Forms.Button();
            this.image = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // description
            // 
            this.description.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.description.Location = new System.Drawing.Point(177, 88);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(305, 208);
            this.description.TabIndex = 7;
            this.description.Text = "[ExitDialogDescription]";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(176, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 61);
            this.label1.TabIndex = 6;
            this.label1.Text = "[ExitDialogTitle]";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.viewLog);
            this.panel1.Controls.Add(this.finish);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 5;
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 9;
            // 
            // viewLog
            // 
            this.viewLog.AutoSize = true;
            this.viewLog.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewLog.Location = new System.Drawing.Point(16, 21);
            this.viewLog.Name = "viewLog";
            this.viewLog.Size = new System.Drawing.Size(80, 21);
            this.viewLog.TabIndex = 1;
            this.viewLog.TabStop = true;
            this.viewLog.Text = "[ViewLog]";
            this.viewLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewLog_LinkClicked);
            // 
            // finish
            // 
            this.finish.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finish.Location = new System.Drawing.Point(401, 8);
            this.finish.Name = "finish";
            this.finish.Size = new System.Drawing.Size(75, 33);
            this.finish.TabIndex = 0;
            this.finish.Text = "[WixUIFinish]";
            this.finish.UseVisualStyleBackColor = true;
            this.finish.Click += new System.EventHandler(this.finish_Click);
            // 
            // image
            // 
            this.image.Dock = System.Windows.Forms.DockStyle.Left;
            this.image.Location = new System.Drawing.Point(0, 0);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(156, 312);
            this.image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.image.TabIndex = 4;
            this.image.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.description);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.image);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(494, 312);
            this.panel2.TabIndex = 8;
            // 
            // ExitDialog
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExitDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.ExitDialog_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label description;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button finish;
        private System.Windows.Forms.PictureBox image;
        private System.Windows.Forms.LinkLabel viewLog;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel border1;
    }
}