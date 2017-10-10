using GCL.Event;
namespace GCL.Collections.Pool {

    ///
    /// @author baibing
    /// 
    ///
    public class PoolEventArg : EventArg {

        ///
        /// 
        ///
        public PoolEventArg() {
        }

        ///
        /// @param size
        ///
        public PoolEventArg(int size)
            : base(size) {

        }

        ///
        /// @param level
        ///
        public PoolEventArg(EventLevel level)
            : base(level) {

        }

        ///
        /// @param obj
        ///
        public PoolEventArg(object obj)
            : base(obj) {

        }

        ///
        /// @param objs
        ///
        public PoolEventArg(object[] objs)
            : base(objs) {

        }

        ///
        /// @param obj
        /// @param allowSet
        ///
        public PoolEventArg(object obj, bool allowSet)
            : base(obj, allowSet) {

        }

        ///
        /// @param objs
        /// @param allowSet
        ///
        public PoolEventArg(object[] objs, bool allowSet)
            : base(objs, allowSet) {

        }

        ///
        /// @param eventNum
        /// @param size
        ///
        public PoolEventArg(int eventNum, int size)
            : base(eventNum, size) {

        }

        ///
        /// @param eventNum
        /// @param obj
        ///
        public PoolEventArg(int eventNum, object obj)
            : base(eventNum, obj) {

        }

        ///
        /// @param eventNum
        /// @param objs
        ///
        public PoolEventArg(int eventNum, object[] objs)
            : base(eventNum, objs) {

        }

        ///
        /// @param eventNum
        /// @param obj
        /// @param allowSet
        ///
        public PoolEventArg(int eventNum, object obj, bool allowSet)
            : base(eventNum, obj, allowSet) {

        }

        ///
        /// @param eventNum
        /// @param objs
        /// @param allowSet
        ///
        public PoolEventArg(int eventNum, object[] objs, bool allowSet)
            : base(eventNum, objs, allowSet) {

        }

        ///
        /// @param level
        /// @param size
        ///
        public PoolEventArg(EventLevel level, int size)
            : base(level, size) {

        }

        ///
        /// @param level
        /// @param obj
        ///
        public PoolEventArg(EventLevel level, object obj)
            : base(level, obj) {

        }

        ///
        /// @param level
        /// @param objs
        ///
        public PoolEventArg(EventLevel level, object[] objs)
            : base(level, objs) {

        }

        ///
        /// @param level
        /// @param obj
        /// @param allowSet
        ///
        public PoolEventArg(EventLevel level, object obj, bool allowSet)
            : base(level, obj, allowSet) {

        }

        ///
        /// @param level
        /// @param objs
        /// @param allowSet
        ///
        public PoolEventArg(EventLevel level, object[] objs, bool allowSet)
            : base(level, objs, allowSet) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param obj
        ///
        public PoolEventArg(EventLevel level, int eventNum, object obj)
            : base(level, eventNum, obj) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param obj
        /// @param allowSet
        ///
        public PoolEventArg(EventLevel level, int eventNum, object obj,
                bool allowSet)
            : base(level, eventNum, obj, allowSet) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param size
        ///
        public PoolEventArg(EventLevel level, int eventNum, int size)
            : base(level, eventNum, size) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param objs
        ///
        public PoolEventArg(EventLevel level, int eventNum, object[] objs)
            : base(level, eventNum, objs) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param objs
        /// @param allowSet
        ///
        public PoolEventArg(EventLevel level, int eventNum, object[] objs,
                bool allowSet)
            : base(level, eventNum, objs, allowSet) {

        }

        private object _Value;
        public object Value {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        ///
        /// @return value
        ///
        public object GetValue() {
            return _Value;
        }


        public void SetValue(object value) {
            _Value = value;
        }

        public static PoolEventArg CreatePoolEventArg(object value) {
            PoolEventArg arg = new PoolEventArg();
            arg.SetValue(value);
            return arg;
        }

        /// <summary>
        /// EventHandle的默认实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _EventHandleDefault(object sender,PoolEventArg e) {
        }

        /// <summary>
        /// EventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(EEventHandle<PoolEventArg> handle, object sender, PoolEventArg e) {
            try {
                handle(sender, e);
            } catch {
            }
        }
    }
}
