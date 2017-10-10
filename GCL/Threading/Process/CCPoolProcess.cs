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

        #region �������
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
        /// �����ȴ�ʱ��
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

        #region ˢ���̵߳���ֹ
        void poolRefresher_FinallyDoEvent(object sender, EventArg e) {
            //222�¼�ˢ���̹߳ر�
            this.CallProcessEventSimple(222, "ˢ���̹߳رգ�");
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
                this.CallProcessEventSimple(101, "��ȡ�������߳���" + this.creaters.Length);
                this.CallProcessEventSimple(101, "��ȡ�������̵߳ȴ�ʱ��" + this.createrWaittime + "����");
                this.CallProcessEventSimple(101, "��ȡ�������̳߳�����" + this.pool.GetCapacity());
                this.CallProcessEventSimple(101, "��ȡ�������̵߳ȴ�ʱ��" + this.customerWaittime + "����");
                this.CallProcessEventSimple(101, "��ȡ�������̳߳�ˢ��ʱ��" + this.refreshWaittime + "����");
                for (int w = 0; w < this.creaters.Length; w++) {
                    creaters[w] = new CreaterPoolThread(_createaction, w, this.createrWaittime, pool);
                    creaters[w].ThreadEvent += new EventHandle(CCPoolProcess_ThreadEvent);
                    creaters[w].ExceptionThrowenEvent += new EventHandle(CCPoolProcess_ExceptionThrowenEvent);
                    creaters[w].FinallyDoEvent += new EventHandle(CCPoolProcess_FinallyDoEvent);
                    creaters[w].CreateWaitTime = this.CreateWaitTime;
                }
                this.CallProcessEventSimple(101, "�������̳߳�ʼ����ɣ�");
                this.CallProcessEventSimple(ProcessState.READY);
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "�������̳߳�ʼ������" + ex.ToString());
            }
        }

        #region �������߳��¼�����
        void CCPoolProcess_ExceptionThrowenEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            ThreadEventArg te = e as ThreadEventArg;
            //203�¼� �������쳣
            this.CallProcessEventSimple(203, new object[] { thread.GetNum() + "�Ź������쳣��ֹ��" + te.GetException().ToString(), te.GetException() });
        }

        void CCPoolProcess_FinallyDoEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            if (Tool.IsEnable(thread.GetValue()))
                try {
                    RollBack(thread, e, thread.GetValue());
                } catch (Exception ex) {
                    //201�¼� ����������ع��쳣
                    this.CallProcessEventSimple(201, new object[] { "�ع�δ����Ķ����������! \r\n" + ex.ToString(), thread.GetValue() });
                }

            //202�¼� �������ر�
            this.CallProcessEventSimple(202, thread.GetNum() + "�Ź������رգ�");
            if (CheckStop())
                this.CallProcessEventSimple(ProcessState.STOP);
        }

        void CCPoolProcess_ThreadEvent(object sender, EventArg e) {
            CreaterPoolThread thread = sender as CreaterPoolThread;
            this.CallProcessEventSimple(e.GetEventNumber(), thread.GetNum() + "�Ź�����," + e.ToStringOfPara(0));
        }
        #endregion

        public override void Start() {
            try {
                foreach (CreaterPoolThread thread in creaters)
                    thread.Start();
                this.CallProcessEventSimple(ProcessState.START);
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "�������߳���������" + ex.ToString());
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

        #region �������̺߳���
        void thread_ThreadEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            this.CallProcessEventSimple(e.GetEventNumber(), thread.GetNum() + "��������," + e.ToStringOfPara(0));
        }

        void thread_FinallyDoEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            if (Tool.IsEnable(thread.GetValue()))
                try {
                    RollBack(sender, e, thread.GetValue());
                } catch {
                    //211�¼� �����߶���ع��쳣
                    this.CallProcessEventSimple(211, new object[] { "�ع�δ����Ķ����������!", thread.GetValue() });
                }
            //212�¼� �����߹ر�
            this.CallProcessEventSimple(212, thread.GetNum() + "�������߹رգ�");
            if (this.isDispose)
                if (this.CheckStop())
                    this.CallProcessEventSimple(ProcessState.STOP);
        }

        void thread_ExceptionThrowenEvent(object sender, EventArg e) {
            CustomerPoolThread thread = sender as CustomerPoolThread;
            ThreadEventArg te = e as ThreadEventArg;
            //213�¼� �������쳣
            this.CallProcessEventSimple(213, new object[] { thread.GetNum() + "���������쳣��ֹ��" + te.GetException().ToString(), te.GetException() });
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