using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.IO.Log;

namespace GCL.Threading.Process {
    public class ProcessEventArg : EventArg {
        /// <summary>
        /// 默认的Process事件的实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _ProcessEventHandleDefault(object sender, ProcessEventArg e) {
        }

        /// <summary>
        /// ProcessEventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        public static void CallProcessEventSafely(ProcessEventHandle handle, object sender, ProcessEventArg e) {
            try {
                handle(sender, e);
            } catch {
            }
        }

        /// <summary>
        /// ProcessEventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(ProcessEventHandle handle, object sender, ProcessEventArg e) {
            CallProcessEventSafely(handle, sender, e);
        }

        private ProcessState state = ProcessState.NOSTATE;
        /// <summary>
        /// 获得对应的状态
        /// </summary>
        public ProcessState GetState() {
            return state;
        }

        /// <summary>
        /// 程序状态
        /// </summary>
        public ProcessState State {
            get {
                return GetState();
            }
        }

        private Exception ex = null;
        /// <summary>
        /// 获得对应的错误
        /// </summary>
        public Exception GetException() {
            return ex;
        }

        /// <summary>
        /// 程序错误
        /// </summary>
        public Exception Exception {
            get {
                return GetException();
            }
        }

        /// <summary>
        /// 默认为Info
        /// </summary>
        private LogType logType = LogType.INFO;

        /// <summary>
        /// 获取消息日志等级
        /// </summary>
        /// <returns></returns>
        public LogType GetLogType() {
            return logType;
        }

        /// <summary>
        /// 消息日志等级
        /// </summary>
        public LogType LogType {
            get { return GetLogType(); }
        }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventNum"></param>
        /// <param name="args"></param>
        public ProcessEventArg(LogType type, int eventNum, object[] args)
            : base(eventNum, args) {
            this.logType = type;
        }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventNum"></param>
        /// <param name="arg"></param>
        public ProcessEventArg(LogType type, int eventNum, object arg)
            : base(eventNum, arg) {
            this.logType = type;
        }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="state"></param>
        public ProcessEventArg(ProcessState state)
            : base(EventLevel.Importent) {
            this.state = state;
            this.logType = IO.Log.LogType.RELEASE;
        }
        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        public ProcessEventArg(ProcessState state, Exception ex, LogType type, object arg)
            : base(EventLevel.Importent, arg) {
            this.state = state;
            this.ex = ex;
            this.logType = type;
        }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="args"></param>
        public ProcessEventArg(ProcessState state, Exception ex, object[] args)
            : base(EventLevel.Importent, args) {
            this.state = state;
            this.ex = ex;
        }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="arg"></param>
        public ProcessEventArg(ProcessState state, Exception ex, object arg)
            : base(EventLevel.Importent, arg) {
            this.state = state;
            this.ex = ex;
        }

        #region 继承的构造函数

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置
        /// </summary>
        /// <param name="size">事件参数数目</param>
        public ProcessEventArg(int size)
            : base(new object[size], true) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置
        /// </summary>
        /// <param name="level">事件级别</param>
        public ProcessEventArg(EventLevel level)
            : base(level, -1, null, false) {
        }


        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置
        /// </summary>
        /// <param name="obj">事件参数</param>
        public ProcessEventArg(object obj)
            : base(new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置
        /// </summary>
        /// <param name="objs">事件参数</param>
        public ProcessEventArg(object[] objs)
            : base(objs, false) {
        }



        /// <summary>
        /// 实现相关的数组参数设置 默认无事件号 事件级别为Comment
        /// </summary>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置</param>
        public ProcessEventArg(object obj, bool allowSet)
            : base(EventLevel.Comment, -1, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认无事件号 事件级别为Comment
        /// </summary>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置</param>
        public ProcessEventArg(object[] objs, bool allowSet)
            : base(EventLevel.Comment, -1, objs, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="size">事件参数大小</param>
        public ProcessEventArg(int eventNum, int size)
            : base(EventLevel.Comment, eventNum, size) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        public ProcessEventArg(int eventNum, object obj)
            : base(eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        public ProcessEventArg(int eventNum, object[] objs)
            : base(eventNum, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(int eventNum, object obj, bool allowSet)
            : base(eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(int eventNum, object[] objs, bool allowSet)
            : base(EventLevel.Comment, eventNum, objs, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="size">事件数目</param>
        public ProcessEventArg(EventLevel level, int size)
            : base(level, -1, size) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="objs">事件参数</param>
        public ProcessEventArg(EventLevel level, object obj)
            : base(level, new object[] { obj }, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="obj">事件参数</param>
        public ProcessEventArg(EventLevel level, object[] objs)
            : base(level, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(EventLevel level, object obj, bool allowSet)
            : base(level, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(EventLevel level, object[] objs, bool allowSet)
            : base(level, -1, objs, allowSet) {
        }


        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        public ProcessEventArg(EventLevel level, int eventNum, object obj)
            : base(level, eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(EventLevel level, int eventNum, object obj, bool allowSet)
            : base(level, eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认可以设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="size">事件参数数目</param>
        public ProcessEventArg(EventLevel level, int eventNum, int size)
            : base(level, eventNum, new object[size], true) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        public ProcessEventArg(EventLevel level, int eventNum, object[] objs)
            : base(level, eventNum, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public ProcessEventArg(EventLevel level, int eventNum, object[] objs, bool allowSet)
            : base(level, eventNum, objs, allowSet) {
        }

        #endregion
    }
}
