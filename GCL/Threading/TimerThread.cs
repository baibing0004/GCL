using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
using GCL.Common;

namespace GCL.Threading {
    public class TimerEventArg : EventArg {
        public TimerEventArg(DateTime now, DateTime nextTime)
            : base(new object[] { now, nextTime }) {
        }

        /// <summary>
        /// 返回此次运行的时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetNow() {
            return (DateTime)this.GetPara(0);
        }

        /// <summary>
        /// 返回此次运行的时间
        /// </summary>
        /// <returns></returns>
        public DateTime Now {
            get {
                return GetNow();
            }
        }

        /// <summary>
        /// 返回下次运行的时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetNext() {
            return (DateTime)this.GetPara(1);
        }

        /// <summary>
        /// 返回下次运行的时间
        /// </summary>
        public DateTime Next {
            get {
                return GetNext();
            }
        }
    }

    public class TimerThread : CircleThread {

        /// <summary>
        /// 用户定义的等待时间 毫秒
        /// </summary>
        private int waitTime;

        /// <summary>
        /// 用户设置的等待时间
        /// </summary>
        /// <returns></returns>
        public int GetWaitTime() {
            return waitTime;
        }

        public void SetWaitTime(int data) {
            if (data <= 0)
                throw new IndexOutOfRangeException("错误的设置值!");
            this.waitTime = data;
            // 初始化变量            
            if (this.waitTime >= 10 * 1000)
                // 设置最慢每隔10秒唤醒一次
                this.useTime = 10 * 1000;
            else
                this.useTime = this.waitTime;
        }

        /// <summary>
        /// 用户设置的等待时间
        /// </summary>
        public int WaitTime {
            get {
                return this.GetWaitTime();
            }
            set {
                this.SetWaitTime(value);
            }
        }
        /// <summary>
        /// 程序使用的等待时间 毫秒
        /// </summary>
        private int useTime;

        /// <summary>
        /// 使用用户设置的间隔等待时间新建定时器线程 毫秒
        /// </summary>
        /// <param name="waitTime"></param>
        public TimerThread(Run protRun, int waitTime)
            : base(protRun) {
            this.SetWaitTime(waitTime);
        }

        public TimerThread(int waitTime)
            : base() {
            this.SetWaitTime(waitTime);
        }

        protected TimerThread() : this(1000) { }

        /// <summary>
        /// 定义下次事件发生时间
        /// </summary>
        private DateTime nextTime = DateTime.Now;

        /// <summary>
        /// 获取下次事件发生的时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime GetNextTime(DateTime time) {
            return time.AddMilliseconds(this.waitTime);
        }

        public override void CircleRun() {            
            if (this.nextTime.CompareTo(DateTime.Now) <= 0) {
                DateTime _time = this.nextTime;
                this.nextTime = this.GetNextTime(DateTime.Now);
                this.TimerRun(_time, this.nextTime);
            } else
                // 休眠一段时间唤醒
                Tool.ObjectSleep(this.useTime);

            if (this.IsFirst()) {
                this.nextTime = this.GetNextTime(DateTime.Now);
            }
        }

        public virtual void TimerRun(DateTime now, DateTime next) {
            this.protRun(this, new TimerEventArg(now, next));
        }
    }
}
