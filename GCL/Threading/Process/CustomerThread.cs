using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections;

namespace GCL.Threading.Process {

    /// <summary>
    /// 生成对象的代理声明 需要线程安全的
    /// </summary>
    /// <returns></returns>
    public delegate void CustomobjectDel(object sender, EventArg e, object data);

    /// <summary>
    /// 消费者线程
    /// </summary>
    public class CustomerThread : CircleThread {

        /// <summary>
        /// 用于生成对象的代理实例
        /// </summary>
        private CustomobjectDel customobject;

        /// <summary>
        /// 需要放入的线程 如果出现满错误 会定时等待。
        /// </summary>
        private LimitQueue queue;

        /// <summary>
        /// 等待时间 毫秒 默认半秒钟
        /// </summary>
        private int waitTime = 500;

        /// <summary>
        /// 判断是否有等待时间
        /// </summary>
        /// <returns></returns>
        public bool HasWaitTime() {
            return this.waitTime > -1;
        }


        /// <summary>
        /// 返回 waitTime。
        /// </summary>
        /// <returns></returns>
        public int GetWaitTime() {
            return waitTime;
        }
        /// <summary>
        /// 等待时间
        /// </summary>
        public int WaitTime {
            get {
                return GetWaitTime();
            }
        }

        /// <summary>
        /// CreateobjectDel对象，线程号，限制队列对象 生成新的创建者对象 默认等待半秒钟
        /// </summary>
        /// <param name="cod">产生获得产品对象代理方法</param>
        /// <param name="num">线程号</param>
        /// <param name="queue">限制队列对象</param>
        public CustomerThread(CustomobjectDel cod, int num, LimitQueue queue)
            : this(cod, num, queue, 500) {
        }

        /// <summary>
        /// CreateobjectDel对象，线程号，限制队列对象，等待时间 生成新的创建者对象
        /// </summary>
        /// <param name="cod">产生获得产品对象代理方法</param>
        /// <param name="num">线程号</param>
        /// <param name="queue">限制队列对象</param>
        /// <param name="waitTime">等待时间</param>
        public CustomerThread(CustomobjectDel cod, int num, LimitQueue queue, int waitTime)
            : base() {
            this.protRun = this.CustomTRun;
            this.customobject = cod;
            this.SetNum(num);
            this.queue = queue;
            this.waitTime = waitTime;
        }

        /// <summary>
        /// 用于消费对象的过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CustomTRun(object sender, EventArg e) {
            object data = null;

            while (this.IsCanRun())
                try {
                    lock (this.queue) {
                        data = this.queue.Peek();
                        if (Tool.IsEnable(e))
                            this.queue.Dequeue();
                    }
                    break;
                } catch {
                    if (this.HasWaitTime())
                        Tool.ObjectWait(this.queue, waitTime);
                    else
                        Tool.ObjectWait(queue);
                }

            if (!Tool.IsEnable(data))
                this.SetCanRun(false);
            else
                this.Customobject(sender, e, data);

        }

        /// <summary>
        /// 允许覆盖以处理对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="data"></param>
        public virtual void Customobject(object sender, EventArg e, object data) {
            this.customobject(sender, e, data);
        }
    }
}
