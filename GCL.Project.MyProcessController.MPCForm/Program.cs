using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;
using GCL.Common;

namespace GCL.Project.MyProcessController.MPCForm {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.Run(new FrmMain());
        }
    }
}