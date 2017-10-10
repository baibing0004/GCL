using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.IO.Log;
using GCL.Event;
using GCL.Common;
using GCL.Threading;
using GCL.Threading.Process;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 单线程插件类，继承基本插件类
    /// 只需要初始化时设置不同的线程类（安全/循环/定时）可以实现对Action方法不同的调用
    /// 不需要处理init(),start(),stop()等操作，不需要注意CallProcessEvent传出状态
    /// </summary>
    public abstract class ABusinessThreadServer : ABusinessServer {


        #region 继承的构造函数
        /// <summary>
        /// 
        /// </summary>
        protected ABusinessThreadServer() : base() { }

        /// <summary>
        /// 便于外部设定日志格式化字符串
        /// </summary>
        /// <param name="formatter"></param>
        protected ABusinessThreadServer(string formatter)
            : base(formatter) {
        }

        /// <summary>
        /// 设定执行线程
        /// </summary>
        /// <param name="thread"></param>
        public ABusinessThreadServer(ProtectThread thread) {
            this.SetThread(thread);
        }

        /// <summary>
        /// 设定执行线程
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="thread"></param>
        public ABusinessThreadServer(string formatter, ProtectThread thread)
            : base(formatter) {
            this.SetThread(thread);
        }
        #endregion

        protected virtual void Alarm(string alarmText) {
            this.CallProcessEventSimple(LogType.RELEASE, -202, alarmText);
        }

        private ProtectThread thread;

        public void SetThread(ProtectThread thread) {
            if (Tool.IsEnable(this.thread))
                throw new InvalidOperationException("线程对象已经设置!");
            this.thread = thread;
            this.thread.SetRun(new Run(_Action));
            this.thread.ExceptionThrowenEvent += new EventHandle(thread_ExceptionThrowenEvent);
            this.thread.FinallyDoEvent += new EventHandle(thread_FinallyDoEvent);
        }

        protected ProtectThread GetThread() {
            return thread;
        }

        void thread_FinallyDoEvent(object sender, EventArg e) {
            if (CheckStop()) {
                try {
                    this.OnClose();
                } catch {
                }
                this.CallProcessEventSimple(ProcessState.STOP);
            }
        }

        void thread_ExceptionThrowenEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(((ThreadEventArg)e).GetException(), "服务运行错误!");
        }


        protected abstract void OnClose();
        /// <summary>
        /// 初次运行
        /// </summary>
        protected abstract void FirstRun();
        /// <summary>
        /// 循环业务处理内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void Action(object sender, EventArg e);

        private void _Action(object sender, EventArg e) {
            if (sender is CircleThread && ((CircleThread)sender).IsFirst())
                this.FirstRun();
            this.Action(sender, e);
        }
        public override void Init() {
            try {
                this.CallProcessEventSimple(ProcessState.INIT);
                this.CallProcessEventSimple(ProcessState.READY);
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "服务初始化错误不可运行!");
            }
        }

        public override void Start() {
            try {
                this.CallProcessEventSimple(ProcessState.START);
                this.thread.Start();
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "服务启动失败!");
            }
        }

        public override void Stop() {
            if (this.thread is CircleThread)
                ((CircleThread)thread).SetCanRun(false);
            else
                Tool.StopThread(this.thread.Thread);
        }

        protected override bool CheckStop() {
            return thread.IsFinally() || thread.Thread == null || !thread.Thread.IsAlive;
        }
    }
}
