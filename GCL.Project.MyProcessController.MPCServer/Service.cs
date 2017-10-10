using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace GCL.Project.MyProcessController.MPCServer {
    public partial class Service : ServiceBase {
        public Service() {
            InitializeComponent();
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
        private BClass bClass;

        protected override void OnStart(string[] args) {
            bClass = new BClass();
            bClass.Init();
            bClass.Start();
        }

        protected override void OnStop() {
            bClass.Dispose();
        }
    }
}
