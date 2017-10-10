
using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
using GCL.Common;
using GCL.Threading;
using GCL.Threading.Process;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Messaging;
using GCL.IO.Log;

namespace GCL.Project.MyProcessController {

    #region 消息队列重载
    public class MessageQueueManager {
        private IMessageFormatter formatter = null;
        private string mqPath;
        private MessageQueue queue;

        public MessageQueue MessageQueue {
            get { return queue; }
        }

        public MessageQueueManager(string mqPath, IMessageFormatter formatter) {
            this.mqPath = mqPath;
            this.formatter = formatter;
            this.BuildQueue();
        }

        public virtual void ReSet() {
            this.Close();
            this.BuildQueue();
        }

        private MessageQueue BuildQueue() {
            if (queue == null) {
                queue = new MessageQueue(mqPath);
                queue.Formatter = formatter;
                queue.DefaultPropertiesToSend.Recoverable = true;
            }
            return queue;
        }

        public Message Receive(TimeSpan timeSpan) {
            try {
                return this.queue.Receive(timeSpan);
            } catch (Exception ex) {
                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
                    //读取信息超时                    
                } else
                    lock (queue) {
                        this.ReSet();
                    }
                throw ex;
            }
        }

        public void Send(object obj) {
            try {
                lock (this.queue) {
                    this.queue.Send(obj);
                }
            } catch (Exception ex) {
                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
                } else
                    lock (queue) {
                        this.ReSet();
                    }

                throw ex;
            }
        }

        public virtual void Close() {
            try {
                lock (queue) {
                    queue.Close();
                }
            } catch {
            } finally {
                queue = null;
            }
        }
    }
    #endregion

    #region MSMQPoolServer Formatter策略
    public interface MSMQPoolServerStaregy {
        IMessageFormatter GetMessageFormatter();
    }


    public class XMLMSMQPoolServerStaregy : MSMQPoolServerStaregy {

        private Type type;
        public XMLMSMQPoolServerStaregy(Type type) {
            this.type = type;
        }

        #region MSMQPoolServerStaregy Members

        public IMessageFormatter GetMessageFormatter() {
            return new XmlMessageFormatter(new Type[] { this.type });
        }

        #endregion
    }

    public class BinaryMSMQPoolServerStaregy : MSMQPoolServerStaregy {

        #region MSMQPoolServerStaregy Members

        public IMessageFormatter GetMessageFormatter() {
            return new BinaryMessageFormatter();
        }

        #endregion
    }
    #endregion

    /// <summary>
    /// 池控制多线程消息队列插件类，继承池控制多线程插件类
    /// 只需要节点内设置设置属性，即可以使用池控制的多线程管理与消息队列处理，实现Custom方法实现数据处理逻辑
    /// 不需要处理init(),start(),stop()等操作，不需要注意CallProcessEvent传出状态
    /// 可以使用CallProcessEventSimple（数字，字符串）方法传出事件号与相关处理内容
    /// E 用来进行消息的解析，是消息队列信息实体的真实的类，例如：AMSMQPoolServer<JobApply2Entity>
    /// </summary>
    public abstract class AMSMQPoolServer : APoolServer {
        public string msmqPath, backmsmqPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatterStaregy">格式化策略对象</param>
        /// <param name="msmqPath">读取消息队列路径</param>
        /// <param name="backMsmqPath">备用消息队列路径</param>
        /// <param name="timeOut">等待超时</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public AMSMQPoolServer(MSMQPoolServerStaregy formatterStaregy, string msmqPath, string backMsmqPath, int timeOut, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : this(formatterStaregy, msmqPath, backMsmqPath, timeOut, createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, 0) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatterStaregy">格式化策略对象</param>
        /// <param name="msmqPath">读取消息队列路径</param>
        /// <param name="backMsmqPath">备用消息队列路径</param>
        /// <param name="timeOut">等待超时</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public AMSMQPoolServer(MSMQPoolServerStaregy formatterStaregy, string msmqPath, string backMsmqPath, int timeOut, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime) {

            formatter = formatterStaregy.GetMessageFormatter();
            #region 开启重试消息队列
            this.msmqPath = msmqPath;
            this.backmsmqPath = backMsmqPath;
            this.CallProcessEventSimple(LogType.RELEASE, 101, "消息队列：" + msmqPath);
            this.CallProcessEventSimple(LogType.RELEASE, 101, "失败消息队列：" + backmsmqPath);
            backQueue = new MessageQueueManager(backmsmqPath, formatter);
            #endregion

            #region 定义生产者超时时间
            timeSpan = TimeSpan.FromMilliseconds(timeOut);
            this.CallProcessEventSimple(LogType.RELEASE, 101, "生产者等待超时时间：" + timeOut);
            #endregion

            #region 开启消息队列
            queues = new MessageQueueManager[poolProcess.Creaters.Length];
            for (int w = 0; w < queues.Length; w++) {
                queues[w] = new MessageQueueManager(msmqPath, formatter);
            }
            #endregion
        }


        private MessageQueueManager backQueue;

        protected MessageQueueManager BackQueue {
            get { return backQueue; }
            set { backQueue = value; }
        }
        private TimeSpan timeSpan;
        private IMessageFormatter formatter = null;

        private MessageQueueManager[] queues;
        protected virtual void CloseQueue() {
            try {
                MessageQueue.ClearConnectionCache();
            } catch {
            }
            foreach (MessageQueueManager queue in queues)
                queue.Close();
            backQueue.Close();
        }

        protected void SendQueue(object key, MessageQueueManager queue, object value, string msmqName) {
            if (key != null)
                lock (key) {
                    SendQueue(queue, value, msmqName);
                } else
                SendQueue(queue, value, msmqName);
        }

        private void SendQueue(MessageQueueManager queue, object value, string msmqName) {
            try {
                queue.Send(value);
            } catch (Exception ex) {
                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
                    //读取信息超时      
                    this.CallProcessEventSimple(LogType.RELEASE, 311, msmqName + "发送连接超时错误");
                } else {
                    this.CallProcessEventSimple(LogType.RELEASE, 310, string.Format("{0}发送错误{1},已经重新建立连接!", msmqName, ex.ToString()));
                    throw ex;
                }
            }
        }

        private string backQueueKey = "";
        public void SendBackQueue(object obj) {
            SendQueue(backQueueKey, backQueue, obj, "BackQueue");
        }

        #region 获得对象与消费对象的方法封装
        protected override object Create(object sender, EventArg e) {
            ProtectThread thread = sender as ProtectThread;
            object v = null;
            try {
                v = queues[thread.GetNum()].Receive(timeSpan).Body;
            } catch (Exception ex) {
                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
                    //读取信息超时                    
                } else {
                    this.CallProcessEventSimple(LogType.RELEASE, 306, string.Format("奇怪异常 实体为空读取消息队列错误已经重新尝试打开{0}", ex.ToString()));
                }
                throw ex;
            }

            this.CallProcessEventSimple(LogType.DEBUG, 301, thread.GetNum() + "构造者创造一个对象!" + poolProcess.Pool.GetSize());
            return v;
        }



        protected override void RollBack(object sender, EventArg e, object value) {
            if (sender is CreaterPoolThread)
                queues[((ProtectThread)sender).GetNum()].Send(value);
            else
                queues[0].Send(value);
            this.CallProcessEventSimple(LogType.RELEASE, 303, "回滚一个对象!" + poolProcess.Pool.GetSize());
        }

        protected override void OnClose(object sender, EventArg e) {
            this.CloseQueue();
            base.OnClose(sender, e);
        }
        #endregion
    }
}
