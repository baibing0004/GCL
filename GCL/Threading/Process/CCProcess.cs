using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Common;
using GCL.Event;
using GCL.Collections;

namespace GCL.Threading.Process {

    /// <summary>
    /// 主要用来处理生产者/消费者模式问题 并提供进程模式方式的解决框架
    /// </summary>
    public class CCProcess : IProcess {

        /// <summary>
        /// 创建对象的方法代理
        /// </summary>
        protected CreaterAction crod;

        /// <summary>
        /// 消费对象的方法代理
        /// </summary>
        protected CustomobjectDel cuod;

        /// <summary>
        /// 生产者数目
        /// </summary>
        protected int createrNum;

        /// <summary>
        /// 获得生产者数目
        /// </summary>
        /// <returns></returns>
        public int GetCreaterNum() {
            return createrNum;
        }

        /// <summary>
        /// 生产者数目
        /// </summary>
        public int CreaterNum {
            get {
                return GetCreaterNum();
            }
        }

        /// <summary>
        /// 消费者数目
        /// </summary>
        protected int customerNum;

        /// <summary>
        /// 获得消费者数目
        /// </summary>
        /// <returns></returns>
        public int GetCustomerNum() {
            return customerNum;
        }

        /// <summary>
        /// 消费者数目
        /// </summary>
        public int CustomerNum {
            get {
                return GetCustomerNum();
            }
        }
        /// <summary>
        /// 根据对象生成生产者方法 消费者方法 缓冲区为10
        /// </summary>
        /// <param name="crod"></param>
        /// <param name="creater"></param>
        /// <param name="cuod"></param>
        /// <param name="customer"></param>
        public CCProcess(CreaterAction crod, int creater, CustomobjectDel cuod, int customer)
            : this(crod, creater, cuod, customer, 10) {
        }

        /// <summary>
        /// 根据对象生成生产者方法 消费者方法
        /// </summary>
        /// <param name="crod"></param>
        /// <param name="creater"></param>
        /// <param name="cuod"></param>
        /// <param name="customer"></param>
        public CCProcess(CreaterAction crod, int creater, CustomobjectDel cuod, int customer, int capacity) {
            if (creater <= 0 | customer <= 0 | capacity <= 0)
                throw new IndexOutOfRangeException("错误，生产者队列，消费者队列，消息队列的长度都不能小于等于0");
            this.crod = crod;
            this.createrNum = creater;
            this.cuod = cuod;
            this.customerNum = customer;
            this.capacity = capacity;
        }

        /// <summary>
        /// 缓冲队列大小
        /// </summary>
        protected int capacity = 0;

        protected CreaterThread[] crT;
        protected CustomerThread[] cuT;

        /// <summary>
        /// 共用的限制对象（消息）队列
        /// </summary>
        protected LimitQueue queue;

        #region IProcess Members

        public event ProcessEventHandle ProcessEvent;

        protected virtual void CallProcessEventSimple(ProcessEventArg e) {
            ProcessEventArg.CallEventSafely(ProcessEvent, this, e);
        }

        protected virtual void CallProcessEventSimple(ProcessState state) {
            this.CallProcessEventSimple(new ProcessEventArg(state));
        }

        protected virtual void CallProcessEventSimple(ProcessState state, Exception ex, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(state, ex, obj));
        }

        protected virtual void CallProcessEventSimple(int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, obj));
        }

        /// <summary>
        /// 生产者线程等待时间
        /// </summary>
        protected int createWaitTime = 500;

        public int GetCreateWaitTime() {
            return this.createWaitTime;
        }

        public void SetCreateWaitTime(int value) {
            if (value <= 0)
                throw new System.IndexOutOfRangeException("对不起，等待时间不能小于等于零！");
            if (this.createWaitTime != 500)
                throw new InvalidOperationException("等待时间已经设置 不可以再次设置");
            this.createWaitTime = value;
        }

        /// <summary>
        /// 生产者当队列满时等待时间 默认半秒钟 主要为了对关闭操作进行灵敏的反应
        /// </summary>
        public int CreateWaitTime {
            get {
                return this.GetCreateWaitTime();
            }
            set {
                this.SetCreateWaitTime(value);
            }
        }

        /// <summary>
        /// 消费者线程等待时间 默认半秒钟 主要为了对关闭操作进行灵敏的反应
        /// </summary>
        protected int customWaitTime = 500;


        public int GetCustomWaitTime() {
            return this.customWaitTime;
        }

        public void SetCustomWaitTime(int value) {
            if (value <= 0)
                throw new System.IndexOutOfRangeException("对不起，等待时间不能小于等于零！");
            if (this.customWaitTime != 500)
                throw new InvalidOperationException("等待时间已经设置 不可以再次设置");
            this.customWaitTime = value;
        }

        /// <summary>
        /// 生产者当队列满时等待时间 默认半秒钟 主要为了对关闭操作进行灵敏的反应
        /// </summary>
        public int CustomWaitTime {
            get {
                return this.GetCustomWaitTime();
            }
            set {
                this.SetCustomWaitTime(value);
            }
        }
        /// <summary>
        /// 初始化操作
        /// 完成 共用的消息队列的大小
        /// 完成 生产者队列的新建
        /// 完成 消费者队列的新建
        /// </summary>
        /// <exception >所有可能抛出的错误</exception>
        public virtual void Init() {
            lock (this) {
                try {

                    //说明开始初始化状态
                    this.CallProcessEventSimple(ProcessState.INIT);

                    //新建共用的消息队列
                    this.queue = new LimitQueue(new System.Collections.Queue(), capacity);

                    //新建生产者队列
                    this.crT = new CreaterThread[this.createrNum];
                    for (int w = 0; w < this.createrNum; w++) {
                        this.crT[w] = new CreaterThread(this.crod, w, this.queue, this.createWaitTime);
                        this.crT[w].ExceptionThrowenEvent += new EventHandle(CCProcess_CR_ExceptionThrowenEvent);
                        this.crT[w].FinallyDoEvent += new EventHandle(CCProcess_CR_FinallyDoEvent);
                    }

                    //新建消费者队列
                    this.cuT = new CustomerThread[this.customerNum];
                    for (int w = 0; w < this.customerNum; w++) {
                        this.cuT[w] = new CustomerThread(this.cuod, w, this.queue, this.customWaitTime);
                        this.cuT[w].ExceptionThrowenEvent += new EventHandle(CCProcess_CU_ExceptionThrowenEvent);
                        this.cuT[w].FinallyDoEvent += new EventHandle(CCProcess_CU_FinallyDoEvent);
                    }

                    //说明初始化成功状态
                    this.CallProcessEventSimple(ProcessState.READY);

                    return;
                } catch (Exception ex) {
                    //抛出初始化错误
                    this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.INIT);
                }
            }
        }

        #region 将线程事件翻译成进程事件

        private void CCProcess_CR_FinallyDoEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(201, string.Format("{0}号生产者线程处理完成结束!", ((ProtectThread)sender).Num));
            this.DecideStop();
        }

        private void CCProcess_CR_ExceptionThrowenEvent(object sender, EventArg e) {
            ThreadEventArg te = (ThreadEventArg)e;
            ProcessEventArg pro = new ProcessEventArg(GCL.IO.Log.LogType.ERROR, 203, new object[] { string.Format("{0}号生产者线程发生错误!如果错误不影响程序继续运行请使用e.Cancle=true", te.GetThread().Num), te.GetException() });
            this.CallProcessEventSimple(pro);
            //判断是否允许重新启动
            if (pro.GetCancle(true))
                ((ProtectThread)sender).CreateThread().Start();
        }

        private void CCProcess_CU_FinallyDoEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(202, string.Format("{0}号消费者线程处理完成结束!", ((ProtectThread)sender).Num));
            this.DecideStop();
        }

        private void CCProcess_CU_ExceptionThrowenEvent(object sender, EventArg e) {
            ThreadEventArg te = (ThreadEventArg)e;
            ProcessEventArg pro = new ProcessEventArg(GCL.IO.Log.LogType.ERROR, 204, new object[] { string.Format("{0}号消费者线程发生错误! 如果错误不影响程序继续运行请使用e.Cancle=true", te.GetThread().Num), te.GetException() });
            this.CallProcessEventSimple(pro);
            //判断是否允许重新启动
            if (pro.GetCancle(true))
                ((ProtectThread)sender).CreateThread().Start();
        }

        #endregion

        /// <summary>
        /// 决定是否停止
        /// </summary>
        private void DecideStop() {

            if (this.isStop)
                return;

            foreach (CircleThread thread in crT) {
                if (thread.IsCanRun())
                    return;
            }

            foreach (CircleThread thread in cuT) {
                if (thread.IsCanRun())
                    return;
            }

            this.isStop = true;
            this.CallProcessEventSimple(ProcessState.STOP);
        }

        /// <summary>
        /// 先启动消费者 后启动生产者 允许暂停/继续操作
        /// </summary>
        public virtual void Start() {
            try {
                this.isStop = false;
                //消费者先启动
                for (int w = 0; w < this.cuT.Length; w++) {
                    //判断线程是否正常运行
                    if (cuT[w].IsCanRun() && Tool.IsEnable(cuT[w].Thread) && cuT[w].Thread.ThreadState == ThreadState.Running) {
                        //如果线程正常运行那么不做处理
                    } else {
                        //不管是线程不允许运行但是线程运行正常的 还是线程允许运行但是线程运行不正常的或者没有启动的 都需要重新启动
                        //线程运行正常的需要强行关闭
                        if (Tool.IsEnable(cuT[w].Thread) && cuT[w].Thread.ThreadState == ThreadState.Running) {
                            cuT[w].SetCanRun(false);
                            Tool.StopThread(cuT[w].Thread);
                        }
                        cuT[w].SetCanRun(true);
                        cuT[w].CreateThread().Start();
                    }
                }

                //生产者后启动
                for (int w = 0; w < this.crT.Length; w++) {
                    //判断线程是否正常运行
                    if (crT[w].IsCanRun() && Tool.IsEnable(crT[w].Thread) && crT[w].Thread.ThreadState == ThreadState.Running) {
                        //如果线程正常运行那么不做处理
                    } else {
                        //不管是线程不允许运行但是线程运行正常的 还是线程允许运行但是线程运行不正常的或者没有启动的 都需要重新启动
                        //线程运行正常的需要强行关闭
                        if (Tool.IsEnable(crT[w].Thread) && crT[w].Thread.ThreadState == ThreadState.Running) {
                            crT[w].SetCanRun(false);
                            Tool.StopThread(crT[w].Thread);
                        }
                        crT[w].SetCanRun(true);
                        crT[w].CreateThread().Start();
                    }
                }
                this.CallProcessEventSimple(ProcessState.START);
                return;
            } catch (Exception ex) {
                //抛出启动错误
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.START);
            }
        }
        /// <summary>
        /// 说明是不是主动停止 从而防止多次启动
        /// </summary>
        private bool isStop = false;
        public virtual void Stop() {
            try {
                this.isStop = true;
                for (int w = 0; w < crT.Length; w++)
                    crT[w].SetCanRun(false);

                for (int w = 0; w < cuT.Length; w++)
                    cuT[w].SetCanRun(false);

                this.CallProcessEventSimple(ProcessState.STOP);
                return;
            } catch (Exception ex) {
                //抛出启动错误
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.STOP);
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose() {
            try {
                this.isStop = true;
                for (int w = 0; w < crT.Length; w++) {
                    crT[w].SetCanRun(false);
                    Tool.StopThread(crT[w].Thread);
                }

                for (int w = 0; w < cuT.Length; w++) {
                    cuT[w].SetCanRun(false);
                    Tool.StopThread(cuT[w].Thread);
                }

                this.queue.Clear();
                this.CallProcessEventSimple(ProcessState.DISPOSE);
                return;
            } catch (Exception ex) {
                //抛出启动错误
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.DISPOSE);
            }
        }

        #endregion
    }
}
