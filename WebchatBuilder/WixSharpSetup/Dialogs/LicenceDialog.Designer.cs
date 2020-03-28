using System.Windows.Forms;

using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class LicenceDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenceDialog));
            this.agreement = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.banner = new System.Windows.Forms.PictureBox();
            this.accepted = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.print = new System.Windows.Forms.Button();
            this.back = new System.Windows.Forms.Button();
            this.border1 = new System.Windows.Forms.Panel();
            this.next = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.border2 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // agreement
            // 
            this.agreement.BackColor = System.Drawing.Color.White;
            this.agreement.ContextMenuStrip = this.contextMenuStrip1;
            this.agreement.Location = new System.Drawing.Point(14, 83);
            this.agreement.MaximumSize = new System.Drawing.Size(462, 167);
            this.agreement.Name = "agreement";
            this.agreement.ReadOnly = true;
            this.agreement.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.agreement.Size = new System.Drawing.Size(462, 167);
            this.agreement.TabIndex = 5;
            this.agreement.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 34);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(139, 30);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.banner);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(494, 58);
            this.panel2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "[AdvancedWelcomeEulaDlgTitle]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "[LicenseAgreementDlgTitle]";
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
            // accepted
            // 
            this.accepted.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accepted.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accepted.Location = new System.Drawing.Point(12, 269);
            this.accepted.Name = "accepted";
            this.accepted.Size = new System.Drawing.Size(464, 26);
            this.accepted.TabIndex = 3;
            this.accepted.Text = "[LicenseAgreementDlgLicenseAcceptedCheckBox]";
            this.accepted.UseVisualStyleBackColor = true;
            this.accepted.CheckedChanged += new System.EventHandler(this.accepted_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.print);
            this.panel1.Controls.Add(this.back);
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.next);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 2;
            // 
            // print
            // 
            this.print.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.print.Location = new System.Drawing.Point(143, 9);
            this.print.Name = "print";
            this.print.Size = new System.Drawing.Size(75, 33);
            this.print.TabIndex = 0;
            this.print.Text = "[WixUIPrint]";
            this.print.UseVisualStyleBackColor = true;
            this.print.Click += new System.EventHandler(this.print_Click);
            // 
            // back
            // 
            this.back.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.Location = new System.Drawing.Point(224, 9);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 33);
            this.back.TabIndex = 0;
            this.back.Text = "[WixUIBack]";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 6;
            // 
            // next
            // 
            this.next.Enabled = false;
            this.next.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(305, 9);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 33);
            this.next.TabIndex = 0;
            this.next.Text = "[WixUINext]";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // cancel
            // 
            this.cancel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.Location = new System.Drawing.Point(401, 9);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 33);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "[WixUICancel]";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // border2
            // 
            this.border2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border2.Dock = System.Windows.Forms.DockStyle.Top;
            this.border2.Location = new System.Drawing.Point(0, 58);
            this.border2.Name = "border2";
            this.border2.Size = new System.Drawing.Size(494, 1);
            this.border2.TabIndex = 7;
            // 
            // LicenceDialog
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.border2);
            this.Controls.Add(this.agreement);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.accepted);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LicenceDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.LicenceDialog_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        Panel panel1;
        Button back;
        Button next;
        CheckBox accepted;
        Panel panel2;
        Label label2;
        Label label1;
        PictureBox banner;
        RichTextBox agreement;
        Button cancel;

        #endregion
        private Button print;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem copyToolStripMenuItem;
        private Panel border1;
        private Panel border2;
    }
}