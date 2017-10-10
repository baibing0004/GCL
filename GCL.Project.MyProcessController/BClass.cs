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

        #region 中介者
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
        /// 日志记录者
        /// </summary>
        private Logger logger;

        /// <summary>
        /// 服务
        /// </summary>
        private ABusinessServer[] servers = new ABusinessServer[0];

        /// <summary>
        /// 默认所有的都可以
        /// </summary>
        public BClass() {
        }

        #region 处理外部命令
        private string[] command;
        public void ExecuteCommand(string content) {
            this.logger.Debug(string.Format("Command:{0} {1}", this.command[0], string.Format(this.command[1], content)));
            System.Diagnostics.Process.Start(this.command[0], string.Format(this.command[1], content));
        }
        #endregion

        #region IProcess Members

        /// <summary>
        /// 读取配置信息
        /// </summary>
        public override void Init() {
            try {
                this.ma = ConfigManagerFactory.GetConfigManagerFromFile(ConfigManagerFactory.GetApplicationConfigManager(), AppDomain.CurrentDomain.BaseDirectory + "MyProcessController.pcf");
                this.middler = new Middler(ma);
                this.logger = GetObject("Logger") as Logger;
                this.CallProcessEventSimple(ProcessState.INIT);
                //101 事件读取配置信息
                this.CallProcessEventSimple(LogType.RELEASE, 101, "MyProcessController启动!");
                this.SetLogType(this.logger.DefaultLogRecord.GetLogType());
                this.CallProcessEventSimple(LogType.RELEASE, 101, "成功获得日志等级:" + this.GetLogType().ToString());

                string serName = "";
                try {
                    object[] _s = this.GetObjects("Servers");
                    if (_s == null || _s.Length == 0 || _s[0] == null)
                        throw new Exception("未发现任何业务服务!");
                    foreach (object _t in _s)
                        if (_t == null)
                            throw new Exception("未发现任何业务服务!");
                    //运行目录 system32
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
                        //102事件 业务服务注册成功
                        this.CallProcessEventSimple(LogType.RELEASE, 102, new object[] { ser.GetType().Name + "业务服务注册", ser });
                        ser.Init(this.middler,this.logger);
                        this.CanDisposeByReady = this.CanDisposeByReady && ser.CanDisposeByReady;
                    }
                } catch (Exception ex) {
                    throw new Exception(serName + "注册出现错误\r\n" + ex.ToString(), ex);
                }

                this.CallProcessEventSimple(LogType.RELEASE, 101, "业务服务注册成功!");

                this.command = (ma.GetConfigValue("AppSettings", "MyProcessController.Command") as string).Split(' ');
                this.CallProcessEventSimple(LogType.RELEASE, 101, string.Format("获得执行命令行!{0}", ma.GetConfigValue("AppSettings", "MyProcessController.Command")));

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
        /// 初始化设备 并启动收发和处理线程.
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
                this.CallProcessEventSimple(ex, "启动时出现意外");
            }
        }

        /// <summary>
        /// 关闭收发和处理线程
        /// </summary>
        public override void Stop() {
            try {
                foreach (ABusinessServer server in this.servers)
                    if (!isDispose)
                        server.Stop();
                    else
                        server.Dispose();
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "BClass停止时出现意外!");
            }
        }

        #endregion

        #region 业务线程的线程事件终止与错误事件订阅方法

        void BClass_ProcessEvent(object sender, ProcessEventArg e) {
            if (e.Level == EventLevel.Importent) {
                switch (e.State) {
                    case ProcessState.INIT:
                        //209事件 服务初始化
                        this.CallProcessEventSimple(LogType.RELEASE, 209, new object[] { sender.GetType().Name + "服务初始化", sender });
                        break;
                    case ProcessState.READY:
                        //210事件 服务准备就绪
                        this.CallProcessEventSimple(LogType.RELEASE, 210, new object[] { sender.GetType().Name + "服务准备就绪", sender });
                        break;
                    case ProcessState.START:
                        //201事件 服务启动
                        this.CallProcessEventSimple(LogType.RELEASE, 201, new object[] { sender.GetType().Name + "服务启动", sender });
                        break;
                    case ProcessState.STOP:
                        //204事件 服务终止
                        this.CallProcessEventSimple(LogType.RELEASE, 204, new object[] { sender.GetType().Name + "服务终止", sender });
                        if (CheckStop())
                            this.CallProcessEventSimple(ProcessState.STOP);
                        break;
                    case ProcessState.DISPOSE:
                        //服务彻底关闭
                        //208事件 服务撤销
                        this.CallProcessEventSimple(LogType.RELEASE, 208, new object[] { sender.GetType().Name + "服务撤销", sender });
                        break;
                    case ProcessState.EXCEPTION:
                        //205事件 业务线程因为未捕捉错误终止            
                        this.CallProcessEventSimple(LogType.ERROR, 205, new object[] { sender.GetType().Name + e.ToStringOfPara(0) + ":" + ((ProcessEventArg)e).GetException().ToString(), sender, e });
                        break;
                }
            } else {
                switch (e.EventNumber) {
                    case -202:
                        try {
                            this.ExecuteCommand(e.ToStringOfPara(0));
                            //202事件触发报警通知
                            this.CallProcessEventSimple(LogType.DEBUG, 202, new object[] { "执行命令" + string.Format(" {0} {1} ", this.command[0], string.Format(this.command[1], e.ToStringOfPara(0))) + "成功!", sender, e });
                            this.CallProcessEventSimple(LogType.RELEASE, 202, new object[] { "执行命令成功!", sender, e });
                        } catch (Exception ex) {
                            //203事件报警执行失败
                            this.CallProcessEventSimple(LogType.ERROR, 203, new object[] { "执行命令出错:" + ex.ToString(), sender, e });
                        }
                        break;
                    case -211:
                        this.CallProcessEventSimple(LogType.RELEASE, 211, new object[] { "呼叫程序整体关闭!", sender, e });
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

        #region 公共方法

        public void RefreshSettings() {
            //103事件 刷新Settings列表
            this.CallProcessEventSimple(LogType.RELEASE, 103, new object[] { "刷新Settings列表", this.GetSettings() });
        }

        public string GetSettings() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("日志等级:\t\t" + this.GetLogType().ToString());
            sb.AppendLine("业务服务:");
            foreach (ABusinessServer ser in this.servers) {
                sb.AppendLine("\t\t\t" + ser.GetType().Name);
            }
            sb.Append("执行命令行:\t\t" + this.command[0]);
            sb.AppendLine(" " + this.command[1]);
            return sb.ToString();
        }

        /// <summary>
        /// 发送短信信息
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="msg"></param>
        public void TestAlarm(string msg) {
            try {
                this.ExecuteCommand(msg);
                //206事件 执行测试命令成功!
                this.CallProcessEventSimple(LogType.RELEASE, 206, "执行测试命令成功!");
            } catch (Exception ex) {
                //207事件 执行测试命令出错
                this.CallProcessEventSimple(LogType.ERROR, 207, "执行测试命令出错:" + ex.ToString());
            }
        }

        private DateTime recordTime = new DateTime();
        /// <summary>
        /// 安全纪录日志信息
        /// </summary>
        /// <param name="data"></param>
        public void RecordLog(string data) {
            this.logger.Info(data);
        }

        /// <summary>
        /// 关闭日志
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
                        this.logger.Release("业务程序初始化!");
                        break;
                    case ProcessState.READY:
                        this.logger.Release("业务程序准备就绪！");
                        break;
                    case ProcessState.START:
                        this.logger.Release("业务程序启动！");
                        break;
                    case ProcessState.STOP:
                        this.logger.Release("业务程序停止！");
                        break;
                    case ProcessState.DISPOSE:
                        this.logger.Release("业务程序清除资源完成运行！");
                        this.CloseLog();
                        this.ma.Dispose();
                        break;
                    case ProcessState.EXCEPTION:
                        if (this.logger != null)
                            this.logger.Release("程序发生致命错误！终止运行错误描述" + e.Exception.Message + "");
                        else
                            throw e.Exception;
                        break;
                }
            } else
                this.logger.Log(e.LogType, e.GetEventNumber() + "事件," + e.ToStringOfPara(0));
            if (e.GetLogType() <= this.GetLogType())
                base.CallProcessEventSimple(e);
        }

        #endregion


    }

}