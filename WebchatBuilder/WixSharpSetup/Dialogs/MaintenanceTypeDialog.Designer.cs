using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class MaintenanceTypeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaintenanceTypeDialog));
            this.banner = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cancel = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.border1 = new System.Windows.Forms.Panel();
            this.remove = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.border2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).BeginInit();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "[MaintenanceTypeDlgTitle]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(266, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "[MaintenanceTypeDlgDescription]";
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
            this.panel2.TabIndex = 13;
            // 
            // cancel
            // 
            this.cancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.Location = new System.Drawing.Point(401, 8);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 33);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "[WixUICancel]";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // next
            // 
            this.next.Enabled = false;
            this.next.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(305, 8);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 33);
            this.next.TabIndex = 0;
            this.next.Text = "[WixUINext]";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.next);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 12;
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 17;
            // 
            // remove
            // 
            this.remove.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remove.Location = new System.Drawing.Point(29, 92);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(113, 33);
            this.remove.TabIndex = 16;
            this.remove.Text = "[MaintenanceTypeDlgRemoveButton]";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(57, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(420, 41);
            this.label5.TabIndex = 1;
            this.label5.Text = "[MaintenanceTypeDlgRemoveText]";
            // 
            // border2
            // 
            this.border2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border2.Dock = System.Windows.Forms.DockStyle.Top;
            this.border2.Location = new System.Drawing.Point(0, 58);
            this.border2.Name = "border2";
            this.border2.Size = new System.Drawing.Size(494, 1);
            this.border2.TabIndex = 18;
            // 
            // MaintenanceTypeDialog
            // 
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.border2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.remove);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MaintenanceTypeDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.MaintenanceTypeDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.banner)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox banner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button remove;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel border1;
        private System.Windows.Forms.Panel border2;

    }
}