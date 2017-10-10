using System;
using System.Collections;
using GCL.Common;
using GCL.Event;
using GCL.Threading;
namespace GCL.Collections.Pool {

    ///
    /// @author baibing
    /// 
    ///
    public class LRUPoolRefreshStaregy : QueuePoolStaregy, IPoolRefreshStaregy {
        public LRUPoolRefreshStaregy(System.Collections.IDictionary map, TimeSpan timeSpan) {
            this.keyMap = map;
            this.keyMap.Clear();
            this.timeSpan = timeSpan;
        }

        public LRUPoolRefreshStaregy(TimeSpan timeSpan)
            : this(new System.Collections.Generic.Dictionary<object, object>(), timeSpan) {
        }

        protected System.Collections.IDictionary keyMap;
        protected System.TimeSpan timeSpan;


        #region IPoolRefreshStaregy Members

        public virtual object AllowDel() {
            lock (this.coll) {
                try {
                    object v = this.coll.Peek();
                    if (Tool.IsEnable(v) && Tool.IsEnable(this.keyMap[v]) && ((DateTime)this.keyMap[v]).CompareTo(DateTime.Now) <= 0) {
                        return v;
                    } else
                        return null;
                } catch (System.InvalidOperationException ex) {
                    return null;
                }
            }
        }

        #endregion


        public override object Set(object value) {
            lock (this.coll) {
                object v = base.Set(value);
                this.keyMap.Add(value, DateTime.Now.Add(this.timeSpan));
                return v;
            }
        }

        public override void Clear() {
            lock (this.coll) {
                base.Clear();
                this.keyMap.Clear();
            }
        }

        public override void Remove(object value) {
            lock (this.coll) {
                base.Remove(value);
                this.keyMap.Remove(value);
            }
        }
    }
}
