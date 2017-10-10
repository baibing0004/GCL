using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;


namespace GCL.Module.Trigger {
    /// <summary>
    ///
    /// 继承Trigger 处理对于计数方式而触发的触发器。其默认算法在触发以后，触发器静止状态不再触发。复杂算法可以通过继承重载reSet方法实现，如果复杂可以通过订阅事件进行处理。
    /// 
    /// @author baibing
    /// @version 2.1.0.70920
    /// 
    /// </summary>
    public class CountTrigger : ATrigger {

        public object NumKey = DateTime.Now;

        private long num = 0, init = 1, limitNum = 0;

        public long InitNum {
            get { return init; }
            set { init = value; }
        }

        public long Num {
            get { return num; }
            set { num = value; }
        }

        public long LimitNum {
            get { return limitNum; }
            set { limitNum = value; }
        }

        public CountTrigger(int num, int limit) {
            this.num = num;
            this.init = num;
            this.limitNum = limit;
        }
        public CountTrigger(int limit)
            : this(1, limit) {
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            lock (this.NumKey) {
                if (this.num >= this.limitNum) {
                    this.ReSet();
                    return new EventArg();
                } else
                    this.num++;
            }
            return null;
        }

        public override void ReSet() {
            this.num = init;
        }
    }
}
