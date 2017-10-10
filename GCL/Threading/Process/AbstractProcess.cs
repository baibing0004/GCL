using System;
using System.Collections.Generic;
using System.Text;
using GCL.IO.Log;

namespace GCL.Threading.Process {
    /// <summary>
    /// 用于为IProcess的继承类提供一些方便的统一的实现方法
    /// 注意如果INIT中新建了资源 请务必重载CheckDispose()方法 释放资源 否则很容易造成无法关闭程序
    /// </summary>
    public abstract class AbstractProcess : IProcess {

        ///// <summary>
        /////  用于为IProcess的继承类提供一些方便的统一的实现方法
        ///// </summary>
        ///// <param name="type">外部唯一一次可以设置其日志级别的机会</param>
        //public AbstractProcess(LogType type)
        //    : this() {
        //    this.logType = type;
        //}

        /// <summary>
        ///  用于为IProcess的继承类提供一些方便的统一的实现方法
        /// </summary>
        /// <param name="type">外部唯一一次可以设置其日志级别的机会</param>
        public AbstractProcess() {
            this.ProcessEvent += new ProcessEventHandle(ProcessEventArg._ProcessEventHandleDefault);
        }

        /// <summary>
        /// 基础方法 最终调用并抛出事件 如果要提前处理事件可以在这里处理
        /// </summary>
        /// <param name="e"></param>
        protected virtual void CallProcessEventSimple(ProcessEventArg e) {
            if (e.Level == Event.EventLevel.Importent) {
                if (e.State != ProcessState.EXCEPTION)
                    state = e.State;
                else if (state == ProcessState.INIT)
                    state = ProcessState.NOSTATE;
                else if (state == ProcessState.START)
                    state = ProcessState.STOP;
                if (state == ProcessState.STOP)
                    CheckDispose(true);
            }
            if (e.Level == GCL.Event.EventLevel.Importent || e.LogType <= this.BaseLogType)
                ProcessEventArg.CallEventSafely(this.ProcessEvent, this, e);
        }

        /// <summary>
        /// 方便处理方法 便于程序声明进程状态
        /// </summary>
        /// <param name="state"></param>
        protected virtual void CallProcessEventSimple(ProcessState state) {
            this.CallProcessEventSimple(new ProcessEventArg(state));
        }

        /// <summary>
        /// 方便处理方法 便于程序声明状态，错误，以及一个附加的对象
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(Exception ex, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(ProcessState.EXCEPTION, ex, LogType.ERROR, obj));
        }

        /// <summary>
        /// 方便处理方法 便于程序声明状态，错误，以及一个附加的对象
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(Exception ex, object[] obj) {
            this.CallProcessEventSimple(new ProcessEventArg(ProcessState.EXCEPTION, ex, LogType.ERROR, obj));
        }

        /// <summary>
        /// 方便处理方法 说明事件号已经附加一个对象
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, obj));
        }

        /// <summary>
        ///  方便处理方法 说明事件号已经附加一组对象
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallProcessEventSimple(int eventNum, object[] objs) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, objs));
        }

        /// <summary>
        /// 方便处理方法 说明事件号已经附加一个对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(LogType type, int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(type, eventNum, obj));
        }

        /// <summary>
        ///  方便处理方法 说明事件号已经附加一组对象
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallProcessEventSimple(LogType type, int eventNum, object[] objs) {
            this.CallProcessEventSimple(new ProcessEventArg(type, eventNum, objs));
        }

        /// <summary>
        /// 默认为Info
        /// </summary>
        private LogType logType = LogType.All;

        /// <summary>
        /// 获取消息日志等级
        /// </summary>
        /// <returns></returns>
        public LogType GetLogType() {
            return logType;
        }

        /// <summary>
        /// 允许改变日志等级
        /// </summary>
        /// <param name="type"></param>
        protected void SetLogType(LogType type) {
            this.logType = type;
        }

        /// <summary>
        /// 消息日志等级
        /// </summary>
        public LogType BaseLogType {
            get { return GetLogType(); }
        }

        private ProcessState state = ProcessState.NOSTATE;

        /// <summary>
        /// 进程状态
        /// </summary>
        /// <returns></returns>
        public ProcessState GetState() {
            return state;
        }

        /// <summary>
        /// 进程状态
        /// </summary>
        public ProcessState State {
            get {
                return GetState();
            }
        }

        #region IProcess Members

        public event ProcessEventHandle ProcessEvent;

        public abstract void Init();

        public abstract void Start();

        /// <summary>
        /// 请检测CheckStop以确定 是否触发Stop状态或者Dispose状态
        /// </summary>
        public abstract void Stop();

        #endregion

        #region IDisposable Members

        public virtual void Dispose() {
            try {
                this.isDispose = true;
                this.Stop();
                CheckDispose();
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, this.GetType().Name + "关闭出现异常！");
            }
        }

        protected abstract bool CheckStop();

        protected virtual void CheckDispose() {
            this.CheckDispose(CheckStop() || (canDisposeByReady && this.GetState() <= ProcessState.READY));
        }

        private bool canDisposeByReady = true;

        public bool CanDisposeByReady {
            get { return canDisposeByReady; }
            set { canDisposeByReady = value; }
        }


        protected bool isDispose = false;
        protected virtual void CheckDispose(bool dispose) {
            if (this.isDispose && dispose)
                this.CallProcessEventSimple(ProcessState.DISPOSE);
        }

        #endregion
    }
}
