namespace GCL.Project.MyProcessController.MPCForm {
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
            if(disposing && (components != null)) {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.tpLogAll = new System.Windows.Forms.TabPage();
            this.lstAll = new System.Windows.Forms.ListBox();
            this.tpContral = new System.Windows.Forms.TabPage();
            this.gb = new System.Windows.Forms.GroupBox();
            this.txtSettings = new System.Windows.Forms.TextBox();
            this.gbTest = new System.Windows.Forms.GroupBox();
            this.bunTest = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tcAll = new System.Windows.Forms.TabControl();
            this.tpProcess = new System.Windows.Forms.TabPage();
            this.lstProcess = new System.Windows.Forms.ListBox();
            this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.recTimer = new System.Windows.Forms.Timer(this.components);
            this.waitTimer = new System.Windows.Forms.Timer(this.components);
            this.labRTime = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labSTime = new System.Windows.Forms.Label();
            this.labSTdesc = new System.Windows.Forms.Label();
            this.bunStart = new System.Windows.Forms.Button();
            this.bunStop = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tpLogAll.SuspendLayout();
            this.tpContral.SuspendLayout();
            this.gb.SuspendLayout();
            this.gbTest.SuspendLayout();
            this.tcAll.SuspendLayout();
            this.tpProcess.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tpLogAll
            // 
            this.tpLogAll.Controls.Add(this.lstAll);
            this.tpLogAll.Location = new System.Drawing.Point(4, 25);
            this.tpLogAll.Name = "tpLogAll";
            this.tpLogAll.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogAll.Size = new System.Drawing.Size(591, 391);
            this.tpLogAll.TabIndex = 0;
            this.tpLogAll.Text = "全部日志面板";
            this.tpLogAll.UseVisualStyleBackColor = true;
            // 
            // lstAll
            // 
            this.lstAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAll.FormattingEnabled = true;
            this.lstAll.HorizontalScrollbar = true;
            this.lstAll.ItemHeight = 12;
            this.lstAll.Location = new System.Drawing.Point(3, 3);
            this.lstAll.Name = "lstAll";
            this.lstAll.Size = new System.Drawing.Size(585, 388);
            this.lstAll.TabIndex = 0;
            // 
            // tpContral
            // 
            this.tpContral.Controls.Add(this.gb);
            this.tpContral.Controls.Add(this.gbTest);
            this.tpContral.Location = new System.Drawing.Point(4, 25);
            this.tpContral.Name = "tpContral";
            this.tpContral.Padding = new System.Windows.Forms.Padding(3);
            this.tpContral.Size = new System.Drawing.Size(591, 391);
            this.tpContral.TabIndex = 3;
            this.tpContral.Text = "管理设置面板";
            this.tpContral.UseVisualStyleBackColor = true;
            // 
            // gb
            // 
            this.gb.Controls.Add(this.txtSettings);
            this.gb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb.Location = new System.Drawing.Point(3, 3);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(585, 313);
            this.gb.TabIndex = 2;
            this.gb.TabStop = false;
            this.gb.Text = "程序设置";
            // 
            // txtSettings
            // 
            this.txtSettings.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtSettings.BackColor = System.Drawing.SystemColors.Control;
            this.txtSettings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSettings.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSettings.Location = new System.Drawing.Point(3, 17);
            this.txtSettings.Multiline = true;
            this.txtSettings.Name = "txtSettings";
            this.txtSettings.ReadOnly = true;
            this.txtSettings.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtSettings.Size = new System.Drawing.Size(579, 293);
            this.txtSettings.TabIndex = 0;
            // 
            // gbTest
            // 
            this.gbTest.Controls.Add(this.bunTest);
            this.gbTest.Controls.Add(this.txtInfo);
            this.gbTest.Controls.Add(this.label2);
            this.gbTest.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbTest.Location = new System.Drawing.Point(3, 316);
            this.gbTest.Name = "gbTest";
            this.gbTest.Size = new System.Drawing.Size(585, 72);
            this.gbTest.TabIndex = 0;
            this.gbTest.TabStop = false;
            this.gbTest.Text = "测试警报";
            // 
            // bunTest
            // 
            this.bunTest.Location = new System.Drawing.Point(440, 38);
            this.bunTest.Name = "bunTest";
            this.bunTest.Size = new System.Drawing.Size(75, 23);
            this.bunTest.TabIndex = 4;
            this.bunTest.Text = "测试";
            this.bunTest.UseVisualStyleBackColor = true;
            this.bunTest.Click += new System.EventHandler(this.bunTest_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(78, 20);
            this.txtInfo.MaxLength = 70;
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(356, 41);
            this.txtInfo.TabIndex = 3;
            this.txtInfo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInfo_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "文本内容：";
            // 
            // tcAll
            // 
            this.tcAll.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcAll.Controls.Add(this.tpContral);
            this.tcAll.Controls.Add(this.tpLogAll);
            this.tcAll.Controls.Add(this.tpProcess);
            this.tcAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcAll.Location = new System.Drawing.Point(10, 10);
            this.tcAll.Name = "tcAll";
            this.tcAll.Padding = new System.Drawing.Point(0, 0);
            this.tcAll.SelectedIndex = 0;
            this.tcAll.Size = new System.Drawing.Size(599, 420);
            this.tcAll.TabIndex = 0;
            // 
            // tpProcess
            // 
            this.tpProcess.Controls.Add(this.lstProcess);
            this.tpProcess.Location = new System.Drawing.Point(4, 25);
            this.tpProcess.Name = "tpProcess";
            this.tpProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tpProcess.Size = new System.Drawing.Size(591, 391);
            this.tpProcess.TabIndex = 5;
            this.tpProcess.Text = "程序日志面板";
            this.tpProcess.UseVisualStyleBackColor = true;
            // 
            // lstProcess
            // 
            this.lstProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProcess.FormattingEnabled = true;
            this.lstProcess.HorizontalScrollbar = true;
            this.lstProcess.ItemHeight = 12;
            this.lstProcess.Location = new System.Drawing.Point(3, 3);
            this.lstProcess.Name = "lstProcess";
            this.lstProcess.Size = new System.Drawing.Size(585, 388);
            this.lstProcess.TabIndex = 0;
            // 
            // niMain
            // 
            this.niMain.Icon = ((System.Drawing.Icon)(resources.GetObject("niMain.Icon")));
            this.niMain.Text = "SPM框架";
            this.niMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niMain_MouseDoubleClick);
            // 
            // recTimer
            // 
            this.recTimer.Interval = 500;
            this.recTimer.Tick += new System.EventHandler(this.recTimer_Tick);
            // 
            // waitTimer
            // 
            this.waitTimer.Enabled = true;
            this.waitTimer.Tick += new System.EventHandler(this.waitTimer_Tick);
            // 
            // labRTime
            // 
            this.labRTime.AutoSize = true;
            this.labRTime.Location = new System.Drawing.Point(311, 15);
            this.labRTime.Name = "labRTime";
            this.labRTime.Size = new System.Drawing.Size(0, 12);
            this.labRTime.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(228, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "已运行时间：";
            // 
            // labSTime
            // 
            this.labSTime.AutoSize = true;
            this.labSTime.Location = new System.Drawing.Point(79, 15);
            this.labSTime.Name = "labSTime";
            this.labSTime.Size = new System.Drawing.Size(0, 12);
            this.labSTime.TabIndex = 12;
            // 
            // labSTdesc
            // 
            this.labSTdesc.AutoSize = true;
            this.labSTdesc.Location = new System.Drawing.Point(8, 15);
            this.labSTdesc.Name = "labSTdesc";
            this.labSTdesc.Size = new System.Drawing.Size(65, 12);
            this.labSTdesc.TabIndex = 11;
            this.labSTdesc.Text = "启动时间：";
            // 
            // bunStart
            // 
            this.bunStart.Enabled = false;
            this.bunStart.Location = new System.Drawing.Point(436, 10);
            this.bunStart.Name = "bunStart";
            this.bunStart.Size = new System.Drawing.Size(75, 23);
            this.bunStart.TabIndex = 1;
            this.bunStart.Text = "启动";
            this.bunStart.UseVisualStyleBackColor = true;
            this.bunStart.Click += new System.EventHandler(this.bunStart_Click);
            // 
            // bunStop
            // 
            this.bunStop.Enabled = false;
            this.bunStop.Location = new System.Drawing.Point(517, 10);
            this.bunStop.Name = "bunStop";
            this.bunStop.Size = new System.Drawing.Size(75, 23);
            this.bunStop.TabIndex = 0;
            this.bunStop.Text = "停止";
            this.bunStop.UseVisualStyleBackColor = true;
            this.bunStop.Click += new System.EventHandler(this.bunStop_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labRTime);
            this.panel2.Controls.Add(this.bunStart);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.bunStop);
            this.panel2.Controls.Add(this.labSTime);
            this.panel2.Controls.Add(this.labSTdesc);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(10, 436);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(599, 45);
            this.panel2.TabIndex = 3;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 491);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tcAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MPC框架";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.SizeChanged += new System.EventHandler(this.FrmMain_SizeChanged);
            this.tpLogAll.ResumeLayout(false);
            this.tpContral.ResumeLayout(false);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.gbTest.ResumeLayout(false);
            this.gbTest.PerformLayout();
            this.tcAll.ResumeLayout(false);
            this.tpProcess.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tpLogAll;
        private System.Windows.Forms.TabPage tpContral;
        private System.Windows.Forms.TabControl tcAll;
        private System.Windows.Forms.ListBox lstAll;
        private System.Windows.Forms.GroupBox gbTest;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bunTest;
        private System.Windows.Forms.GroupBox gb;
        private System.Windows.Forms.TabPage tpProcess;
        private System.Windows.Forms.ListBox lstProcess;
        private System.Windows.Forms.NotifyIcon niMain;
        private System.Windows.Forms.Timer recTimer;
        private System.Windows.Forms.Timer waitTimer;
        private System.Windows.Forms.Label labRTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labSTime;
        private System.Windows.Forms.Label labSTdesc;
        private System.Windows.Forms.Button bunStart;
        private System.Windows.Forms.Button bunStop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtSettings;
    }
}

