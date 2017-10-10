using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections.Pool;
namespace GCL.Threading.Process {

    public delegate void CustomerAction(object sender, Event.EventArg e, object value);

    public class CustomerPoolThread:Threading.CircleThread{
        private CustomerAction _customeraction;

        public CustomerAction Customer {
            get { return _customeraction; }
        }

        private object value = null;
        public void SetValue(object data) {
            if (!Tool.IsEnable(value))
                value = data;
            else
                throw new InvalidOperationException("其中的对象还没有处理完毕！");
        }
        public object GetValue() {
            return value;
        }
        public object Value {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        private Pool pool;
        public CustomerPoolThread(CustomerAction del, int num, int waitTime, Pool pool)
            : base() {
            this.SetRun(new Run(this.TimerAction));
            this.SetNum(num);
            this.pool = pool;
            this._customeraction = del;
        }

        public virtual void TimerAction(object sender, Event.EventArg e) {
            if (Tool.IsEnable(value))
                try {
                    _customeraction(sender, e, this.value);
                    this.value = null;
                    this.pool.Set(this);
                } catch (IndexOutOfRangeException) {
                } catch (Exception ex) {
                    this.CallThreadEventSimple(203, new object[] { ex.ToString(), ex });
                }
        }
    }
}
