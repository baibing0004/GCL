namespace GCL.Project.MyProcessController.MPCForm {
    partial class ServerPanel {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.bunStop = new System.Windows.Forms.Button();
            this.bunStart = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.list = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bunStop
            // 
            this.bunStop.Enabled = false;
            this.bunStop.Location = new System.Drawing.Point(84, 3);
            this.bunStop.Name = "bunStop";
            this.bunStop.Size = new System.Drawing.Size(75, 25);
            this.bunStop.TabIndex = 0;
            this.bunStop.Text = "Í£Ö¹";
            this.bunStop.UseVisualStyleBackColor = true;
            this.bunStop.Click += new System.EventHandler(this.bunStop_Click);
            // 
            // bunStart
            // 
            this.bunStart.Enabled = false;
            this.bunStart.Location = new System.Drawing.Point(3, 3);
            this.bunStart.Name = "bunStart";
            this.bunStart.Size = new System.Drawing.Size(75, 25);
            this.bunStart.TabIndex = 1;
            this.bunStart.Text = "Æô¶¯";
            this.bunStart.UseVisualStyleBackColor = true;
            this.bunStart.Click += new System.EventHandler(this.bunStart_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 258);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 36);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bunStart);
            this.panel2.Controls.Add(this.bunStop);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(295, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(164, 36);
            this.panel2.TabIndex = 2;
            // 
            // list
            // 
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.FormattingEnabled = true;
            this.list.HorizontalScrollbar = true;
            this.list.Location = new System.Drawing.Point(0, 0);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(459, 251);
            this.list.TabIndex = 3;
            // 
            // ServerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.list);
            this.Controls.Add(this.panel1);
            this.Name = "ServerPanel";
            this.Size = new System.Drawing.Size(459, 294);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bunStop;
        private System.Windows.Forms.Button bunStart;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox list;
        private System.Windows.Forms.Panel panel2;
    }
}
