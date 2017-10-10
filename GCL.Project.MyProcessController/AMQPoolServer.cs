using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.IO.Log;
using GCL.Threading.Process;
using GCL.Event;
using GCL.Common;
using GCL.Threading;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 消息队列接口
    /// </summary>
    public interface IMessageQueue {
        /// <summary>
        /// 重设连接
        /// </summary>
        void ReSet();
        /// <summary>
        /// 接收 如果timeSpan为空，那么一直等待
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        object Receive(TimeSpan timeSpan);

        /// <summary>
        /// 发送对象
        /// </summary>
        /// <param name="obj"></param>
        void Send(object obj);

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        /// <summary>
        /// 主要用于提供描述 比如消息队列地址等等信息
        /// </summary>
        /// <returns></returns>
        string GetDescription();
    }

    /// <summary>
    /// 生产工厂接口
    /// </summary>
    public interface IMessageQueueFactory {
        IMessageQueue GenerateMessageQueue();
        void SetQueue(IMessageQueue queue);
    }

    /// <summary>
    /// 这个错误外层不会做ReSet处理，仅仅提示信息，但除此以外的错误会调用消息队列ReSet方法重置队列！
    /// </summary>
    public class MessageQueueCommonException : Exception {
        public MessageQueueCommonException(string desc, Exception innerException) : base(desc, innerException) { }
    }
    /// <summary>
    /// 通用消息队列处理程序！
    /// </summary>
    public abstract class AMQPoolServer : APoolServer {

        /// <summary>
        /// 备份消息队列
        /// </summary>
        private IMessageQueue backQueue;
        /// <summary>
        /// 接收用消息队列数组
        /// </summary>
        private IMessageQueue[] queues;

        /// <summary>
        /// 消息队列工厂
        /// </summary>
        private IMessageQueueFactory queueFactory;
        private TimeSpan timeSpan;

        public AMQPoolServer(IMessageQueueFactory factory, IMessageQueue backQueue, int timeOut, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : this(factory, backQueue, timeOut, createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, 0) {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory">消息队列工厂（接收用）</param>
        /// <param name="backQueue">备份消息队列</param>
        /// <param name="timeOut">生产超时等待时间</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public AMQPoolServer(IMessageQueueFactory factory, IMessageQueue backQueue, int timeOut, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime) {
            this.queueFactory = factory;
            this.backQueue = backQueue;
            timeSpan = TimeSpan.FromMilliseconds(timeOut);
        }

        public override void Init() {

            try {
                #region 重试消息队列提示
                this.CallProcessEventSimple(LogType.RELEASE, 101, "消息队列：");
                this.CallProcessEventSimple(LogType.RELEASE, 101, "失败消息队列：" + backQueue.GetDescription());
                #endregion

                #region 定义生产者超时时间
                this.CallProcessEventSimple(LogType.RELEASE, 101, "生产者等待超时时间：" + timeSpan.Ticks);
                #endregion

                #region 开启消息队列
                queues = new IMessageQueue[poolProcess.Creaters.Length];
                for (int w = 0; w < queues.Length; w++) {
                    queues[w] = queueFactory.GenerateMessageQueue();
                }
                #endregion
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "构造消息队列出现错误");
            }
            base.Init();
        }

        protected virtual void CloseQueue() {
            foreach (IMessageQueue queue in queues)
                queue.Close();
            backQueue.Close();
        }

        protected void SendQueue(object key, IMessageQueue queue, object value, string queueName) {
            if (key != null)
                lock (key) {
                    SendQueue(queue, value, queueName);
                } else
                SendQueue(queue, value, queueName);
        }

        private void SendQueue(IMessageQueue queue, object value, string queueName) {
            try {
                queue.Send(value);
            } catch (MessageQueueCommonException ex) {
                this.CallProcessEventSimple(LogType.RELEASE, 311, string.Format("{0}发生普通错误{1}{2}", queueName, Tool.LineSeparator, ex.ToString()));
                throw ex;
            } catch (Exception ex) {
                this.CallProcessEventSimple(LogType.RELEASE, 310, string.Format("{0}发生严重错误,将重新建立连接:{1}{2}", queueName, Tool.LineSeparator, ex.ToString()));
                try {
                    queue.ReSet();
                } catch (Exception ex2) {
                    this.CallProcessEventSimple(LogType.RELEASE, 312, string.Format("{0}ReSet发生严重错误:{1}{2}", queueName, Tool.LineSeparator, ex2.ToString()));
                    throw ex2;
                }
                throw ex;
            }
        }

        private object backQueueKey = DateTime.Now;
        public void SendBackQueue(object obj) {
            SendQueue(backQueueKey, backQueue, obj, "BackQueue");
        }

        #region 获得对象与消费对象的方法封装
        protected override object Create(object sender, EventArg e) {
            ProtectThread thread = sender as ProtectThread;
            object v = null;
            try {
                v = queues[thread.GetNum()].Receive(timeSpan);
            } catch (MessageQueueCommonException ex) {
                throw ex;
            } catch (Exception ex) {
                this.CallProcessEventSimple(LogType.RELEASE, 306, string.Format("构造者{0}发生严重错误,将重新建立连接:{1}{2}", thread.GetNum(), Tool.LineSeparator, ex.ToString()));
                try {
                    queues[thread.GetNum()].ReSet();
                } catch (Exception ex2) {
                    this.CallProcessEventSimple(LogType.RELEASE, 307, string.Format("构造者{0}ReSet发生严重错误:{1}{2}", thread.GetNum(), Tool.LineSeparator, ex2.ToString()));
                    throw ex2;
                }
                throw ex;
            }
            this.CallProcessEventSimple(LogType.DEBUG, 301, thread.GetNum() + "构造者创造一个对象!" + poolProcess.Pool.GetSize());
            return v;
        }



        protected override void RollBack(object sender, EventArg e, object value) {
            if (sender is CreaterPoolThread)
                this.SendQueue(queues[((ProtectThread)sender).GetNum()], value, "RollBack");
            else
                this.SendQueue(queues[0], value, "RollBack");
            this.CallProcessEventSimple(LogType.RELEASE, 303, "回滚一个对象!" + poolProcess.Pool.GetSize());
        }

        protected override void OnClose(object sender, EventArg e) {
            this.CloseQueue();
            base.OnClose(sender, e);
        }
        #endregion
    }
}
