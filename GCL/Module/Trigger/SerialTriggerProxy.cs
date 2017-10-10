using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Module.Trigger {
    /// <summary>
    /// 串行触发器代理 要求先触发器的事件来触发后触发器，当2个条件都满足时才触发整体的事件
    /// 
    /// @author 白冰
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class SerialTriggerProxy : TriggerProxy {
        private ATrigger fT;
        public SerialTriggerProxy(ATrigger first, ATrigger main)
            : base(main) {
            first.TriggerEvent += new GCL.Event.EventHandle(first_TriggerEvent);
            fT = first;
            this.SetEnable(fT.IsEnable);
        }

        void first_TriggerEvent(object sender, GCL.Event.EventArg e) {
            GetTrigger().Taste(e);
        }

        public override void SetEnable(bool isEnable) {
            lock (this) {
                fT.SetEnable(isEnable);
                base.SetEnable(isEnable); 
            }
        }

        public override void ReSet() {
            lock (this) {
                fT.ReSet();
                base.ReSet(); 
            }
        }

        public override bool Taste(GCL.Event.EventArg e) {
            return fT.Taste(e);
        }
    }
}
