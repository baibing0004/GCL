using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using GCL.Common;
using GCL.Event;
using GCL.Threading;
using GCL.Threading.Process;
namespace GCL.Project.MyProcessController.MPCForm {
    public partial class ServerPanel : UserControl {
        public ServerPanel() {
            InitializeComponent();
        }

        private string serverName;
        private ABusinessServer server;
        public ServerPanel(BClass bclass, ABusinessServer server, int limitNum)
            : this() {
            this.server = server;
            serverName = server.GetType().Name;
            //bclass.ProcessEvent += new GCL.Threading.ProcessEventHandle(bclass_ProcessEvent1);            
            server.ProcessEvent += new ProcessEventHandle(server_ProcessEvent1);
            this.Enter += new EventHandler(ServerPanel_Enter);
            this.bunStart.Enabled = true;
            this.MAXLISTLINES = limitNum;
        }


        #region 公共方法
        private int MAXLISTLINES = 1000;
        private void AddListContent(string msg) {
            lock (this.list) {
                if (list.Items.Count >= MAXLISTLINES)
                    list.Items.Clear();
                list.Items.Add(Tool.FormatNow() + " " + msg);
            }
        }
        #endregion

        #region 事件订阅处理
        void server_ProcessEvent1(object sender, ProcessEventArg e) {
            this.Invoke(new ProcessEventHandle(server_ProcessEvent), sender, e);
        }

        void server_ProcessEvent(object sender, ProcessEventArg e) {
            if (e.Level == EventLevel.Importent) {
                switch (e.State) {
                    case ProcessState.INIT:
                        this.AddListContent(serverName + "开始初始化!");
                        break;
                    case ProcessState.READY:
                        this.AddListContent(serverName + "准备就绪！");
                        break;
                    case ProcessState.START:
                        this.AddListContent(serverName + "启动！");
                        this.bunStart.Enabled = false;
                        this.bunStop.Enabled = true;
                        break;
                    case ProcessState.STOP:
                        this.AddListContent(serverName + "停止！");
                        this.bunStop.Enabled = false;
                        this.bunStart.Enabled = true;
                        break;
                    case ProcessState.DISPOSE:
                        this.AddListContent(serverName + "清除资源完成运行！");
                        break;
                    case ProcessState.EXCEPTION:
                        this.AddListContent("服务发生致命错误！终止运行错误描述" + e.Exception.Message + "");
                        break;
                }
            } else {
                this.AddListContent(e.GetEventNumber() + "事件," + e.ToStringOfPara(0));
            }
        }

        void ServerPanel_Enter(object sender, EventArgs e) {
            this.list.SelectedIndex = this.list.Items.Count - 1;
        }
        void bclass_ProcessEvent1(object sender, ProcessEventArg e) {
            this.Invoke(new ProcessEventHandle(this.bclass_ProcessEvent), sender, e);
        }
        /*
         * 201事件 业务服务启动
         * 202事件 成功触发报警通知
         * 203事件 报警执行失败
         * 204事件 业务线程终止
         * 205事件 业务线程因为未捕捉错误终止
         */
        void bclass_ProcessEvent(object sender, ProcessEventArg e) {
            if (this.server == e.GetPara(1)) {
                try {
                    switch (e.EventNumber) {
                        case 201:
                            //201事件 业务服务启动
                            this.bunStart.Enabled = false;
                            this.bunStop.Enabled = true;
                            break;
                        case 204:
                            //204事件 业务线程终止
                            this.bunStop.Enabled = false;
                            this.bunStart.Enabled = true;
                            break;
                    }
                    this.AddListContent(string.Format("{0}事件,{1}", e.GetEventNumber(), e.ToStringOfPara(0)));
                } catch (Exception ex) {
                    this.AddListContent(ex.ToString());
                }
            }
        }
        #endregion

        #region 控件方法
        private void bunStart_Click(object sender, EventArgs e) {
            this.server.Start();
        }

        private void bunStop_Click(object sender, EventArgs e) {
            this.server.Stop();
        }
        #endregion
    }
}
