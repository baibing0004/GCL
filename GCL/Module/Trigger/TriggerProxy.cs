using GCL.Event;
using System;

namespace GCL.Module.Trigger {
    /// <summary>
    /// 触发器代理 用来完全代理Trigger可以用来生成其它代理的Trigger。
    /// 
    /// @author 白冰
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public abstract class TriggerProxy : ATrigger {

        private ATrigger trigger;

        /// <summary>
        /// @return trigger
        /// </summary>
        public ATrigger GetTrigger() {
            return trigger;
        }



        protected void SetTrigger(ATrigger trigger) {
            if (this.trigger != null)
                throw new InvalidOperationException("触发器已经设置不能重新设置！");
            this.trigger = trigger;
            trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
        }

        /// <summary>
        /// 继承的无参数构造函数，需要起设置Trigger
        /// </summary>
        protected TriggerProxy() {
        }
        public TriggerProxy(ATrigger trigger) {
            this.SetTrigger(trigger);
        }

        void trigger_TriggerEvent(object sender, EventArg e) {
            this.ReSet();
            this.CallTriggerEventSafely(this, e);
        }

        public override bool IsEnable {
            get {
                return trigger.IsEnable;
            }
            set {
                SetEnable(value);
            }
        }

        public override bool Taste(GCL.Event.EventArg e) {
            return trigger.Taste(e);
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            return null;
        }

        public override void SetEnable(bool isEnable) {
            trigger.SetEnable(isEnable);
        }

        public override void ReSet() {
            this.trigger.ReSet();
        }

    }
}