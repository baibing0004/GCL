using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
using System;

namespace GCL.Module.Trigger {
    /// <summary>
    /// 每次到期后都会以当前时间为基准计算下次运行时间!
    /// </summary>
    public class CronExpressionTrigger : ATrigger {
        private CronExpression.CronExpression expression;

        private DateTime passTime = DateTime.Now, lastTime = DateTime.Now;

        public DateTime PassTime {
            get { return passTime; }
        }

        public DateTime LastTime {
            get {
                return lastTime;
            }
        }

        private Random random;
        //随机秒
        private int randSec;
        public CronExpressionTrigger(string text) {
            this.expression = new GCL.Module.CronExpression.CronExpression(text);
            this.ReSet();
        }

        /// <summary>
        /// 增加随机参数用于处理定时Cron但又稍有偏差的情况
        /// </summary>
        /// <param name="text"></param>
        /// <param name="randSecond"></param>
        public CronExpressionTrigger(string text, int randSecond) : this(text) {
            this.random = new Random();
            this.randSec = randSecond;
        }
        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            if (this.passTime.CompareTo(DateTime.Now) <= 0) {
                this.ReSet();
                return new EventArg(this.passTime);
            }
            return null;
        }

        public override void ReSet() {
            if (passTime.CompareTo(DateTime.Now) <= 0) {
                lastTime = passTime;
                passTime = this.expression.Next(DateTime.Now);
                if (random != null)
                    passTime = passTime.AddSeconds(random.Next(this.randSec));
            }
        }
    }
}
