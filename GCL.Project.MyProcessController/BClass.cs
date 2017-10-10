using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using GCL.Common;
using GCL.Event;
using GCL.Threading;
using GCL.Threading.Process;
using GCL.IO.Log;
using GCL.IO.Config;
using GCL.Bean.Middler;

namespace GCL.Project.MyProcessController {

    public class BClass : AbstractProcess {

        #region �н���
        private Middler middler;
        private ConfigManager ma;
        public object GetObject(string key) {
            return middler.GetObjectByAppName("MyProcessController", key);
        }
        public object[] GetObjects(string key) {
            return middler.GetObjectsByAppName("MyProcessController", key);
        }
        #endregion

        /// <summary>
        /// ��־��¼��
        /// </summary>
        private Logger logger;

        /// <summary>
        /// ����
        /// </summary>
        private ABusinessServer[] servers = new ABusinessServer[0];

        /// <summary>
        /// Ĭ�����еĶ�����
        /// </summary>
        public BClass() {
        }

        #region �����ⲿ����
        private string[] command;
        public void ExecuteCommand(string content) {
            this.logger.Debug(string.Format("Command:{0} {1}", this.command[0], string.Format(this.command[1], content)));
            System.Diagnostics.Process.Start(this.command[0], string.Format(this.command[1], content));
        }
        #endregion

        #region IProcess Members

        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        public override void Init() {
            try {
                this.ma = ConfigManagerFactory.GetConfigManagerFromFile(ConfigManagerFactory.GetApplicationConfigManager(), AppDomain.CurrentDomain.BaseDirectory + "MyProcessController.pcf");
                this.middler = new Middler(ma);
                this.logger = GetObject("Logger") as Logger;
                this.CallProcessEventSimple(ProcessState.INIT);
                //101 �¼���ȡ������Ϣ
                this.CallProcessEventSimple(LogType.RELEASE, 101, "MyProcessController����!");
                this.SetLogType(this.logger.DefaultLogRecord.GetLogType());
                this.CallProcessEventSimple(LogType.RELEASE, 101, "�ɹ������־�ȼ�:" + this.GetLogType().ToString());

                string serName = "";
                try {
                    object[] _s = this.GetObjects("Servers");
                    if (_s == null || _s.Length == 0 || _s[0] == null)
                        throw new Exception("δ�����κ�ҵ�����!");
                    foreach (object _t in _s)
                        if (_t == null)
                            throw new Exception("δ�����κ�ҵ�����!");
                    //����Ŀ¼ system32
                    //throw new Exception(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
                    //object a = Activator.CreateInstanceFrom("Boss.DAL.dll", "Boss.DAL.Bill");
                    //throw new Exception(_s[0] == null ? "N" : "Y");
                    //throw new Exception(_s[0].GetType().Name);
                    this.servers = new ABusinessServer[_s.Length];
                    for (int w = 0; w < _s.Length; w++)
                        this.servers[w] = _s[w] as ABusinessServer;

                    foreach (ABusinessServer ser in this.servers) {
                        serName = ser.GetType().Name;
                        ser.ProcessEvent += new ProcessEventHandle(BClass_ProcessEvent);
                        //102�¼� ҵ�����ע��ɹ�
                        this.CallProcessEventSimple(LogType.RELEASE, 102, new object[] { ser.GetType().Name + "ҵ�����ע��", ser });
                        ser.Init(this.middler,this.logger);
                        this.CanDisposeByReady = this.CanDisposeByReady && ser.CanDisposeByReady;
                    }
                } catch (Exception ex) {
                    throw new Exception(serName + "ע����ִ���\r\n" + ex.ToString(), ex);
                }

                this.CallProcessEventSimple(LogType.RELEASE, 101, "ҵ�����ע��ɹ�!");

                this.command = (ma.GetConfigValue("AppSettings", "MyProcessController.Command") as string).Split(' ');
                this.CallProcessEventSimple(LogType.RELEASE, 101, string.Format("���ִ��������!{0}", ma.GetConfigValue("AppSettings", "MyProcessController.Command")));

                this.CallProcessEventSimple(ProcessState.READY);
                this.RefreshSettings();
            } catch (Exception ex) {
                try {
                    foreach (ABusinessServer server in servers)
                        server.Dispose();
                } catch {
                } finally {
                    this.servers = new ABusinessServer[0];
                    this.CanDisposeByReady = true;
                }
                this.CallProcessEventSimple(ex, ProcessState.INIT);
                //try {
                //    this.logger.Close();
                //} catch {
                //}
            }
        }



        /// <summary>
        /// ��ʼ���豸 �������շ��ʹ����߳�.
        /// </summary>
        public override void Start() {
            try {
                this.CallProcessEventSimple(ProcessState.START);
                foreach (ABusinessServer server in this.servers)
                    server.Start();
            } catch (Exception ex) {
                foreach (ABusinessServer server in this.servers)
                    if (Tool.IsEnable(server))
                        server.Stop();
                this.CallProcessEventSimple(ex, "����ʱ��������");
            }
        }

        /// <summary>
        /// �ر��շ��ʹ����߳�
        /// </summary>
        public override void Stop() {
            try {
                foreach (ABusinessServer server in this.servers)
                    if (!isDispose)
                        server.Stop();
                    else
                        server.Dispose();
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "BClassֹͣʱ��������!");
            }
        }

        #endregion

        #region ҵ���̵߳��߳��¼���ֹ������¼����ķ���

        void BClass_ProcessEvent(object sender, ProcessEventArg e) {
            if (e.Level == EventLevel.Importent) {
                switch (e.State) {
                    case ProcessState.INIT:
                        //209�¼� �����ʼ��
                        this.CallProcessEventSimple(LogType.RELEASE, 209, new object[] { sender.GetType().Name + "�����ʼ��", sender });
                        break;
                    case ProcessState.READY:
                        //210�¼� ����׼������
                        this.CallProcessEventSimple(LogType.RELEASE, 210, new object[] { sender.GetType().Name + "����׼������", sender });
                        break;
                    case ProcessState.START:
                        //201�¼� ��������
                        this.CallProcessEventSimple(LogType.RELEASE, 201, new object[] { sender.GetType().Name + "��������", sender });
                        break;
                    case ProcessState.STOP:
                        //204�¼� ������ֹ
                        this.CallProcessEventSimple(LogType.RELEASE, 204, new object[] { sender.GetType().Name + "������ֹ", sender });
                        if (CheckStop())
                            this.CallProcessEventSimple(ProcessState.STOP);
                        break;
                    case ProcessState.DISPOSE:
                        //���񳹵׹ر�
                        //208�¼� ������
                        this.CallProcessEventSimple(LogType.RELEASE, 208, new object[] { sender.GetType().Name + "������", sender });
                        break;
                    case ProcessState.EXCEPTION:
                        //205�¼� ҵ���߳���Ϊδ��׽������ֹ            
                        this.CallProcessEventSimple(LogType.ERROR, 205, new object[] { sender.GetType().Name + e.ToStringOfPara(0) + ":" + ((ProcessEventArg)e).GetException().ToString(), sender, e });
                        break;
                }
            } else {
                switch (e.EventNumber) {
                    case -202:
                        try {
                            this.ExecuteCommand(e.ToStringOfPara(0));
                            //202�¼���������֪ͨ
                            this.CallProcessEventSimple(LogType.DEBUG, 202, new object[] { "ִ������" + string.Format(" {0} {1} ", this.command[0], string.Format(this.command[1], e.ToStringOfPara(0))) + "�ɹ�!", sender, e });
                            this.CallProcessEventSimple(LogType.RELEASE, 202, new object[] { "ִ������ɹ�!", sender, e });
                        } catch (Exception ex) {
                            //203�¼�����ִ��ʧ��
                            this.CallProcessEventSimple(LogType.ERROR, 203, new object[] { "ִ���������:" + ex.ToString(), sender, e });
                        }
                        break;
                    case -211:
                        this.CallProcessEventSimple(LogType.RELEASE, 211, new object[] { "���г�������ر�!", sender, e });
                        this.Dispose();
                        break;
                    default:
                        if (e.EventNumber >= 1000)
                            this.CallProcessEventSimple(e.LogType, e.EventNumber, new object[] { e.ToStringOfPara(0), sender, e });
                        else
                            this.CallProcessEventSimple(e.LogType, 1000 + e.EventNumber, new object[] { e.ToStringOfPara(0), sender, e });
                        break;
                }
            }
        }

        protected override bool CheckStop() {
            bool isStop = true;
            foreach (ABusinessServer server in servers)
                if (!(isStop && (server.GetState() == ProcessState.STOP) || server.GetState() == ProcessState.DISPOSE))
                    isStop = false;
            return isStop;
        }

        protected override void CheckDispose(bool dispose) {
            bool checkDispose = true;
            foreach (ABusinessServer server in servers)
                if (!(dispose && checkDispose && server.GetState() == ProcessState.DISPOSE))
                    checkDispose = false;
            base.CheckDispose(checkDispose || (this.CanDisposeByReady && dispose));
        }



        #endregion

        #region ��������

        public void RefreshSettings() {
            //103�¼� ˢ��Settings�б�
            this.CallProcessEventSimple(LogType.RELEASE, 103, new object[] { "ˢ��Settings�б�", this.GetSettings() });
        }

        public string GetSettings() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("��־�ȼ�:\t\t" + this.GetLogType().ToString());
            sb.AppendLine("ҵ�����:");
            foreach (ABusinessServer ser in this.servers) {
                sb.AppendLine("\t\t\t" + ser.GetType().Name);
            }
            sb.Append("ִ��������:\t\t" + this.command[0]);
            sb.AppendLine(" " + this.command[1]);
            return sb.ToString();
        }

        /// <summary>
        /// ���Ͷ�����Ϣ
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="msg"></param>
        public void TestAlarm(string msg) {
            try {
                this.ExecuteCommand(msg);
                //206�¼� ִ�в�������ɹ�!
                this.CallProcessEventSimple(LogType.RELEASE, 206, "ִ�в�������ɹ�!");
            } catch (Exception ex) {
                //207�¼� ִ�в����������
                this.CallProcessEventSimple(LogType.ERROR, 207, "ִ�в����������:" + ex.ToString());
            }
        }

        private DateTime recordTime = new DateTime();
        /// <summary>
        /// ��ȫ��¼��־��Ϣ
        /// </summary>
        /// <param name="data"></param>
        public void RecordLog(string data) {
            this.logger.Info(data);
        }

        /// <summary>
        /// �ر���־
        /// </summary>
        public void CloseLog() {
            try {
                this.logger.Close();
            } catch {
            }
        }


        protected override void CallProcessEventSimple(ProcessEventArg e) {
            if (e.Level == EventLevel.Importent) {
                switch (e.State) {
                    case ProcessState.INIT:
                        this.logger.Release("ҵ������ʼ��!");
                        break;
                    case ProcessState.READY:
                        this.logger.Release("ҵ�����׼��������");
                        break;
                    case ProcessState.START:
                        this.logger.Release("ҵ�����������");
                        break;
                    case ProcessState.STOP:
                        this.logger.Release("ҵ�����ֹͣ��");
                        break;
                    case ProcessState.DISPOSE:
                        this.logger.Release("ҵ����������Դ������У�");
                        this.CloseLog();
                        this.ma.Dispose();
                        break;
                    case ProcessState.EXCEPTION:
                        if (this.logger != null)
                            this.logger.Release("����������������ֹ���д�������" + e.Exception.Message + "");
                        else
                            throw e.Exception;
                        break;
                }
            } else
                this.logger.Log(e.LogType, e.GetEventNumber() + "�¼�," + e.ToStringOfPara(0));
            if (e.GetLogType() <= this.GetLogType())
                base.CallProcessEventSimple(e);
        }

        #endregion


    }

}