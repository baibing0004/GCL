using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;

namespace GCL.Module.Trigger {
    /// <summary>
    ///  ������ԭ���������� ��ĳ�������߱�ʱ�������¼�����
    /// </summary>
    public abstract class ATrigger:IDisposable {

        public event EventHandle TriggerEvent;

        private bool isEnable = false;

        public ATrigger(bool isEnable) {
            this.TriggerEvent += new EventHandle(EventArg._EventHandleDefault);
            this.isEnable = isEnable;
        }

        public ATrigger()
            : this(true) {
        }

        /// <summary>
        /// �����Ƿ�������
        /// </summary>
        /// <param name="IsEnable"></param>
        public virtual void SetEnable(bool isEnable) {
            this.isEnable = isEnable;
        }

        public virtual bool IsEnable {
            get { return isEnable; }
            set { SetEnable(value); }
        }

        /// <summary>
        /// ��Ҫʵ�ֵķ��� ����н����ô�ᴥ���¼��������������С�
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract EventArg Attempt(EventArg e);

        /// <summary>
        /// ���Ը��� �Դ��еĴ���ʽ������������¼������ҿ��Բ������¼�
        /// </summary>
        /// <param name="e"></param>
        protected virtual void CallTriggerEventSafely(object sender, EventArg e) {
            EventArg.CallEventSafely(this.TriggerEvent, sender, e);
        }

        /// <summary>
        /// �����Ƿ���Դ����¼�
        /// </summary>
        public virtual bool Taste() {
            return this.Taste(null);
        }

        /// <summary>
        /// �����Ƿ���Դ����¼�
        /// </summary>
        /// <param name="e"></param>
        public virtual bool Taste(EventArg e) {
            if (!this.IsEnable)
                return false;
            EventArg result = this.Attempt(e);
            if (Tool.IsEnable(result)) {
                CallTriggerEventSafely(this, result);
                return true;
            } else
                return false;
        }

        public abstract void ReSet();

        #region IDisposable Members

        public virtual void Dispose() {
            this.SetEnable(false);
        }

        #endregion

        ~ATrigger(){
            this.Dispose();
        }
    }
}
