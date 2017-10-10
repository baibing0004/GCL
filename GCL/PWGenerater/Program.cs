using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PWGenerater {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            MessageBox.Show(string.Format("因严重错误，程序关闭！\r\n{0}", e.ToString()), "密码生成器", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
