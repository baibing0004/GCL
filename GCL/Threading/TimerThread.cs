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
        /// ���ش˴����е�ʱ��
        /// </summary>
        /// <returns></returns>
        public DateTime GetNow() {
            return (DateTime)this.GetPara(0);
        }

        /// <summary>
        /// ���ش˴����е�ʱ��
        /// </summary>
        /// <returns></returns>
        public DateTime Now {
            get {
                return GetNow();
            }
        }

        /// <summary>
        /// �����´����е�ʱ��
        /// </summary>
        /// <returns></returns>
        public DateTime GetNext() {
            return (DateTime)this.GetPara(1);
        }

        /// <summary>
        /// �����´����е�ʱ��
        /// </summary>
        public DateTime Next {
            get {
                return GetNext();
            }
        }
    }

    public class TimerThread : CircleThread {

        /// <summary>
        /// �û�����ĵȴ�ʱ�� ����
        /// </summary>
        private int waitTime;

        /// <summary>
        /// �û����õĵȴ�ʱ��
        /// </summary>
        /// <returns></returns>
        public int GetWaitTime() {
            return waitTime;
        }

        public void SetWaitTime(int data) {
            if (data <= 0)
                throw new IndexOutOfRangeException("���������ֵ!");
            this.waitTime = data;
            // ��ʼ������            
            if (this.waitTime >= 10 * 1000)
                // ��������ÿ��10�뻽��һ��
                this.useTime = 10 * 1000;
            else
                this.useTime = this.waitTime;
        }

        /// <summary>
        /// �û����õĵȴ�ʱ��
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
        /// ����ʹ�õĵȴ�ʱ�� ����
        /// </summary>
        private int useTime;

        /// <summary>
        /// ʹ���û����õļ���ȴ�ʱ���½���ʱ���߳� ����
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
        /// �����´��¼�����ʱ��
        /// </summary>
        private DateTime nextTime = DateTime.Now;

        /// <summary>
        /// ��ȡ�´��¼�������ʱ��
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
                // ����һ��ʱ�份��
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
