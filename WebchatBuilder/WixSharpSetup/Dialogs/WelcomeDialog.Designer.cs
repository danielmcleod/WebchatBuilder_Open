using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    partial class WelcomeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeDialog));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.border1 = new System.Windows.Forms.Panel();
            this.next = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.image = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(177, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 208);
            this.label2.TabIndex = 3;
            this.label2.Text = "[WelcomeDlgDescription]";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(176, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 61);
            this.label1.TabIndex = 2;
            this.label1.Text = "[WelcomeDlgTitle]";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.border1);
            this.panel1.Controls.Add(this.next);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 312);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 49);
            this.panel1.TabIndex = 1;
            // 
            // border1
            // 
            this.border1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.border1.Dock = System.Windows.Forms.DockStyle.Top;
            this.border1.Location = new System.Drawing.Point(0, 0);
            this.border1.Name = "border1";
            this.border1.Size = new System.Drawing.Size(494, 1);
            this.border1.TabIndex = 5;
            // 
            // next
            // 
            this.next.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(290, 8);
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
            this.cancel.Location = new System.Drawing.Point(386, 8);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 33);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "[WixUICancel]";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // image
            // 
            this.image.Dock = System.Windows.Forms.DockStyle.Left;
            this.image.Location = new System.Drawing.Point(0, 0);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(156, 312);
            this.image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.image.TabIndex = 0;
            this.image.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.image);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(494, 312);
            this.panel2.TabIndex = 4;
            // 
            // WelcomeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WelcomeDialog";
            this.Text = "[Dialog_Title]";
            this.Load += new System.EventHandler(this.WelcomeDialog_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox image;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel border1;
    }
}