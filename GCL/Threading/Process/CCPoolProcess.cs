using System;
using System.Collections;
using System.Text;
using GCL.Event;
using GCL.Collections;
using GCL.Collections.Pool;
using GCL.Common;
using GCL.Threading;
namespace GCL.Threading.Process {

    public class CCPoolProcess : AbstractProcess, IPoolValueFactory {

        #region 相关属性
        private CreaterAction _createaction;
        public CreaterAction Creater {
            get { return _createaction; }
        }

        private CustomerAction _customeraction;
        public CustomerAction Customer {
            get { return _customeraction; }
        }

        private CustomerAction _rollbackaction;
        public CustomerAction RollBack {
            get { return _rollbackaction; }
        }

        private CreaterPoolThread[] creaters;
        public CreaterPoolThread[] Creaters {
            get { return creaters; }
        }

        private Pool pool;
        public Pool Pool {
            get { return pool; }
        }

        public bool WaitCustomerClose {
            get { return this.pool.IsWaitCustomerClose(); }
            set { this.pool.SetWaitCustomerClose(value); }
        }

        /// <summary>
        /// 生产等待时间
        /// </summary>
        private int createWaitTime = 0;
        public int CreateWaitTime {
            get { return createWaitTime; }
            set { createWaitTime = value; }
        }

        #endregion

        private PoolRefreshThread poolRefresher;
        public PoolRefreshThread PoolRefresher {
            get { return poolRefresher; }
        }

        private int createrWaittime, customerWaittime, refreshWaittime;
        public CCPoolProcess(CreaterAction ca, CustomerAction cua, CustomerAction rba, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime) {
            this._createaction = ca;
            this._customeraction = cua;
            this._rollbackaction = rba;
            this.creaters = new CreaterPoolThread[createrNum];
            this.createrWaittime = createrWaittime;
            this.createWaitTime = createrWaittime;
            this.customerWaittime = customerWaittime;
            pool = new KeyValuePool(new StackPoolStaregy(), new DictionarySet(), customerNum, "", this);
            this.refreshWaittime = refreshWaittime;
            poolRefresher = new PoolRefreshThread(pool, (refreshWaittime / 2) > 30000 ? 30000 : (refreshWaittime / 2), new LRUPoolRefreshStaregy(TimeSpan.FromMilliseconds(refreshWaittime)));
            poolRefresher.FinallyDoEvent += new EventHandle(poolRefresher_FinallyDoEvent);
            poolRefresher.ThreadEvent += new EventHandle(poolRefresher_ThreadEvent);
            poolRefresher.Start();
            this.CanDisposeByReady = false;
        }

        #region 刷新线程的终止
        void poolRefresher_FinallyDoEvent(object sender, EventArg e) {
            //222事件刷新线程关闭
            this.CallProcessEventSimple(222, "刷新线程关闭！");
            if (isDispose)
                if (CheckStop())
                    this.CallProcessEventSimple(ProcessState.STOP);
        }

        void poolRefresher_ThreadEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(e.GetEventNumber(), e.ToStringOfPara(0));
        }

        #endregion

        public override void Init() {
            try {
                this.CallProcessEventSimple(ProcessState.INIT);
                this.CallProcessEventSimple(101, "获取构造者线程数" + this.creaters.Length);
                this.CallProcessEventSimple(101, "获取构造者线程等待时间" + this.createrWaittime + "毫秒");
                this.CallProcessEventSimple(101, "获取消费者线程池容量" + this.pool.GetCapacity());
                this.CallProcessEventSimple(101, "获取消费者线程等待时间" + this.customerWaittime + "毫秒");
                this.CallProcessEventSimple(101, "获取消费者线程池刷新时间" + this.refreshWaittime + "毫秒");
                for (int w = 0; w < this.creaters.Length; w++) {
                    creaters[w] = new CreaterPoolThread(_createaction, w, this.createrWaittime, pool);
                    creaters[w].ThreadEvent += new EventHandle(CCPoolProcess_ThreadEvent);
                    creaters[w].ExceptionThrowenEvent += new EventHandle(CCPoolProcess_ExceptionThrowenEvent);
                    creaters[w].FinallyDoEvent += new EventHandle(CCPoolProcess_FinallyDoEvent);
                    creaters[w].CreateWaitTime = this.CreateWaitTime;
                }
                this.CallProcessEventSimple(101, "构造者线程初始化完成！");
                this.CallProcessEventSimple(ProcessState.READY);
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "构造者线程初始化错误" + ex.ToString());
            }
        }

        #region 构造者线程事件处理
        void CCPoolProcess_ExceptionThrowenEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            ThreadEventArg te = e as ThreadEventArg;
            //203事件 构造器异常
            this.CallProcessEventSimple(203, new object[] { thread.GetNum() + "号构造器异常终止！" + te.GetException().ToString(), te.GetException() });
        }

        void CCPoolProcess_FinallyDoEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            if (Tool.IsEnable(thread.GetValue()))
                try {
                    RollBack(thread, e, thread.GetValue());
                } catch (Exception ex) {
                    //201事件 构造器对象回滚异常
                    this.CallProcessEventSimple(201, new object[] { "回滚未处理的对象出现问题! \r\n" + ex.ToString(), thread.GetValue() });
                }

            //202事件 构造器关闭
            this.CallProcessEventSimple(202, thread.GetNum() + "号构造器关闭！");
            if (CheckStop())
                this.CallProcessEventSimple(ProcessState.STOP);
        }

        void CCPoolProcess_ThreadEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            this.CallProcessEventSimple(e.GetEventNumber(), thread.GetNum() + "号构造器," + e.ToStringOfPara(0));
        }
        #endregion

        public override void Start() {
            try {
                foreach (CreaterPoolThread thread in creaters)
                    thread.Start();
                this.CallProcessEventSimple(ProcessState.START);
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "构造者线程启动错误" + ex.ToString());
            }
        }

        public override void Stop() {
            try {
                foreach (CreaterPoolThread thread in creaters)
                    thread.SetCanRun(false);
            } catch {
            }
        }

        public override void Dispose() {
            this.Stop();
            if (customerList == null)
                customerList = pool.Close();
            base.Dispose();
        }

        private IList customerList;
        protected override bool CheckStop() {
            bool createrClose = true;
            foreach (CreaterPoolThread thread in creaters)
                createrClose = createrClose && (thread == null || thread.IsFinally());
            if (customerList != null)
                for (IEnumerator em = customerList.GetEnumerator(); em.MoveNext(); ) {
                    CustomerPoolThread thread = em.Current as CustomerPoolThread;
                    createrClose = createrClose && thread.IsFinally();
                }


            if (!isDispose)
                return createrClose;
            else {
                return createrClose && poolRefresher.IsFinally();
            }
        }

        #region 消费者线程函数
        void thread_ThreadEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            this.CallProcessEventSimple(e.GetEventNumber(), thread.GetNum() + "号消费者," + e.ToStringOfPara(0));
        }

        void thread_FinallyDoEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            if (Tool.IsEnable(thread.GetValue()))
                try {
                    RollBack(sender, e, thread.GetValue());
                } catch {
                    //211事件 消费者对象回滚异常
                    this.CallProcessEventSimple(211, new object[] { "回滚未处理的对象出现问题!", thread.GetValue() });
                }
            //212事件 消费者关闭
            this.CallProcessEventSimple(212, thread.GetNum() + "号消费者关闭！");
            if (this.isDispose)
                if (this.CheckStop())
                    this.CallProcessEventSimple(ProcessState.STOP);
        }

        void thread_ExceptionThrowenEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            ThreadEventArg te = e as ThreadEventArg;
            //213事件 消费者异常
            this.CallProcessEventSimple(213, new object[] { thread.GetNum() + "号消费者异常终止！" + te.GetException().ToString(), te.GetException() });
        }
        #endregion

        #region IPoolValueFactory Members



        public object CreateObject(object e) {
            CustomerPoolThread thread = new CustomerPoolThread(_customeraction, DateTime.Now.Millisecond, this.customerWaittime, pool);
            thread.ExceptionThrowenEvent += new EventHandle(thread_ExceptionThrowenEvent);
            thread.FinallyDoEvent += new EventHandle(thread_FinallyDoEvent);
            thread.ThreadEvent += new EventHandle(thread_ThreadEvent);
            thread.Start();
            return thread;
        }



        public void CloseObject(object obj) {
            CustomerPoolThread thread = obj as CustomerPoolThread;
            thread.SetCanRun(false);
        }

        #endregion
    }
}