using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 使用.Net封装的消息队列对象
    /// </summary>
    public class NetMessageQueue : IMessageQueue {

        private IMessageFormatter formatter = null;
        private string mqPath;
        private MessageQueue queue;

        public MessageQueue MessageQueue {
            get { return queue; }
        }

        public NetMessageQueue(string mqPath, IMessageFormatter formatter) {
            this.mqPath = mqPath;
            this.formatter = formatter;
            this.BuildQueue();
        }

        private MessageQueue BuildQueue() {
            if (queue == null) {
                queue = GenerateQueue();
            }
            return queue;
        }

        /// <summary>
        /// 用于重新生成一个.Net消息队列对象
        /// </summary>
        /// <returns></returns>
        protected virtual MessageQueue GenerateQueue() {
            MessageQueue que = new MessageQueue(mqPath);
            que.Formatter = formatter;
            que.DefaultPropertiesToSend.Recoverable = true;
            return que;
        }

        #region IMessageQueue Members

        public void Close() {
            //以下这句是否可以频繁调用表示怀疑
            try {
                MessageQueue.ClearConnectionCache();
            } catch {
            }
            try {
                lock (queue) {
                    queue.Close();
                }
            } catch {
            } finally {
                queue = null;
            }
        }

        public virtual string GetDescription() {
            return this.mqPath;
        }

        public void ReSet() {
            this.Close();
            this.BuildQueue();
        }

        public object Receive(TimeSpan timeSpan) {
            try {
                return this.queue.Receive(timeSpan).Body;
            } catch (Exception ex) {
                if (ex is MessageQueueException && ((MessageQueueException)ex).MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) {
                    //读取信息超时    
                    throw new MessageQueueCommonException("读取信息超时", ex);
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
                    throw new MessageQueueCommonException("发送信息超时", ex);
                } else
                    lock (queue) {
                        this.ReSet();
                    }
                throw ex;
            }
        }

        #endregion
    }

    /// <summary>
    /// 用于重复产生某个消息队列的消息队列对象实例
    /// </summary>
    public class NetMessageQueueFactory : IMessageQueueFactory {
        #region IMessageQueueFactory Members

        private string mqPath;
        private IMessageFormatter formatter;
        public NetMessageQueueFactory(string mqPath, IMessageFormatter formatter) {
            this.mqPath = mqPath;
            this.formatter = formatter;
        }
        public IMessageQueue GenerateMessageQueue() {
            return new NetMessageQueue(mqPath, formatter);
        }

        public void SetQueue(IMessageQueue queue) {
            queue.Close();
        }
        #endregion
    }
}
