using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections.Pool;
namespace GCL.Threading.Process {
    public delegate object CreaterAction(object sender, Event.EventArg e);

    /// <summary>
    /// 池进程使用的池生产者线程
    /// </summary>
    public class CreaterPoolThread : CircleThread {
        private CreaterAction _createaction;
        public CreaterAction Creater {
            get { return _createaction; }
        }

        private object value = null;
        public void SetValue(object data) {
            if (!Tool.IsEnable(value))
                value = data;
            else
                throw new InvalidOperationException("其中的对象还没有处理完毕！");
        }
        public object GetValue() {
            return value;
        }
        public object Value {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        private Pool pool;
        protected internal void SetPool(Pool pool) {
            if (!Tool.IsEnable(this.pool) || this.pool.IsClose()) {
                this.pool = pool;
                this.pool.OnGetLimit += new EEventHandle<PoolEventArg>(pool_OnGetLimit);
                this.pool.OnSet += new EEventHandle<PoolEventArg>(pool_OnSet);
            } else
                throw new InvalidOperationException("池可用不可以更新!");
        }
        public CreaterPoolThread(CreaterAction del, int num, int waitTime, Pool pool) {
            this.SetRun(new Run(this.TimerAction));
            this.SetNum(num);
            this.SetPool(pool);
            this.CreateWaitTime = waitTime;
            this._createaction = del;
        }

        void pool_OnSet(object sender, PoolEventArg e) {
            limited = false;
        }

        void pool_OnGetLimit(object sender, PoolEventArg e) {
            limited = true;
        }

        /// <summary>
        /// 生产等待时间
        /// </summary>
        private int createWaitTime = 0;
        public int CreateWaitTime {
            get { return createWaitTime; }
            set { createWaitTime = value; }
        }

        private bool limited = false;
        public virtual void TimerAction(object sender, Event.EventArg e) {
            try {
                while (this.IsCanRun()) {
                    if (!Tool.IsEnable(value)) {
                        try
                        {
                            this.SetValue(_createaction(sender, e));
                        }catch{}
                    }

                    if (Tool.IsEnable(value) && limited) {
                    } else if (Tool.IsEnable(value)) {
                        CustomerPoolThread thread = pool.Get() as CustomerPoolThread;
                        thread.SetValue(this.value);
                        this.value = null;
                    }
                    else
                        if (CreateWaitTime > 0)
                            Tool.ObjectSleep(CreateWaitTime);
                }
            } catch (Exception ex) {
#if DEBUG
                this.CallThreadEventSimple(204, new object[] { ex.ToString(), ex });
#endif
            }
        }
    }
}
