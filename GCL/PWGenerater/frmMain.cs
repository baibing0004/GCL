using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PWGenerater {
    public partial class FrmMain : Form {
        public FrmMain() {
            InitializeComponent();
        }

        private void bunGen_Click(object sender, EventArgs e) {
            if (!PublicClass.Common.Tool.IsEnable(this.txtSource.Text))
                this.txtSource.Text = sourceGenerator.GenerateSource();
            this.txtPwd.Text = passwordGenerator.GeneratePwd(this.txtSource.Text);
        }

        private ISourceGenerator sourceGenerator;
        private IPassWordGenerator passwordGenerator;
        private void FrmMain_Load(object sender, EventArgs e) {
            sourceGenerator = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("PWGenerater", "SourceGenerator") as ISourceGenerator;
            passwordGenerator = PublicClass.Bean.Middler.Middlement.GetObjectByAppName("PWGenerater", "PasswordGenerator") as IPassWordGenerator;
            if (sourceGenerator == null)
                throw new InvalidOperationException("SourceGenerator没有设置");
            if (passwordGenerator == null)
                throw new InvalidOperationException("PasswordGenerator没有设置");
        }

        private void bunCopy_Click(object sender, EventArgs e) {
            if (PublicClass.Common.Tool.IsEnable(this.txtPwd.Text)) {
                Clipboard.SetData("Text", this.txtPwd.Text);
                MessageBox.Show(string.Format("{0}\r\n已经粘贴到内存!", this.txtPwd.Text), "密码生成器", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else
                MessageBox.Show("密码框中的内容无效!", "密码生成器", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void bunReGen_Click(object sender, EventArgs e) {
            this.txtSource.Text = "";
            this.bunGen_Click(sender, e);
        }

        private void txtSource_Click(object sender, EventArgs e) {
            this.txtSource.SelectAll();
        }
    }
}
