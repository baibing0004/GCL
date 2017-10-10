using System;
using GCL.Threading;
using GCL.Event;
namespace GCL.Module.Trigger {
    /// <summary>
    /// 循环触发器 按照时间（毫秒）定期触发。在SetEnable时取消
    /// 
    /// @author 白冰
    /// @version 2.0.81209.1
    /// </summary>
    public class TimerTriggerProxy : TriggerProxy, System.IDisposable {

        /// <summary>
        /// @param trigger
        /// </summary>
        public TimerTriggerProxy(ATrigger trigger, int waitTime)
            : base(trigger) {
            this.timer = new TimerThread(new Run(TimerTriggerProxy_TimeEvent), waitTime);
            this.timer.FinallyDoEvent += new EventHandle(timer_FinallyDoEvent);
            this.SetEnable(false);
        }

        protected void TimerTriggerProxy_TimeEvent(object sender, EventArg e) {
            try {
                this.Taste();
            } catch (Exception e1) {
            }
        }

        void timer_FinallyDoEvent(object sender, EventArg e) {
            try {
                SetEnable(false);
            } catch (Exception e1) {
            }
        }

        ///<summary>
        /// （非 Javadoc）
        /// 
        /// @see GCL.module.trigger.TriggerProxy#setEnable(bool)
        /// </summary>
        public override void SetEnable(bool isEnable) {
            bool baseEnable = base.IsEnable;
            base.SetEnable(isEnable);
            if (baseEnable != isEnable) {
                if (isEnable)
                    timer.Start();
                else
                    timer.SetCanRun(isEnable);
            }

        }



        private TimerThread timer;

        public TimerThread Timer {
            get { return timer; }

        }

        public TimerThread GetTimer() {
            return timer;
        }

        /// <summary>
        /// @return waitTime
        /// </summary>
        public int GetWaitTime() {
            return this.timer.GetWaitTime();
        }

        /// <summary>
        /// @param waitTime
        ///            要设置的 waitTime
        /// </summary>
        public void SetWaitTime(int waitTime) {
            this.timer.SetWaitTime(waitTime);
        }
    }
}
