using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections;
using GCL.Threading;

namespace GCL.Threading.Process {

    /// <summary>
    /// 生产者线程
    /// </summary>
    public class CreaterThread : CircleThread {

        /// <summary>
        /// 用于生成对象的代理实例
        /// </summary>
        private CreaterAction createobject;

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
        /// CreaterAction对象，线程号，限制队列对象 生成新的创建者对象 默认等待半秒钟
        /// </summary>
        /// <param name="cod">产生获得产品对象代理方法</param>
        /// <param name="num">线程号</param>
        /// <param name="queue">限制队列对象</param>
        public CreaterThread(CreaterAction cod, int num, LimitQueue queue)
            : this(cod, num, queue, 500) {
        }

        /// <summary>
        /// CreaterAction对象，线程号，限制队列对象，等待时间 生成新的创建者对象
        /// </summary>
        /// <param name="cod">产生获得产品对象代理方法</param>
        /// <param name="num">线程号</param>
        /// <param name="queue">限制队列对象</param>
        /// <param name="waitTime">等待时间</param>
        public CreaterThread(CreaterAction cod, int num, LimitQueue queue, int waitTime)
            : base() {
            this.protRun = this.CreateTRun;
            this.createobject = cod;
            this.SetNum(num);
            this.queue = queue;
            this.waitTime = waitTime;
        }

        /// <summary>
        /// 用于创建对象的过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CreateTRun(object sender, EventArg e) {
            object data = this.Createobject(sender, e);
            while (this.IsCanRun())
                try {
                    this.queue.Enqueue(data);
                    break;
                } catch {
                    if (this.HasWaitTime())
                        Tool.ObjectWait(this.queue, waitTime);
                    else
                        Tool.ObjectWait(queue);
                }

            if (!Tool.IsEnable(data))
                this.SetCanRun(false);
        }

        /// <summary>
        /// 允许覆盖载以生成对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual object Createobject(object sender, EventArg e) {
            return this.createobject(sender, e);
        }
    }
}
