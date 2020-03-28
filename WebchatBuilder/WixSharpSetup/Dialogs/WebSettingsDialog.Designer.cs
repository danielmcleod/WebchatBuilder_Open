using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class WebSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebSettingsDialog));
            this.banner = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.back = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.border1 = new System.Windows.Forms.Panel();
            this.cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.siteName = new System.Windows.Forms.TextBox();
            this.border2 = new System.Windows.Forms.Panel();
            this.appPoolName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.domain = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.allowedDomains = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ipAddr = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.runAsCurrent = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.panel2.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "[WebSettingsDlgDescription]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "[WebSettingsDlgTitle]";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.back);
            this.panel1.Controls.Add(this.next);
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 9;
            // 
            // back
            // 
            this.back.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.Location = new System.Drawing.Point(224, 8);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 33);
            this.back.TabIndex = 0;
            this.back.Text = "[WixUIBack]";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // next
            // 
            this.next.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(305, 8);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 33);
            this.next.TabIndex = 0;
            this.next.Text = "[WixUINext]";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 14;
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(29, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "[SiteNameLabel]";
            // 
            // siteName
            // 
            this.siteName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siteName.Location = new System.Drawing.Point(32, 109);
            this.siteName.Name = "siteName";
            this.siteName.Size = new System.Drawing.Size(210, 29);
            this.siteName.TabIndex = 13;
            // 
            // border2
            // 
            this.border2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border2.Dock = System.Windows.Forms.DockStyle.Top;
            this.border2.Location = new System.Drawing.Point(0, 58);
            this.border2.Name = "border2";
            this.border2.Size = new System.Drawing.Size(494, 1);
            this.border2.TabIndex = 15;
            // 
            // appPoolName
            // 
            this.appPoolName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appPoolName.Location = new System.Drawing.Point(252, 109);
            this.appPoolName.Name = "appPoolName";
            this.appPoolName.Size = new System.Drawing.Size(210, 29);
            this.appPoolName.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(249, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 23);
            this.label4.TabIndex = 16;
            this.label4.Text = "[AppPoolNameLabel]";
            // 
            // domain
            // 
            this.domain.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.domain.Location = new System.Drawing.Point(32, 169);
            this.domain.Name = "domain";
            this.domain.Size = new System.Drawing.Size(210, 29);
            this.domain.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(29, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 23);
            this.label5.TabIndex = 18;
            this.label5.Text = "[WcbDomainLabel]";
            // 
            // allowedDomains
            // 
            this.allowedDomains.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allowedDomains.Location = new System.Drawing.Point(252, 169);
            this.allowedDomains.Name = "allowedDomains";
            this.allowedDomains.Size = new System.Drawing.Size(210, 29);
            this.allowedDomains.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(249, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 23);
            this.label6.TabIndex = 20;
            this.label6.Text = "[AllowedDomainsLabel]";
            // 
            // ipAddr
            // 
            this.ipAddr.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ipAddr.Location = new System.Drawing.Point(32, 229);
            this.ipAddr.Name = "ipAddr";
            this.ipAddr.Size = new System.Drawing.Size(210, 29);
            this.ipAddr.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(29, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 23);
            this.label7.TabIndex = 22;
            this.label7.Text = "[IpAddressLabel]";
            // 
            // port
            // 
            this.port.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.port.Location = new System.Drawing.Point(251, 229);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(210, 29);
            this.port.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(248, 201);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 23);
            this.label8.TabIndex = 24;
            this.label8.Text = "[PortLabel]";
            // 
            // runAsCurrent
            // 
            this.runAsCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runAsCurrent.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runAsCurrent.Location = new System.Drawing.Point(32, 280);
            this.runAsCurrent.Name = "runAsCurrent";
            this.runAsCurrent.Size = new System.Drawing.Size(430, 26);
            this.runAsCurrent.TabIndex = 26;
            this.runAsCurrent.Text = "[RunAsCurrentLabel]";
            this.runAsCurrent.UseVisualStyleBackColor = true;
            this.runAsCurrent.Visible = false;
            // 
            // WebSettingsDialog
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.runAsCurrent);
            this.Controls.Add(this.port);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ipAddr);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.allowedDomains);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.domain);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.appPoolName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.border2);
            this.Controls.Add(this.siteName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WebSettingsDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.WebSettingsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.banner)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox banner;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox siteName;
        private System.Windows.Forms.Panel border1;
        private System.Windows.Forms.Panel border2;
        private System.Windows.Forms.TextBox appPoolName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox domain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox allowedDomains;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ipAddr;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox runAsCurrent;
    }
}