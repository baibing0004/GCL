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
            MessageBox.Show(string.Format("����Application_ThreadExceptionδ�����쳣��:\r\n{0}", e.Exception.ToString()), "MPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            bClass.RecordLog("����Form��UI�߳�δ�����쳣��" + e.Exception.ToString());
            this.Close();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show(string.Format("����CurrentDomain_UnhandledExceptionδ�����쳣��:\r\n{0}", e.ExceptionObject.ToString()), "MPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            bClass.RecordLog("�������򼶱�δ�����쳣��" + e.ExceptionObject.ToString());
            bClass.TestAlarm("�����쳣�ر�!");
            this.Close();
            //�Զ�����
            //System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);
        }

        #region �����¼���װ
        void bClass_ProcessEvent1(object sender, ProcessEventArg e) {
            //����contral�ķ�������������Ȩ�޴����¼�����
            this.Invoke(new ProcessEventHandle(bClass_ProcessEvent), new object[] { sender, e });
        }

        void bClass_ProcessEvent(object sender, ProcessEventArg e) {
            try {
                if (e.Level == EventLevel.Importent) {
                    #region ������̼��¼�
                    switch (e.State) {
                        case ProcessState.INIT:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "��ʼ��ʼ��!");
                            break;
                        case ProcessState.READY:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "׼��������");
                            this.bunStart.Enabled = true;
                            break;
                        case ProcessState.START:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "��ʼ������");

                            this.labSTdesc.Text = "����ʱ��:";
                            this.startTime = DateTime.Now;
                            this.labSTime.Text = Tool.FormatDate(startTime);
                            this.recTimer.Start();

                            this.waitTimer.Stop();
                            this.firstRun = false;
                            this.bunStart.Enabled = false;
                            this.bunStop.Enabled = true;
                            break;
                        case ProcessState.STOP:
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "ֹͣ��");

                            this.labSTdesc.Text = "ֹͣʱ��:";
                            this.recTimer.Stop();

                            this.firstRun = true;
                            this.waitTimer.Start();

                            this.labSTime.Text = Tool.FormatNow();
                            this.niMain.Text = Properties.Settings.Default.ApplicationName;
                            bClass.RecordLog(string.Format("�˴�ִ�д���ʱ�乲", this.labRTime.Text));
                            this.bunStart.Enabled = true;
                            this.bunStop.Enabled = false;
                            break;
                        case ProcessState.DISPOSE:
                            waitTimer.Stop();
                            waitTimer.Enabled = false;
                            this.AddProcessLog(Properties.Settings.Default.ApplicationName + "�����Դ������У�");
                            this.Close();
                            //Application.ExitThread();
                            break;
                        case ProcessState.EXCEPTION:
                            string desc = "";
                            switch (((AbstractProcess)sender).GetState()) {
                                case ProcessState.INIT:
                                    desc = "��ʼ��";
                                    break;
                                case ProcessState.START:
                                    desc = "��������";
                                    break;
                                case ProcessState.STOP:
                                    desc = "�رս���";
                                    break;
                                case ProcessState.DISPOSE:
                                    desc = "��ֹ����";
                                    break;
                            }
                            this.AddProcessLog("����" + desc + "��������������ֹ���д�������" + e.Exception.Message);
                            this.bunStop_Click(this, new EventArgs());
                            this.firstRun = true;
                            this.waitTimer.Start();
                            break;
                    }
                    #endregion
                } else {
                    #region ������ͨ�¼�

                    /*�¼�˵����
                 * 101�¼� ��ȡ������Ϣ
                 * 102�¼� ҵ�����ע��
                 * 103�¼� ˢ��Settings�б�
                 * 201�¼� ҵ���������
                 * 202�¼� �ɹ���������֪ͨ
                 * 203�¼� ����ִ��ʧ��
                 * 204�¼� ҵ���߳���ֹ
                 * 205�¼� ҵ���߳���Ϊδ��׽������ֹ
                 * 206�¼� ִ�в�������ɹ�!
                 * 207�¼� ִ�в����������
                 * 208�¼� ������
                 * 209�¼� �����ʼ��
                 * 210�¼� ����׼������
                 */
                    string desc = string.Format("{0}�¼�,{1}", e.GetEventNumber(), e.ToStringOfPara(0));
                    switch (e.EventNumber) {

                        #region ���̼��¼�
                        case 101:
                            //101 �¼���ȡ������Ϣ
                            //���̼��¼�
                            this.AddProcessLog(desc);
                            break;
                        case 102:
                            //102�¼� ҵ�����ע��
                            this.AddTabPage((ABusinessServer)e.GetPara(1));
                            this.AddProcessLog(desc);
                            break;
                        case 103:
                            this.txtSettings.Text = e.ToStringOfPara(1);
                            break;
                        #endregion

                        default:
                            if (e.Length > 1 && e.GetPara(1) is ABusinessServer)
                                this.AddListContent(this.lstAll, e.GetPara(1).GetType().Name + "���� " + desc);
                            else
                                this.AddListContent(this.lstAll, desc);
                            break;
                    }
                    #endregion
                }
            } catch (Exception ex) {
                this.AddProcessLog("�������δ��׽���� " + ex.ToString());
                //this.ShowMessage("�������δ��׽���� " + ex.ToString(), MessageBoxIcon.Stop);
            }
        }
        #endregion

        #region ��������
        private void AddTabPage(ABusinessServer server) {
            ServerPanel sp = new ServerPanel(this.bClass, server, MAXLISTLINES);
            sp.Dock = DockStyle.Fill;
            TabPage tp = new TabPage();
            tp.Name = "tp" + server.GetType().Name;
            tp.Text = server.GetType().Name + "�������" + (tcAll.TabPages.Count - 2);
            tp.ImageIndex = tcAll.TabPages.Count;
            tp.Controls.Add(sp);
            
            tcAll.TabPages.Add(tp);
        }

        /// <summary>
        /// ��Ҫ����ͳһ��ʾ�ĸ�ʽ
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string msg) {
            return ShowMessage(msg, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        /// <summary>
        /// ��Ҫ����ͳһ��ʾ�ĸ�ʽ
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        private DialogResult ShowMessage(string msg, MessageBoxIcon icon) {
            return ShowMessage(msg, MessageBoxButtons.OK, icon);
        }
        /// <summary>
        /// ��Ҫ����ͳһ��ʾ�ĸ�ʽ
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
        /// ��ӽ�����־
        /// </summary>
        /// <param name="msg"></param>
        private void AddProcessLog(string msg) {
            this.AddListContent(this.lstProcess, msg);
            this.AddListContent(this.lstAll, msg);
        }

        #endregion

        #region �ؼ�����

        private void FrmMain_Load(object sender, EventArgs e) {
            this.bClass.Init();
        }

        /// <summary>
        /// ���Ͷ��Ų���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bunTest_Click(object sender, EventArgs e) {
            try {
                bClass.TestAlarm(this.txtInfo.Text);
                this.ShowMessage("������Ϣ����ɹ���", MessageBoxIcon.Information);
            } catch (Exception ex) {
                this.ShowMessage(ex.Message, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// �趨�ı����ڵ��ı�Ϊ��������Ʒ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSendTime_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.KeyChar = new char();
        }

        /// <summary>
        /// �趨�ı�����ı�Ϊ�Ǹ�ʽ��
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
                this.AddProcessLog("��ȴ����̽������ر���Դ��");
                bClass.Dispose();
            } else {
                Application.ExitThread();
            }
        }

        private DateTime startTime = new DateTime();
        private string descText = Properties.Settings.Default.ApplicationName + "\r\n������{0}";
        private void recTimer_Tick(object sender, EventArgs e) {
            TimeSpan ts = DateTime.Now.Subtract(startTime);
            this.labRTime.Text = string.Format("{0}��{1}Сʱ{2}��{3}��", new object[] { ts.Days, ts.Hours, ts.Minutes, ts.Seconds });
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
                    return "���Ͷ���";
                case "1":
                    return "���ն���";
                case "2":
                    return "ϵͳ����";
                default:
                    return "δ֪����";
            }
        }

        private string SmsOver(string IsOver) {
            switch (IsOver) {
                case "0":
                    return "δ����δִ��";
                case "1":
                    return "�Ѵ������ִ��";
                case "2":
                    return "��Ȩ�޻�ָ���ʽ����";
                case "3":
                    return "ȡ��ִ��,����ָ��";
                case "4":
                    return "�Ѵ���������״̬";
                default:
                    return "δ֪״̬";
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