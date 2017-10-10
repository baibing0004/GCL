using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Event;
using GCL.Common;

namespace GCL.Threading {

    /// <summary>
    /// ����������ִ���¼�
    /// </summary>
    public delegate void Run(object sender, EventArg e);

    public class ThreadEventArg : EventArg {

        /**
     * 
     */
        public ThreadEventArg()
            : base() {
        }

        /**
         * @param level
         * @param eventNum
         * @param size
         */
        public ThreadEventArg(EventLevel level, int eventNum, int size)
            : base(level, eventNum, size) {
        }

        /**
         * @param level
         * @param eventNum
         * @param obj
         * @param allowSet
         */
        public ThreadEventArg(EventLevel level, int eventNum, object obj, bool allowSet)
            : base(level, eventNum, obj, allowSet) {
        }

        /**
         * @param level
         * @param eventNum
         * @param obj
         */
        public ThreadEventArg(EventLevel level, int eventNum, object obj)
            : base(level, eventNum, obj) {

        }

        /**
         * @param level
         * @param eventNum
         * @param objs
         * @param allowSet
         */
        public ThreadEventArg(EventLevel level, int eventNum, object[] objs,
                bool allowSet)
            : base(level, eventNum, objs, allowSet) {

        }

        /**
         * @param level
         * @param eventNum
         * @param objs
         */
        public ThreadEventArg(EventLevel level, int eventNum, object[] objs)
            : base(level, eventNum, objs) {

        }

        /**
         * @param level
         * @param size
         */
        public ThreadEventArg(EventLevel level, int size)
            : base(level, size) {

        }

        /**
         * @param level
         * @param obj
         * @param allowSet
         */
        public ThreadEventArg(EventLevel level, object obj, bool allowSet)
            : base(level, obj, allowSet) {

        }

        /**
         * @param level
         * @param obj
         */
        public ThreadEventArg(EventLevel level, object obj)
            : base(level, obj) {

        }

        /**
         * @param level
         * @param objs
         * @param allowSet
         */
        public ThreadEventArg(EventLevel level, object[] objs, bool allowSet)
            : base(level, objs, allowSet) {

        }

        /**
         * @param level
         * @param objs
         */
        public ThreadEventArg(EventLevel level, object[] objs)
            : base(level, objs) {

        }

        /**
         * @param level
         */
        public ThreadEventArg(EventLevel level)
            : base(level) {

        }

        /**
         * @param eventNum
         * @param size
         */
        public ThreadEventArg(int eventNum, int size)
            : base(eventNum, size) {

        }

        /**
         * @param eventNum
         * @param obj
         * @param allowSet
         */
        public ThreadEventArg(int eventNum, object obj, bool allowSet)
            : base(eventNum, obj, allowSet) {

        }

        /**
         * @param eventNum
         * @param obj
         */
        public ThreadEventArg(int eventNum, object obj)
            : base(eventNum, obj) {

        }

        /**
         * @param eventNum
         * @param objs
         * @param allowSet
         */
        public ThreadEventArg(int eventNum, object[] objs, bool allowSet)
            : base(eventNum, objs, allowSet) {

        }

        /**
         * @param eventNum
         * @param objs
         */
        public ThreadEventArg(int eventNum, object[] objs)
            : base(eventNum, objs) {

        }

        /**
         * @param size
         */
        public ThreadEventArg(int size)
            : base(size) {

        }

        /**
         * @param obj
         * @param allowSet
         */
        public ThreadEventArg(object obj, bool allowSet)
            : base(obj, allowSet) {

        }

        /**
         * @param obj
         */
        public ThreadEventArg(object obj)
            : base(obj) {

        }

        /**
         * @param objs
         * @param allowSet
         */
        public ThreadEventArg(object[] objs, bool allowSet)
            : base(objs, allowSet) {

        }

        /**
         * @param objs
         */
        public ThreadEventArg(object[] objs)
            : base(objs) {

        }

        /**
         * 
         * @param ex
         * @param td
         * @param e
         */
        protected ThreadEventArg(Exception ex, ProtectThread td, EventArg e)
            : base(e) {
            this.ex = ex;
            this.thread = td;
        }

        public ThreadEventArg(Exception ex, ProtectThread td) {
            this.ex = ex;
            this.thread = td;
        }

        private Exception ex;

        private ProtectThread thread;

        /**
         * @return ex
         */
        public Exception GetException() {
            return ex;
        }

        /**
	 * @param ex
	 *            Ҫ���õ� ex
	 */
        protected void SetException(Exception ex) {
            this.ex = ex;
        }

        /**
         * @return thread
         */
        public ProtectThread GetThread() {
            return thread;
        }

        /**
         * @param thread
         *            Ҫ���õ� thread
         */
        protected void SetThread(ProtectThread thread) {
            this.thread = thread;
        }
    }

    /// <summary>
    /// ��Ҫ�����ṩ���߳������������Ĳ�׽����
    /// FinallyDo���������ʱ�ܻ����е�һ�δ���������ڴ���������
    /// ����ͨ��ExceptionThrowen�������ݸ��̣߳�Ĭ�Ϸ�����ֻ��������������������滻��������Դ�����󵫲������������߳� 
    /// </summary>
    public class ProtectThread {

        protected Run protRun;
        protected string id = Guid.NewGuid().ToString().Replace("-", "");
        //����߳�ID
        public string ID { get { return id; } }

        public void SetRun(Run runMethod) {
            if (!Tool.IsEnable(runMethod))
                throw new Exception("Null method �մ�������");
            if (Tool.IsEnable(this.protRun))
                throw new InvalidOperationException("��������Ѿ����ã�������������!");
            this.protRun = runMethod;
        }

        /// <summary>
        /// �̺߳�
        /// </summary>
        private int num = -1;

        /// <summary>
        /// -1ʱΪδ����
        /// </summary>
        /// <returns></returns>
        public int GetNum() {
            return num;
        }

        public void SetNum(int data) {
            if (this.num != -1)
                throw new InvalidOperationException("��ֵ�Ѿ�����");
            else
                this.num = data;
        }

        /// <summary>
        /// �̺߳� ֻ������һ��
        /// </summary>
        public int Num {
            get {
                return GetNum();
            }
            set {
                SetNum(value);
            }
        }
        /// <summary>
        /// �Զ��崦��protRun����
        /// </summary>
        /// <param name="_protRun"></param>
        public ProtectThread(Run _protRun)
            : this() {
            this.SetRun(_protRun);
        }

        /// <summary>
        /// ֻ�ڼ̳�ʱ��Ч ������Ҫ�Զ���proRun����
        /// </summary>
        public ProtectThread() {
            this.ExceptionThrowenEvent += new EventHandle(EventArg._EventHandleDefault);
            this.FinallyDoEvent += new EventHandle(EventArg._EventHandleDefault);
            this.ThreadEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// �����߳�ִ�з���
        /// </summary>
        public virtual void run() {
            try {
                isFinally = false;
                this.ProtectedRun();
            } catch (Exception ex) {
                this.ExceptionThrowen(ex);
            } finally {
                try {
                    isFinally = true;
                    this.FinallyDo();
                } catch {
                }
            }
        }

        /// <summary>
        /// ���鸲�� �Դ�����в�׽���̷߳���
        /// </summary>
        protected virtual void ProtectedRun() {
            this.protRun(this, new ThreadEventArg(this));
        }

        /// <summary>
        /// �׳����� �����������¼� ���FinallyDo�������߳�ֹͣ
        /// </summary>
        public event EventHandle ExceptionThrowenEvent;

        /// <summary>
        /// ���鸲�� ���Ҫ��һЩ��ı������д�����Ҫ��д���������Ĭ������ӡ�������
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void ExceptionThrowen(Exception ex) {
            Console.WriteLine(this.GetType().FullName + " run Exception:"
                    + ex.Message);
            EventArg.CallEventSafely(ExceptionThrowenEvent, this, new ThreadEventArg(ex, this));
        }

        /// <summary>
        /// ֪ͨ������ɹرղ�����
        /// </summary>
        public event EventHandle FinallyDoEvent;

        private bool isFinally = true;

        public bool IsFinally() {
            return isFinally;
        }

        /// <summary>
        /// ���鸲�� ���Ҫ��һЩ��ı������д�����Ҫ��д���������Ĭ������ӡ�������
        /// </summary>
        protected virtual void FinallyDo() {
            Console.WriteLine(this.GetType().FullName + " run finally");
            EventArg.CallEventSafely(FinallyDoEvent, this, new ThreadEventArg(this));
        }

        /// <summary>
        /// ��ȫ�����߳� ��֧�ֶ�������
        /// </summary>
        public virtual void Start() {
            if (IsFinally()) {
                Tool.StopThread(this.Thread);
                this.CreateThread().Start();
            }
        }

        /// <summary>
        /// �����߳� ������Thread����Ϊ���һ�����е��߳�
        /// </summary>
        /// <returns></returns>
        public virtual Thread CreateThread() {
            Thread thread = new Thread(new ThreadStart(run));
            this.SetThread(thread);
            return thread;
        }

        /// <summary>
        /// ���һ���̶߳���
        /// </summary>
        private Thread thread;

        /// <summary>
        /// ����̶߳���
        /// </summary>
        /// <returns></returns>
        public Thread GetThread() {
            return this.thread;
        }

        /// <summary>
        /// �����̶߳���
        /// </summary>
        /// <param name="thread"></param>
        protected void SetThread(Thread thread) {
            this.thread = thread;
        }

        /// <summary>
        /// ���������һ���̶߳���
        /// </summary>
        public Thread Thread {
            get {
                return GetThread();
            }
        }

        /// <summary>
        /// ���������е��¼� ֻ�����ڴ�����Ϣ֮��
        /// </summary>
        public event EventHandle ThreadEvent;
        protected virtual void CallThreadEventSimple(EventArg e) {
            EventArg.CallEventSafely(this.ThreadEvent, this, e);
        }
        /// <summary>
        /// ���㴦���� ˵���¼����Ѿ�����һ������
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallThreadEventSimple(int eventNum, object obj) {
            this.CallThreadEventSimple(new EventArg(eventNum, obj));
        }

        /// <summary>
        ///  ���㴦���� ˵���¼����Ѿ�����һ�����
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallThreadEventSimple(int eventNum, object[] objs) {
            this.CallThreadEventSimple(new EventArg(eventNum, objs));
        }
    }
}
