using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GCL.Event;
using GCL.Common;
using GCL.Threading;
using System.Runtime.Serialization;
namespace GCL.Project.MyProcessController {
    public class TestBusinessServer : ABusinessThreadServer {

        public TestBusinessServer() : this(200) { }
        public TestBusinessServer(int waitTime) : base(new TimerThread(waitTime)) { }

        protected override void FirstRun() {
            this.CallProcessEventSimple(301, "首次初始化运行!");
        }

        protected override void Action(object sender, EventArg e) {
            TimerEventArg te = e as TimerEventArg;
            this.CallProcessEventSimple(GCL.IO.Log.LogType.RELEASE, 301, string.Format("测试事件触发 {0} 下次{1}", te.Now, te.Next));
            //this.Alarm("测试警报内容");
            //this.SetCanRun(false);
            //this.Stop();
        }

        protected override void OnClose() {
            throw new NotImplementedException();
        }
    }

    public class TestPoolServer : APoolServer {

        private MessageQueueManager ma;
        public TestPoolServer(MessageQueueManager ma, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshTime)
            : base(createrNum, createrWaitTime, customerNum, customerWaitTime, refreshTime) {
            this.ma = ma;
        }

        public TestPoolServer(MessageQueueManager ma, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshTime, int createWaitTime)
            : base(createrNum, createrWaitTime, customerNum, customerWaitTime, refreshTime, createWaitTime) {
            this.ma = ma;
        }

        protected override object Create(object sender, EventArg e) {
            return new TestEntity();
        }

        protected override void Custom(object sender, EventArg e, object value) {
            try {
                this.ma.Send(value);
                this.CallProcessEventSimple(401, "发送成功!");
            } catch (Exception ex) {
                this.CallProcessEventSimple(402, ex.ToString());
            }
        }

        protected override void RollBack(object sender, EventArg e, object value) {
            this.Custom(sender, e, value);
        }

    }

    public class TestPoolServer2 : AMSMQPoolServer {

        public TestPoolServer2(string msmqPath, string backPath, int timeout, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshTime)
            : base(new XMLMSMQPoolServerStaregy(typeof(TestEntity)), msmqPath, backPath, timeout, createrNum, createrWaitTime, customerNum, customerWaitTime, refreshTime) {
        }

        public TestPoolServer2(string msmqPath, string backPath, int timeout, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshTime, int createWaitTime)
            : base(new XMLMSMQPoolServerStaregy(typeof(TestEntity)), msmqPath, backPath, timeout, createrNum, createrWaitTime, customerNum, customerWaitTime, refreshTime, createWaitTime) {
        }

        protected override void Custom(object sender, EventArg e, object value) {
            TestEntity entity = value as TestEntity;
            this.CallProcessEventSimple(401, "读取对象为：" + entity.A);
        }
    }

    [System.Serializable]
    public class TestEntity {
        public int A { get; set; }
        public TestEntity() {
            A = 2;
        }

        public TestEntity(int t) {
            A = t;
        }

        public static System.Messaging.IMessageFormatter CreateFormatter() {
            return new System.Messaging.XmlMessageFormatter(new Type[] { typeof(TestEntity) });
        }

        public static Type[] ToType(object[] paras) {
            return Array.ConvertAll<object, Type>(paras, To);
        }

        public static Type To(object para) {
            if (para is Type)
                return (Type)para;
            else
                throw new InvalidOperationException(string.Format("{0}类型转换失败！", para.GetType().Name));
        }
    }

    public class TestTextPoolServer : APoolServer {

        private TextWriter writer;
        private System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        public TestTextPoolServer(string file, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshTime)
            : base(createrNum, createrWaitTime, customerNum, customerWaitTime, refreshTime) {
            this.writer = new System.IO.StreamWriter(file, true, System.Text.Encoding.ASCII);
        }

        int c = 20;
        protected override object Create(object sender, EventArg e) {
            return new TestEntity(c++);
        }

        int count = 0;
        object key = DateTime.Now;
        protected override void Custom(object sender, EventArg e, object value) {
            try {
                MemoryStream sw = new MemoryStream();
                formatter.Serialize(sw, value);
                byte[] data = sw.ToArray();
                sw.Close();
                lock (key) {
                    writer.WriteLine(System.Text.Encoding.ASCII.GetString(data));
                }
                this.CallProcessEventSimple(401, "写入成功" + count++);
            } catch (Exception ex) {
                this.CallProcessEventSimple(402, ex.ToString());
            }
        }

        protected override void RollBack(object sender, EventArg e, object value) {
            this.Custom(sender, e, value);
        }

        protected override void OnClose(object sender, EventArg e) {
            try {
                writer.Flush();
            } catch {
            }
            try {
                writer.Close();
            } catch {
            }
            base.OnClose(sender, e);
        }

    }
    public class TestTextFileServer : ATextFilePoolServer {

        private IFormatter formatter;
        private string delpath;
        public TestTextFileServer(string path, int minCapacity, string delpath, int createrNum, int createrWaitTime, int customerNum, int customerWaitTime, int refreshWaitTime)
            : base(path, minCapacity, delpath, createrNum, createrWaitTime, customerNum, customerWaitTime, refreshWaitTime) {
            this.delpath = delpath;
            formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }
        protected override System.IO.TextReader GetReader(System.IO.FileInfo info) {
            return new System.IO.StreamReader(info.FullName, Encoding.ASCII);
        }

        protected override void RemoveFile(System.IO.FileInfo info) {
            string dirname = delpath + "/" + DateTime.Now.ToString("yyMMddHHmmss");
            //Directory.CreateDirectory(dirname);
            info.MoveTo(dirname + info.Name.Replace(".temp", ""));
        }

        protected override void Custom(object sender, EventArg e, object value) {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(value as string);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            try {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                TestEntity entity = formatter.Deserialize(stream) as TestEntity;
                this.CallProcessEventSimple(GCL.IO.Log.LogType.RELEASE, 401, entity.A.ToString());
            } catch (Exception ex) {
                this.CallProcessEventSimple(GCL.IO.Log.LogType.RELEASE, 402, "解析出错，进入错误文件！\r\n" + ex.ToString());
                this.AddErrFile(value.ToString());
            } finally {
                try {
                    stream.Close();
                } catch {
                }
            }
        }

        protected override void RollBack(object sender, EventArg e, object value) {
            this.Custom(sender, e, value);
        }
    }
}
