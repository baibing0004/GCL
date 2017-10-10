using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Event;
using GCL.Common;

namespace GCL.Threading {
    public class CircleThread:ProtectThread,IDisposable {

        public CircleThread(Run protRun)
            : base(protRun) {
            this.FinalRunEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// 只在继承时有效 但是需要自定义protRun对象
        /// </summary>
        public CircleThread()
            : base() {
            this.FinalRunEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// 是否可以继续运行
        /// </summary>
        private bool isCanRun = false;

        /// <summary>
        /// 是否不管用户是否完成操作都立刻终止外部程序
        /// </summary>
        private bool syn = false;

        /// <summary>
        /// 是否是第一次执行
        /// </summary>
        private bool first = false;

        /// <summary>
        /// 用于锁定是否可以继续运行的对象
        /// </summary>
        private object key = DateTime.Now;

        /// <summary>
        /// 短期记载错误
        /// </summary>
        private Exception exThrowen = null;

        /// <summary>
        /// 内部线程使用方法 记录抛出的错误并唤醒主线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pt_ExceptionThrowenEvent(object sender,EventArg e) {
            this.exThrowen = ((ThreadEventArg)e).GetException();
            Tool.ObjectPulseAll(key);
        }

        /// <summary>
        /// 内部线程使用方法 唤醒主线程
        /// </summary>
        private void _pt_FinallyDoEvent(object sender,EventArg e) {
            Tool.ObjectPulseAll(key);
        }

        /// <summary>
        /// 内部线程使用方法 封装 并替换运行内部线程的主要执行方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pt_protRun(object sender,EventArg e) {
            this.CircleRun();
        }


        /// <summary>
        /// 覆盖原保护方法
        /// </summary>
        protected override void ProtectedRun() {
            try {

                this.first = true;
                this.isCanRun = true;
                ProtectThread _pt = new ProtectThread(this._pt_protRun);

                //定义错误事件接收并自动触醒key使主线程继续运行
                _pt.ExceptionThrowenEvent += new EventHandle(_pt_ExceptionThrowenEvent);

                //定义最终事件接收机制自动触醒key使主线程继续运行
                _pt.FinallyDoEvent += new EventHandle(this._pt_FinallyDoEvent);

                while (this.isCanRun) {
                    if (syn) {
                        exThrowen = null;
                        Thread _t = _pt.CreateThread();
                        lock (key) {
                            _t.IsBackground = true;
                            _t.Start();
                            Tool.ObjectWait(key);
                            this.first = false;
                        }

                        if (this.exThrowen != null)
                            throw exThrowen;

                        if (_t.ThreadState == ThreadState.Running) {
                            try {
                                _t.Interrupt();
                            } catch {
                            }
                            try {
                                _t.Abort();
                            } catch {
                            }
                        }
                    } else {
                        this.CircleRun();
                        this.first = false;
                    }
                }
                this.FinalRun();
            } catch (Exception ex) {
                this.isCanRun = false;
                throw ex;
            }
        }

        /// <summary>
        /// 允许重载以使用protRun代理对象
        /// </summary>
        public virtual void CircleRun() {
            base.protRun(this,new ThreadEventArg(this));
        }

        /// <summary>
        /// 是否是第一次循环
        /// </summary>
        /// <returns></returns>
        public bool IsFirst() {
            return first;
        }

        /// <summary>
        /// 声明最终执行事件
        /// </summary>
        public event EventHandle FinalRunEvent;

        /*
         * 建议覆盖 用于线程正常停止时的处理任务，比如对某些类的变量的消除！默认作打印输出处理！
         * 
         * @throws Exception
         */

        /// <summary>
        /// 建议覆盖 用于线程正常停止时的处理任务，比如对某些类的变量的消除！默认作打印输出处理！
        /// 与FinallyDo方法的区别在于其可以抛出错误 并被Throwen方法捕捉
        /// </summary>
        protected void FinalRun() {
            Console.WriteLine(this.GetType().Name + " run end!");
            EventArg.CallEventSafely(FinalRunEvent,this,new ThreadEventArg(this));
        }

        /// <summary>
        /// 是否同步关闭
        /// </summary>
        /// <returns>返回 syn</returns>
        public bool IsSynClose() {
            return syn;
        }

        /*
          * 是否要求同步关闭
          * 
          * @param syn
          *            要设置的 syn。
          */

        /// <summary>
        /// 是否要求同步关闭
        /// </summary>
        /// <param name="syn">要设置的</param>
        public void SetSynClose(bool syn) {
            this.syn = syn;
        }

        /// <summary>
        /// .Net写法 属性是否可以继续执行 如果要中止线程 希望使用这个属性为False 这样可以顺序处理 finalRun 和 finallyDo
        /// </summary>
        public bool CanRun {
            get {
                return IsCanRun();
            }
            set {
                SetCanRun(value);
            }
        }

        /// <summary>
        /// 是否可以执行
        /// </summary>
        /// <returns>返回是否可以执行</returns>
        public bool IsCanRun() {
            return this.isCanRun;
        }

        /// <summary>
        /// 设置是否可以继续执行 如果要中止线程 希望使用这个方法
        /// </summary>
        /// <param name="canRun">是否可以继续执行</param>
        public void SetCanRun(bool canRun) {            
            this.isCanRun = canRun;
            if(!canRun)
                if(this.syn) {
                    Tool.ObjectPulseAll(key);
                }
        }

        #region IDisposable Members

        public void Dispose() {
            this.SetCanRun(false);
        }

        #endregion    
    
        ~CircleThread() {
            this.Dispose();
        }
    }
}
