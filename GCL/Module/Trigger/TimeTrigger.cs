using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;

namespace GCL.Module.Trigger {
    /// <summary>
    ///
    /// 继承Trigger 处理对于过期时间的触发
    /// 
    /// @author baibing
    /// @version 2.1.0.70920
    /// 
    /// </summary>
    public class TimeTrigger : ATrigger {

        private DateTime passTime = DateTime.Now;

        public DateTime PassTime {
            get { return passTime; }
        }
        private TimeSpan span;

        public TimeSpan TimeSpanField {
            get { return span; }
            set { span = value; }
        }

        public TimeTrigger(TimeSpan span) {
            this.span = span;
            if (span == null)
                throw new InvalidOperationException("时间戳不能为空");
            this.ReSet();
        }
        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            if (this.passTime.CompareTo(DateTime.Now) <= 0) {
                this.ReSet();
                return new EventArg(this.passTime);
            }
            return null;
        }

        public override void ReSet() {
            passTime = DateTime.Now.Add(this.span);
        }
    }
}
