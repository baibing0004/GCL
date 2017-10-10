//#define EQUAL
//#define QUEUE
using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
using GCL.Common;
using GCL.Threading;
using GCL.Threading.Process;
using GCL.IO.Log;
using System.Reflection;
using System.Configuration;
using System.IO;
using GCL.Bean.Middler;

namespace GCL.Project.MyProcessController {

    /// <summary>
    /// 基本插件类
    /// 需要处理init(),start(),stop()等操作，注意CallProcessEvent传出状态
    /// </summary>
    public abstract class ABusinessServer : AbstractProcess {

        /// <summary>
        /// 
        /// </summary>
        public ABusinessServer() { }

        /// <summary>
        /// 便于外部设定日志格式化字符串
        /// </summary>
        /// <param name="type"></param>
        public ABusinessServer(string formatter)
            : this() {
            this.alarmFormatter = formatter;
        }

        protected virtual void Alarm(string alarmText) {
            this.CallProcessEventSimple(LogType.RELEASE, -202, alarmText);
        }

        /// <summary>
        /// 呼叫外部程序关闭
        /// </summary>
        protected virtual void CallDispose() {
            this.CallProcessEventSimple(LogType.RELEASE, -211, "");
        }

        private string alarmFormatter = "{0}";

        /// <summary>
        /// 格式化错误报警,当节点不存在时抱错！
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="content">内容</param>
        /// <param name="isAbPath">节点是否是全路径（否的情况下默认为"类名."开头）</param>
        protected virtual void AlarmFormat(string key, object content, bool isAbPath) {
            this.Alarm(string.Format(this.alarmFormatter, content));
        }
        /// <summary>
        /// 格式化错误报警，当节点不存在时抱错！
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="content">内容</param>
        /// <param name="isAbPath">节点是否是全路径（默认为"类名."开头）</param>
        protected virtual void AlarmFormat(string key, object content) {
            this.AlarmFormat(key, content, false);
        }

        private Middler mi;
        public Middler Middler {
            get { return mi; }
        }

        private Logger log;
        public Logger Logger {
            get { return log; }
        }

        internal void Init(Middler middler, Logger logger) {
            this.mi = middler;
            this.log = logger;
            this.Init();
        }
    }





    //#if(DEBUG)


    //    //public class ApplyPoolBusinessServer : ABusinessServer {
    //    //    private CCPoolProcess poolProcess;
    //    //    public ApplyPoolBusinessServer() {
    //    //        poolProcess = new CCPoolProcess(new CreaterAction(this.Create), new CustomerAction(this.Custom), new CustomerAction(this.RollBack), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.Creaters"]),
    //    //    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.CreaterWaitTime"]),
    //    //            Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.Customers"]),
    //    //            Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.CustomerWaitTime"]),
    //    //            Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.RefreshWaitTime"]));
    //    //        poolProcess.ProcessEvent += new ProcessEventHandle(poolProcess_ProcessEvent);
    //    //        this.CanDisposeByReady = poolProcess.CanDisposeByReady;
    //    //    }
    //    //    private StreamReader sr = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.FilePath"],Encoding.GetEncoding(System.Configuration.ConfigurationManager.AppSettings["ApplyPoolBusinessServer.Encoding"]));


    //    //    #region 获得对象与消费对象的方法封装
    //    //    private int count=0;
    //    //    protected object Create(object sender, EventArg e) {
    //    //        if (!sr.EndOfStream) {
    //    //            string data = sr.ReadLine();
    //    //            this.CallProcessEventSimple(301, count.ToString() + "行获取数据" + data);
    //    //            count++;
    //    //            return data;
    //    //        } else return null;
    //    //    }

    //    //    protected void Custom(object sender, EventArg e, object value) {
    //    //        if (value == null)
    //    //            ((CircleThread)sender).SetCanRun(false);
    //    //        else {
    //    //            string data = value.ToString();
    //    //            if (data.IndexOf("304事件") >= 0) {

    //    //            }
    //    //        }
    //    //        this.CallProcessEventSimple(302, thread.GetNum() + "消费者处理一个对象" + poolProcess.Pool.GetSize());
    //    //    }



    //    //    protected void RollBack(object sender, EventArg e, object value) {
    //    //        Custom(sender, e, value);            
    //    //    }
    //    //    #endregion

    //    //    public override void Init() {
    //    //        poolProcess.Init();            
    //    //    }

    //    //    public override void Start() {
    //    //        poolProcess.Start();
    //    //    }

    //    //    public override void Stop() {
    //    //        if (!isDispose)
    //    //            poolProcess.Stop();
    //    //        else
    //    //            poolProcess.Dispose();
    //    //    }

    //    //    protected override bool CheckStop() {
    //    //        return poolProcess.GetState() == ProcessState.STOP || poolProcess.GetState() == ProcessState.DISPOSE;
    //    //    }

    //    //    void poolProcess_ProcessEvent(object sender, ProcessEventArg e) {
    //    //        if ((sr.EndOfStream || this.isDispose) && e.GetState() == ProcessState.STOP) {
    //    //            try {
    //    //                sr.Close();
    //    //            } catch {
    //    //            }
    //    //            this.isDispose = true;
    //    //        }
    //    //        this.CallProcessEventSimple(e);
    //    //    }

    //    //}
    //    public class TestPoolBusinessServer : ABusinessServer {

    //        private CCPoolProcess poolProcess;
    //        public TestPoolBusinessServer() {
    //            poolProcess = new CCPoolProcess(new CreaterAction(this.Create), new CustomerAction(this.Custom), new CustomerAction(this.RollBack), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TestPoolBusinessServer.Creaters"]),
    //        Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TestPoolBusinessServer.CreaterWaitTime"]),
    //                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TestPoolBusinessServer.Customers"]),
    //                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TestPoolBusinessServer.CustomerWaitTime"]),
    //                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TestPoolBusinessServer.RefreshWaitTime"]));
    //            poolProcess.ProcessEvent += new ProcessEventHandle(poolProcess_ProcessEvent);
    //            this.CanDisposeByReady = poolProcess.CanDisposeByReady;
    //#if(EQUAL)
    //            createWaitTime = 1000;
    //            customerWaitTime = 1000;
    //#endif

    //#if(QUEUE)
    //            createWaitTime = 1000;
    //            customerWaitTime = 0;
    //#endif
    //            queues = new System.Messaging.MessageQueue[poolProcess.Creaters.Length];
    //            for (int w = 0; w < queues.Length; w++) {
    //                queues[w] = new System.Messaging.MessageQueue(@".\private$\abcd");
    //                queues[w].Formatter = formatter;
    //            }
    //        }

    //        private System.Messaging.MessageQueue[] queues;
    //        private System.Messaging.XmlMessageFormatter formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(string) });
    //        void poolProcess_ProcessEvent(object sender, ProcessEventArg e) {
    //            if (isDispose && e.GetState() == ProcessState.STOP) {
    //                try {
    //                    System.Messaging.MessageQueue.ClearConnectionCache();
    //                } catch {
    //                }
    //                foreach (System.Messaging.MessageQueue queue in queues)
    //                    try {
    //                        lock (queue) {
    //                            queue.Close();
    //                        }
    //                    } catch {
    //                    }
    //            }
    //            this.CallProcessEventSimple(e);
    //        }

    //        private int createWaitTime, customerWaitTime;
    //        protected object Create(object sender, EventArg e) {
    //            ProtectThread thread = sender as ProtectThread;
    //            if (createWaitTime > 0)
    //                Tool.ObjectSleep(createWaitTime);
    //            this.CallProcessEventSimple(301, thread.GetNum() + "构造者创造一个对象!" + poolProcess.Pool.GetSize());
    //            return "";
    //        }

    //        protected void Custom(object sender, EventArg e, object value) {
    //            ProtectThread thread = sender as ProtectThread;
    //            if (customerWaitTime > 0)
    //                Tool.ObjectSleep(customerWaitTime);
    //            this.CallProcessEventSimple(302, thread.GetNum() + "消费者消费一个对象!" + poolProcess.Pool.GetSize());
    //        }

    //        protected void RollBack(object sender, EventArg e, object value) {
    //            if (sender is CreaterPoolThread)
    //                queues[((ProtectThread)sender).GetNum()].Send(value);
    //            else
    //                queues[0].Send(value);
    //            this.CallProcessEventSimple(303, "回滚一个对象!" + poolProcess.Pool.GetSize());
    //        }

    //        public override void Init() {
    //            poolProcess.Init();
    //        }

    //        public override void Start() {
    //            poolProcess.Start();
    //        }

    //        public override void Stop() {
    //            if (!isDispose)
    //                poolProcess.Stop();
    //            else
    //                poolProcess.Dispose();
    //        }

    //        protected override bool CheckStop() {
    //            return poolProcess.GetState() == ProcessState.STOP || poolProcess.GetState() == ProcessState.DISPOSE;
    //        }


    //    }

    //    public class TestEntity {
    //        private int a, b;

    //        public int B {
    //            get { return b; }
    //            set { b = value; }
    //        }

    //        public int A {
    //            get { return a; }
    //            set { a = value; }
    //        }

    //        public TestEntity(int a, int b) {
    //            A = a;
    //            B = b;
    //        }

    //        public TestEntity() { }
    //    }

    //    public class TestMSMQSendServer : ABusinessThreadServer {
    //        public TestMSMQSendServer():base(new TimerThread(500))
    //        {
    //            backmsmqPath = System.Configuration.ConfigurationManager.AppSettings[this.GetType().Name + ".MSMQPath"];            
    //        }

    //        public string backmsmqPath;
    //        private MessageQueue backQueue;

    //        protected MessageQueue BackQueue {
    //            get { return backQueue; }
    //            set { backQueue = value; }
    //        }
    //        private TimeSpan timeSpan;
    //        private IMessageFormatter formatter = new XmlMessageFormatter(new Type[] { typeof(TestEntity) });
    //        public void SendBackQueue(object obj) {
    //            try {
    //                this.backQueue.Send(obj);
    //            } catch (Exception ex) {
    //                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
    //                    //读取信息超时      
    //                    this.CallProcessEventSimple(311, string.Format("BackQueue发送连接超时错误"));
    //                } else {
    //                    lock (backQueue) {
    //                        try {
    //                            BackQueue.Close();
    //                        } catch {
    //                        } finally {
    //                            backQueue = null;
    //                        }
    //                        backQueue = new MessageQueue(backmsmqPath);
    //                        backQueue.Formatter = formatter;
    //                        backQueue.DefaultPropertiesToSend.Recoverable = true;
    //                    }
    //                    this.CallProcessEventSimple(310, string.Format("BackQueue发送错误{0},已经重新建立连接!", ex.ToString()));
    //                    throw ex;
    //                }
    //            }
    //        }
    //        protected Random random = new Random();

    //        protected override void Action(object sender, EventArg e) {
    //            if (((GCL.Threading.CircleThread)sender).IsFirst()) {
    //                backQueue = new MessageQueue(backmsmqPath);
    //                backQueue.Formatter = formatter;
    //                backQueue.DefaultPropertiesToSend.Recoverable = true;
    //            }
    //            TestEntity entity = new TestEntity(random.Next(10000), random.Next(100));
    //            SendBackQueue(entity);
    //            this.CallProcessEventSimple(301,"发送成功"+entity.A+"/"+entity.B);
    //        }

    //        protected override void FirstRun() {            
    //        }


    //        protected override void OnClose() {
    //            lock (backQueue) {
    //                try {
    //                    BackQueue.Close();
    //                } catch {
    //                } finally {
    //                    backQueue = null;
    //                }
    //            }
    //        }
    //    }

    //    //public class TestMSMQRecServer : AMSMQPoolServer<TestEntity> {
    //    //    public TestMSMQRecServer() {

    //    //    }

    //    //    protected override void Custom(object sender, EventArg e, object value) {
    //    //        TestEntity entity = value as TestEntity;
    //    //        this.CallProcessEventSimple(301, "接收成功" + entity.A + "/" + entity.B);
    //    //        this.SendBackQueue(entity);
    //    //        this.CallProcessEventSimple(302, "发送成功" + entity.A + "/" + entity.B);
    //    //    }
    //    //}
    //    //public class TestMSMQRecServer2 : AMSMQPoolServer<TestEntity> {
    //    //    protected override void Custom(object sender, EventArg e, object value) {
    //    //        TestEntity entity = value as TestEntity;
    //    //        this.CallProcessEventSimple(301, "接收成功" + entity.A + "/" + entity.B);

    //    //    }
    //    //}
    //#endif
}