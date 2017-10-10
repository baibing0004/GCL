using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Module.CronExpression {
    /// <summary>
    /// 日期保存者 用于CronNode上下传递时间
    /// </summary>
    class DateTimeContainer {
        private DateTime time;
        public void SetDateTime(DateTime time) {
            this.time = time;
        }
        public DateTime GetDateTime() {
            return this.time;
        }
    }
}
