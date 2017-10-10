using GCL.Event;
namespace GCL.Collections.Pool {

    ///
    /// @author baibing
    /// 
    ///
    public class MapPoolEventArg : PoolEventArg {

        ///
        /// 
        ///
        public MapPoolEventArg() {
        }

        ///
        /// @param size
        ///
        public MapPoolEventArg(int size)
            : base(size) {

        }

        ///
        /// @param level
        ///
        public MapPoolEventArg(EventLevel level)
            : base(level) {

        }

        ///
        /// @param obj
        ///
        public MapPoolEventArg(object obj)
            : base(obj) {

        }

        ///
        /// @param objs
        ///
        public MapPoolEventArg(object[] objs)
            : base(objs) {

        }

        ///
        /// @param obj
        /// @param allowSet
        ///
        public MapPoolEventArg(object obj, bool allowSet)
            : base(obj, allowSet) {

        }

        ///
        /// @param objs
        /// @param allowSet
        ///
        public MapPoolEventArg(object[] objs, bool allowSet)
            : base(objs, allowSet) {

        }

        ///
        /// @param eventNum
        /// @param size
        ///
        public MapPoolEventArg(int eventNum, int size)
            : base(eventNum, size) {

        }

        ///
        /// @param eventNum
        /// @param obj
        ///
        public MapPoolEventArg(int eventNum, object obj)
            : base(eventNum, obj) {

        }

        ///
        /// @param eventNum
        /// @param objs
        ///
        public MapPoolEventArg(int eventNum, object[] objs)
            : base(eventNum, objs) {

        }

        ///
        /// @param eventNum
        /// @param obj
        /// @param allowSet
        ///
        public MapPoolEventArg(int eventNum, object obj, bool allowSet)
            : base(eventNum, obj, allowSet) {

        }

        ///
        /// @param eventNum
        /// @param objs
        /// @param allowSet
        ///
        public MapPoolEventArg(int eventNum, object[] objs, bool allowSet)
            : base(eventNum, objs, allowSet) {

        }

        ///
        /// @param level
        /// @param size
        ///
        public MapPoolEventArg(EventLevel level, int size)
            : base(level, size) {

        }

        ///
        /// @param level
        /// @param obj
        ///
        public MapPoolEventArg(EventLevel level, object obj)
            : base(level, obj) {

        }

        ///
        /// @param level
        /// @param objs
        ///
        public MapPoolEventArg(EventLevel level, object[] objs)
            : base(level, objs) {

        }

        ///
        /// @param level
        /// @param obj
        /// @param allowSet
        ///
        public MapPoolEventArg(EventLevel level, object obj, bool allowSet)
            : base(level, obj, allowSet) {

        }

        ///
        /// @param level
        /// @param objs
        /// @param allowSet
        ///
        public MapPoolEventArg(EventLevel level, object[] objs, bool allowSet)
            : base(level, objs, allowSet) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param obj
        ///
        public MapPoolEventArg(EventLevel level, int eventNum, object obj)
            : base(level, eventNum, obj) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param obj
        /// @param allowSet
        ///
        public MapPoolEventArg(EventLevel level, int eventNum, object obj,
                bool allowSet)
            : base(level, eventNum, obj, allowSet) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param size
        ///
        public MapPoolEventArg(EventLevel level, int eventNum, int size)
            : base(level, eventNum, size) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param objs
        ///
        public MapPoolEventArg(EventLevel level, int eventNum, object[] objs)
            : base(level, eventNum, objs) {

        }

        ///
        /// @param level
        /// @param eventNum
        /// @param objs
        /// @param allowSet
        ///
        public MapPoolEventArg(EventLevel level, int eventNum, object[] objs,
                bool allowSet)
            : base(level, eventNum, objs, allowSet) {

        }

        private object key;

        ///
        /// @return key
        ///
        public object GetKey() {
            return key;
        }

        ///
        /// @param key
        ///            要设置的 key
        ///
        public void SetKey(object key) {
            this.key = key;
        }

        public object Key {
            get {
                return this.GetKey();
            }
            set {
                this.SetKey(value);
            }
        }

        public static MapPoolEventArg CreateMapPoolEventArg(object key, object value) {
            MapPoolEventArg arg = new MapPoolEventArg();
            arg.SetKey(key);
            arg.SetValue(value);
            return arg;
        }

        /// <summary>
        /// EventHandle的默认实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _EventHandleDefault(object sender, MapPoolEventArg e) {
        }

        /// <summary>
        /// EventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(EEventHandle<MapPoolEventArg> handle, object sender, MapPoolEventArg e) {
            try {
                handle(sender, e);
            } catch {
            }
        }
    }
}
