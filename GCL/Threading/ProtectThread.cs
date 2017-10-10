using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Event;
using GCL.Common;

namespace GCL.Threading {

    /// <summary>
    /// 声明基本的执行事件
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
	 *            要设置的 ex
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
         *            要设置的 thread
         */
        protected void SetThread(ProtectThread thread) {
            this.thread = thread;
        }
    }

    /// <summary>
    /// 主要用于提供对线程体的致命错误的捕捉机制
    /// FinallyDo在运行完成时总会运行的一段代码可以用于处理变量清除
    /// 错误通过ExceptionThrowen方法传递给线程，默认方法内只是输出这个错误！子类可以替换这个方法以处理错误但不能重新启动线程 
    /// </summary>
    public class ProtectThread {

        protected Run protRun;
        protected string id = Guid.NewGuid().ToString().Replace("-", "");
        //获得线程ID
        public string ID { get { return id; } }

        public void SetRun(Run runMethod) {
            if (!Tool.IsEnable(runMethod))
                throw new Exception("Null method 空代理方法！");
            if (Tool.IsEnable(this.protRun))
                throw new InvalidOperationException("代理对象已经设置，不能重新设置!");
            this.protRun = runMethod;
        }

        /// <summary>
        /// 线程号
        /// </summary>
        private int num = -1;

        /// <summary>
        /// -1时为未设置
        /// </summary>
        /// <returns></returns>
        public int GetNum() {
            return num;
        }

        public void SetNum(int data) {
            if (this.num != -1)
                throw new InvalidOperationException("数值已经设置");
            else
                this.num = data;
        }

        /// <summary>
        /// 线程号 只能设置一次
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
        /// 自定义处理protRun对象
        /// </summary>
        /// <param name="_protRun"></param>
        public ProtectThread(Run _protRun)
            : this() {
            this.SetRun(_protRun);
        }

        /// <summary>
        /// 只在继承时有效 但是需要自定义proRun对象
        /// </summary>
        public ProtectThread() {
            this.ExceptionThrowenEvent += new EventHandle(EventArg._EventHandleDefault);
            this.FinallyDoEvent += new EventHandle(EventArg._EventHandleDefault);
            this.ThreadEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// 代理线程执行方法
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
        /// 建议覆盖 对错误进行捕捉的线程方法
        /// </summary>
        protected virtual void ProtectedRun() {
            this.protRun(this, new ThreadEventArg(this));
        }

        /// <summary>
        /// 抛出错误 并产生错误事件 完成FinallyDo方法后线程停止
        /// </summary>
        public event EventHandle ExceptionThrowenEvent;

        /// <summary>
        /// 建议覆盖 如果要对一些类的变量进行处理需要重写这个方法！默认作打印输出处理！
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void ExceptionThrowen(Exception ex) {
            Console.WriteLine(this.GetType().FullName + " run Exception:"
                    + ex.Message);
            EventArg.CallEventSafely(ExceptionThrowenEvent, this, new ThreadEventArg(ex, this));
        }

        /// <summary>
        /// 通知可以完成关闭操作了
        /// </summary>
        public event EventHandle FinallyDoEvent;

        private bool isFinally = true;

        public bool IsFinally() {
            return isFinally;
        }

        /// <summary>
        /// 建议覆盖 如果要对一些类的变量进行处理需要重写这个方法！默认作打印输出处理！
        /// </summary>
        protected virtual void FinallyDo() {
            Console.WriteLine(this.GetType().FullName + " run finally");
            EventArg.CallEventSafely(FinallyDoEvent, this, new ThreadEventArg(this));
        }

        /// <summary>
        /// 安全启动线程 不支持多重启动
        /// </summary>
        public virtual void Start() {
            if (IsFinally()) {
                Tool.StopThread(this.Thread);
                this.CreateThread().Start();
            }
        }

        /// <summary>
        /// 产生线程 并设置Thread属性为最后一次运行的线程
        /// </summary>
        /// <returns></returns>
        public virtual Thread CreateThread() {
            Thread thread = new Thread(new ThreadStart(run));
            this.SetThread(thread);
            return thread;
        }

        /// <summary>
        /// 最近一个线程对象
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 获得线程对象
        /// </summary>
        /// <returns></returns>
        public Thread GetThread() {
            return this.thread;
        }

        /// <summary>
        /// 设置线程对象
        /// </summary>
        /// <param name="thread"></param>
        protected void SetThread(Thread thread) {
            this.thread = thread;
        }

        /// <summary>
        /// 产生的最近一个线程对象
        /// </summary>
        public Thread Thread {
            get {
                return GetThread();
            }
        }

        /// <summary>
        /// 不包括已有的事件 只是用于传递信息之用
        /// </summary>
        public event EventHandle ThreadEvent;
        protected virtual void CallThreadEventSimple(EventArg e) {
            EventArg.CallEventSafely(this.ThreadEvent, this, e);
        }
        /// <summary>
        /// 方便处理方法 说明事件号已经附加一个对象
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallThreadEventSimple(int eventNum, object obj) {
            this.CallThreadEventSimple(new EventArg(eventNum, obj));
        }

        /// <summary>
        ///  方便处理方法 说明事件号已经附加一组对象
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallThreadEventSimple(int eventNum, object[] objs) {
            this.CallThreadEventSimple(new EventArg(eventNum, objs));
        }
    }
}
