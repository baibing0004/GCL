using System;
using System.Collections.Generic;
using System.Text;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    /// <summary>
    /// 根据LogRecord数据源判断是否可以记录
    /// </summary>
    public class SourceTrigger : ATrigger {
        private string source;
        /// <summary>
        /// 数据源
        /// </summary>
        /// <param name="source"></param>
        public SourceTrigger(string source) {
            this.source = source.Trim().ToLower();
        }
        /// <summary>
        /// 对象类名作为数据源
        /// </summary>
        /// <param name="source"></param>
        public SourceTrigger(object source)
            : this(source.GetType().FullName) {
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            if ((e.GetPara(0) as LogRecord).GetSource().Trim().ToLower().Equals(this.source))
                return e;
            else
                return null;
        }

        public override void ReSet() {
        }
    }
}
