using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GCL.Common;
using GCL.Threading;
using GCL.Threading.Process;
using GCL.Collections;
using GCL.Event;

namespace GCL.Project.MyProcessController.MPCForm {
    public partial class FrmMain : Form {

        private BClass bClass;
        public FrmMain() {
            InitializeComponent();
            bClass = new BClass();
            bClass.ProcessEvent += new ProcessEventHandle(bClass_ProcessEvent1);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            MessageBox.Show(string.Format("发生Application_ThreadException未处理异常！:\r\n{0}", e.Exception.ToString()), "MPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            bClass.RecordLog("发生Form的UI线程未处理异常！" + e.Exception.ToString());
            this.Close();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show(string.Format("发生CurrentDomain_UnhandledException未处理异常！:\r\n{0}", e.ExceptionObject.ToString()), "MPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            bClass.RecordLog("发生程序级别未处理异常！" + e.ExceptionObject.ToString());
            bClass.TestAlarm("程序异常关闭!");
            this.Close();
            //自动处理
            //System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);
        }

        #region 进程事件封装
        void bClass_ProcessEvent1(object sender, ProcessEventArg e) {
            //调用contral的方法以这个窗体的权限触发事件处理
            this.Invoke(new ProcessEventHandle(bClass_ProcessEvent), new object[] { sender, e });
        }

        void bClass_ProcessEvent(object sender, ProcessEventArg e) {
            try {
                if (e.Level == EventLevel.Importent) {
                    #region 处理进程级事件
                    switch (e.State) {
                        case ProcessState.INIT:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "开始初始化!");
                            break;
                        case ProcessState.READY:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "准备就绪！");
                            this.bunStart.Enabled = true;
                            break;
                        case ProcessState.START:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "开始启动！");

                            this.labSTdesc.Text = "启动时间:";
                            this.startTime = DateTime.Now;
                            this.labSTime.Text = Tool.FormatDate(startTime);
                            this.recTimer.Start();

                            this.waitTimer.Stop();
                            this.firstRun = false;
                            this.bunStart.Enabled = false;
                            this.bunStop.Enabled = true;
                            break;
                        case ProcessState.STOP:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "停止！");

                            this.labSTdesc.Text = "停止时间:";
                            this.recTimer.Stop();

                            this.firstRun = true;
                            this.waitTimer.Start();

                            this.labSTime.Text = Tool.FormatNow();
                            this.niMain.Text = Properties.Settings.Default.ApplicationName;
                            bClass.RecordLog(string.Format("此次执行处理时间共", this.labRTime.Text));
                            this.bunStart.Enabled = true;
                            this.bunStop.Enabled = false;
                            break;
                        case ProcessState.DISPOSE:
                            waitTimer.Stop();
                            waitTimer.Enabled = false;
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "清除资源完成运行！");
                            this.Close();
                            //Application.ExitThread();
                            break;
                        case ProcessState.EXCEPTION:
                            string desc = "";
                            switch (((AbstractProcess)sender).GetState()) {
                                case ProcessState.INIT:
                                    desc = "初始化";
                                    break;
                                case ProcessState.START:
                                    desc = "启动进程";
                                    break;
                                case ProcessState.STOP:
                                    desc = "关闭进程";
                                    break;
                                case ProcessState.DISPOSE:
                                    desc = "终止进程";
                                    break;
                            }
                            this.AddProcessLog("程序" + desc + "发生致命错误！终止运行错误描述" + e.Exception.Message);
                            this.bunStop_Click(this, new EventArgs());
                            this.firstRun = true;
                            this.waitTimer.Start();
                            break;
                    }
                    #endregion
                } else {
                    #region 处理普通事件

                    /*事件说明：
                 * 101事件 读取配置信息
                 * 102事件 业务服务注册
                 * 103事件 刷新Settings列表
                 * 201事件 业务服务启动
                 * 202事件 成功触发报警通知
                 * 203事件 报警执行失败
                 * 204事件 业务线程终止
                 * 205事件 业务线程因为未捕捉错误终止
                 * 206事件 执行测试命令成功!
                 * 207事件 执行测试命令出错
                 * 208事件 服务撤销
                 * 209事件 服务初始化
                 * 210事件 服务准备就绪
                 */
                    string desc = string.Format("{0}事件,{1}", e.GetEventNumber(), e.ToStringOfPara(0));
                    switch (e.EventNumber) {

                        #region 进程级事件
                        case 101:
                            //101 事件读取配置信息
                            //进程级事件
                            this.AddProcessLog(desc);
                            break;
                        case 102:
                            //102事件 业务服务注册
                            this.AddTabPage((ABusinessServer)e.GetPara(1));
                            this.AddProcessLog(desc);
                            break;
                        case 103:
                            this.txtSettings.Text = e.ToStringOfPara(1);
                            break;
                        #endregion

                        default:
                            if (e.Length > 1 && e.GetPara(1) is ABusinessServer)
                                this.AddListContent(this.lstAll, e.GetPara(1).GetType().Name + "服务 " + desc);
                            else
                                this.AddListContent(this.lstAll, desc);
                            break;
                    }
                    #endregion
                }
            } catch (Exception ex) {
                this.AddProcessLog("程序出现未捕捉错误 " + ex.ToString());
                //this.ShowMessage("程序出现未捕捉错误 " + ex.ToString(), MessageBoxIcon.Stop);
            }
        }
        #endregion

        #region 公共方法
        private void AddTabPage(ABusinessServer server) {
            ServerPanel sp = new ServerPanel(this.bClass, server, MAXLISTLINES);
            sp.Dock = DockStyle.Fill;
            TabPage tp = new TabPage();
            tp.Name = "tp" + server.GetType().Name;
            tp.Text = server.GetType().Name + "控制面板" + (tcAll.TabPages.Count - 2);
            tp.ImageIndex = tcAll.TabPages.Count;
            tp.Controls.Add(sp);
            
            tcAll.TabPages.Add(tp);
        }

        /// <summary>
        /// 主要用于统一显示的格式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string msg) {
            return ShowMessage(msg, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        /// <summary>
        /// 主要用于统一显示的格式
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string msg, MessageBoxIcon icon) {
            return ShowMessage(msg, MessageBoxButtons.OK, icon);
        }
        /// <summary>
        /// 主要用于统一显示的格式
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string msg, MessageBoxButtons buttons, MessageBoxIcon icon) {
            return System.Windows.Forms.MessageBox.Show(this, msg, Properties.Settings.Default.ApplicationName + "", buttons, icon);
        }

        private static int MAXLISTLINES = Properties.Settings.Default.MAXLISTLINES;
        private void AddListContent(System.Windows.Forms.ListBox list, string msg) {
            lock (list) {
                if (list.Items.Count >= MAXLISTLINES)
                    list.Items.Clear();
                list.Items.Add(Tool.FormatNow() + " " + msg);
            }
        }

        /// <summary>
        /// 添加进程日志
        /// </summary>
        /// <param name="msg"></param>
        private void AddProcessLog(string msg) {
            this.AddListContent(this.lstProcess, msg);
            this.AddListContent(this.lstAll, msg);
        }

        #endregion

        #region 控件方法

        private void FrmMain_Load(object sender, EventArgs e) {
            this.bClass.Init();
        }

        /// <summary>
        /// 发送短信测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bunTest_Click(object sender, EventArgs e) {
            try {
                bClass.TestAlarm(this.txtInfo.Text);
                this.ShowMessage("测试信息处理成功！", MessageBoxIcon.Information);
            } catch (Exception ex) {
                this.ShowMessage(ex.Message, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// 设定文本框内的文本为数字与控制符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSendTime_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.KeyChar = new char();
        }

        /// <summary>
        /// 设定文本框的文本为非格式符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInfo_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsSeparator(e.KeyChar) || char.IsSurrogate(e.KeyChar))
                e.KeyChar = new char();
        }

        private void bunStart_Click(object sender, EventArgs e) {
            bClass.Start();
        }

        private void bunStop_Click(object sender, EventArgs e) {
            bClass.Stop();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (bClass.GetState() != ProcessState.DISPOSE) {
                e.Cancel = true;
                this.AddProcessLog("请等待进程结束并关闭资源！");
                bClass.Dispose();
            } else {
                Application.ExitThread();
            }
        }

        private DateTime startTime = new DateTime();
        private string descText = Properties.Settings.Default.ApplicationName + "\r\n已运行{0}";
        private void recTimer_Tick(object sender, EventArgs e) {
            TimeSpan ts = DateTime.Now.Subtract(startTime);
            this.labRTime.Text = string.Format("{0}天{1}小时{2}分{3}秒", new object[] { ts.Days, ts.Hours, ts.Minutes, ts.Seconds });
            this.niMain.Text = string.Format(descText, this.labRTime.Text);
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.niMain.Visible = true;
                this.Hide();
            }
        }

        private void niMain_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.niMain.Visible = false;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        #endregion

        private string SmsFlag(string flag) {
            switch (flag) {
                case "0":
                    return "发送短信";
                case "1":
                    return "接收短信";
                case "2":
                    return "系统短信";
                default:
                    return "未知短信";
            }
        }

        private string SmsOver(string IsOver) {
            switch (IsOver) {
                case "0":
                    return "未处理，未执行";
                case "1":
                    return "已处理，完成执行";
                case "2":
                    return "无权限或指令格式错误";
                case "3":
                    return "取消执行,错误指令";
                case "4":
                    return "已处理但不发送状态";
                default:
                    return "未知状态";
            }
        }

        private bool firstRun = true;
        private DateTime runTime = DateTime.Now;
        private void waitTimer_Tick(object sender, EventArgs e) {
            if (firstRun) {
                runTime = DateTime.Now.AddMinutes(Properties.Settings.Default.WaitTime);
                firstRun = false;
            }
            if (DateTime.Now.CompareTo(runTime) > 0) {
                this.bunStart_Click(this, e);
                this.waitTimer.Enabled = false;
                this.firstRun = !this.waitTimer.Enabled;
            }
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e) {
            Application.ExitThread();
        }
    }
}