using System;
using GCL.Event;
using System.Collections.Generic;
using System.Collections;
namespace GCL.Module.Trigger {
    /// <summary>
    /// 并行触发器代理 用来多个触发器同步测试触发，订阅一个事件等同订阅多个触发器，也可以分别订阅触发器的事件。触发器按照加入先后顺序进行触发。
    /// 
    /// @author 白冰
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class ParallelTriggerProxy : ATrigger {

        public ParallelTriggerProxy() : this(new LinkedList<ATrigger>()) { }

        public ParallelTriggerProxy(ICollection<ATrigger> list) {
            this.list = list;
            this.list.Clear();
        }

        public ATrigger[] GetTriggers() {
            lock (list) {
                ATrigger[] tris = new ATrigger[this.list.Count];
                this.list.CopyTo(tris, 0);
                return tris; 
            }
        }

        protected override EventArg Attempt(EventArg e) {
            return null;
        }

        protected ICollection<ATrigger> list;

        public void AddTrigger(ATrigger trigger) {
            lock (list) {
                trigger.TriggerEvent += new EventHandle(trigger_TriggerEvent);
                trigger.SetEnable(this.IsEnable);
                this.list.Add(trigger);
            }
        }

        void trigger_TriggerEvent(object sender, EventArg e) {
            this.ReSet();
            this.CallTriggerEventSafely(sender, e);
        }

        public bool RemoveTrigger(ATrigger trigger) {
            lock (list) {
                //TODO 没有清理其事件触发
                trigger.SetEnable(false);
                return list.Remove(trigger);
            }
        }

        protected void CallVisiterAction(ITriggerVisiter vister) {
            ATrigger[] tris = GetTriggers();
            for (IEnumerator eum = tris.GetEnumerator(); eum.MoveNext(); )
                try {
                    vister.Action((ATrigger)eum.Current);
                } catch (Exception e) {
                }
        }

        private SetEnableTriggerVisiter seVisiter = new SetEnableTriggerVisiter();

        public override void SetEnable(bool isEnable) {
            seVisiter.IsEnable = isEnable;
            this.CallVisiterAction(seVisiter);
            base.SetEnable(isEnable);
        }

        private TasteTriggerVisiter taVisiter = new TasteTriggerVisiter();

        public override bool Taste(EventArg e) {
            lock (taVisiter) {
                taVisiter.IsActive = false;
                taVisiter.EventArg = e;
                this.CallVisiterAction(taVisiter);
                return taVisiter.IsActive;
            }
        }

        private ReSetTriggerVisiter rsVisiter = new ReSetTriggerVisiter();

        public override void ReSet() {
            this.CallVisiterAction(rsVisiter);
        }

        private DisposeTriggerVisiter dsVisiter = new DisposeTriggerVisiter();
        public override void Dispose() {
            this.CallVisiterAction(dsVisiter);
            base.Dispose();
        }
    }
}