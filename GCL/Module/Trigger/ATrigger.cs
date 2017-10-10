using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;

namespace GCL.Module.Trigger {
    /// <summary>
    ///  触发器原型用来处理 当某个条件具备时触发的事件操作
    /// </summary>
    public abstract class ATrigger:IDisposable {

        public event EventHandle TriggerEvent;

        private bool isEnable = false;

        public ATrigger(bool isEnable) {
            this.TriggerEvent += new EventHandle(EventArg._EventHandleDefault);
            this.isEnable = isEnable;
        }

        public ATrigger()
            : this(true) {
        }

        /// <summary>
        /// 设置是否允许触发
        /// </summary>
        /// <param name="IsEnable"></param>
        public virtual void SetEnable(bool isEnable) {
            this.isEnable = isEnable;
        }

        public virtual bool IsEnable {
            get { return isEnable; }
            set { SetEnable(value); }
        }

        /// <summary>
        /// 需要实现的方法 如果有结果那么会触发事件，否则正常进行。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract EventArg Attempt(EventArg e);

        /// <summary>
        /// 可以覆盖 以串行的处理方式单独处理这个事件，并且可以不触发事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void CallTriggerEventSafely(object sender, EventArg e) {
            EventArg.CallEventSafely(this.TriggerEvent, sender, e);
        }

        /// <summary>
        /// 尝试是否可以触发事件
        /// </summary>
        public virtual bool Taste() {
            return this.Taste(null);
        }

        /// <summary>
        /// 尝试是否可以触发事件
        /// </summary>
        /// <param name="e"></param>
        public virtual bool Taste(EventArg e) {
            if (!this.IsEnable)
                return false;
            EventArg result = this.Attempt(e);
            if (Tool.IsEnable(result)) {
                CallTriggerEventSafely(this, result);
                return true;
            } else
                return false;
        }

        public abstract void ReSet();

        #region IDisposable Members

        public virtual void Dispose() {
            this.SetEnable(false);
        }

        #endregion

        ~ATrigger(){
            this.Dispose();
        }
    }
}
