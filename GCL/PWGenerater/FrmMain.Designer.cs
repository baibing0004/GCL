namespace PWGenerater {
    partial class FrmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bunGen = new System.Windows.Forms.Button();
            this.bunCopy = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bunReGen = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(41, 3);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(365, 20);
            this.txtSource.TabIndex = 0;
            this.txtSource.Click += new System.EventHandler(this.txtSource_Click);
            // 
            // txtPwd
            // 
            this.txtPwd.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtPwd.Location = new System.Drawing.Point(41, 29);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.ReadOnly = true;
            this.txtPwd.Size = new System.Drawing.Size(365, 20);
            this.txtPwd.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "源码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "密码";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSource);
            this.panel1.Controls.Add(this.txtPwd);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 86);
            this.panel1.TabIndex = 6;
            // 
            // bunGen
            // 
            this.bunGen.Dock = System.Windows.Forms.DockStyle.Left;
            this.bunGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bunGen.Location = new System.Drawing.Point(0, 0);
            this.bunGen.Name = "bunGen";
            this.bunGen.Size = new System.Drawing.Size(236, 25);
            this.bunGen.TabIndex = 7;
            this.bunGen.Text = "生成";
            this.bunGen.UseVisualStyleBackColor = true;
            this.bunGen.Click += new System.EventHandler(this.bunGen_Click);
            // 
            // bunCopy
            // 
            this.bunCopy.Dock = System.Windows.Forms.DockStyle.Right;
            this.bunCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bunCopy.Location = new System.Drawing.Point(416, 0);
            this.bunCopy.Name = "bunCopy";
            this.bunCopy.Size = new System.Drawing.Size(43, 61);
            this.bunCopy.TabIndex = 8;
            this.bunCopy.Text = "复制";
            this.bunCopy.UseVisualStyleBackColor = true;
            this.bunCopy.Click += new System.EventHandler(this.bunCopy_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bunGen);
            this.panel2.Controls.Add(this.bunReGen);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 61);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(459, 25);
            this.panel2.TabIndex = 9;
            // 
            // bunReGen
            // 
            this.bunReGen.Dock = System.Windows.Forms.DockStyle.Right;
            this.bunReGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bunReGen.Location = new System.Drawing.Point(242, 0);
            this.bunReGen.Name = "bunReGen";
            this.bunReGen.Size = new System.Drawing.Size(217, 25);
            this.bunReGen.TabIndex = 9;
            this.bunReGen.Text = "重新生成";
            this.bunReGen.UseVisualStyleBackColor = true;
            this.bunReGen.Click += new System.EventHandler(this.bunReGen_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(459, 86);
            this.Controls.Add(this.bunCopy);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "密码生成器";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bunGen;
        private System.Windows.Forms.Button bunCopy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bunReGen;
    }
}