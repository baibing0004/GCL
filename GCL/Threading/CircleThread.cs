using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Event;
using GCL.Common;

namespace GCL.Threading {
    public class CircleThread:ProtectThread,IDisposable {

        public CircleThread(Run protRun)
            : base(protRun) {
            this.FinalRunEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// ֻ�ڼ̳�ʱ��Ч ������Ҫ�Զ���protRun����
        /// </summary>
        public CircleThread()
            : base() {
            this.FinalRunEvent += new EventHandle(EventArg._EventHandleDefault);
        }

        /// <summary>
        /// �Ƿ���Լ�������
        /// </summary>
        private bool isCanRun = false;

        /// <summary>
        /// �Ƿ񲻹��û��Ƿ���ɲ�����������ֹ�ⲿ����
        /// </summary>
        private bool syn = false;

        /// <summary>
        /// �Ƿ��ǵ�һ��ִ��
        /// </summary>
        private bool first = false;

        /// <summary>
        /// ���������Ƿ���Լ������еĶ���
        /// </summary>
        private object key = DateTime.Now;

        /// <summary>
        /// ���ڼ��ش���
        /// </summary>
        private Exception exThrowen = null;

        /// <summary>
        /// �ڲ��߳�ʹ�÷��� ��¼�׳��Ĵ��󲢻������߳�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pt_ExceptionThrowenEvent(object sender,EventArg e) {
            this.exThrowen = ((ThreadEventArg)e).GetException();
            Tool.ObjectPulseAll(key);
        }

        /// <summary>
        /// �ڲ��߳�ʹ�÷��� �������߳�
        /// </summary>
        private void _pt_FinallyDoEvent(object sender,EventArg e) {
            Tool.ObjectPulseAll(key);
        }

        /// <summary>
        /// �ڲ��߳�ʹ�÷��� ��װ ���滻�����ڲ��̵߳���Ҫִ�з���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pt_protRun(object sender,EventArg e) {
            this.CircleRun();
        }


        /// <summary>
        /// ����ԭ��������
        /// </summary>
        protected override void ProtectedRun() {
            try {

                this.first = true;
                this.isCanRun = true;
                ProtectThread _pt = new ProtectThread(this._pt_protRun);

                //��������¼����ղ��Զ�����keyʹ���̼߳�������
                _pt.ExceptionThrowenEvent += new EventHandle(_pt_ExceptionThrowenEvent);

                //���������¼����ջ����Զ�����keyʹ���̼߳�������
                _pt.FinallyDoEvent += new EventHandle(this._pt_FinallyDoEvent);

                while (this.isCanRun) {
                    if (syn) {
                        exThrowen = null;
                        Thread _t = _pt.CreateThread();
                        lock (key) {
                            _t.IsBackground = true;
                            _t.Start();
                            Tool.ObjectWait(key);
                            this.first = false;
                        }

                        if (this.exThrowen != null)
                            throw exThrowen;

                        if (_t.ThreadState == ThreadState.Running) {
                            try {
                                _t.Interrupt();
                            } catch {
                            }
                            try {
                                _t.Abort();
                            } catch {
                            }
                        }
                    } else {
                        this.CircleRun();
                        this.first = false;
                    }
                }
                this.FinalRun();
            } catch (Exception ex) {
                this.isCanRun = false;
                throw ex;
            }
        }

        /// <summary>
        /// ����������ʹ��protRun�������
        /// </summary>
        public virtual void CircleRun() {
            base.protRun(this,new ThreadEventArg(this));
        }

        /// <summary>
        /// �Ƿ��ǵ�һ��ѭ��
        /// </summary>
        /// <returns></returns>
        public bool IsFirst() {
            return first;
        }

        /// <summary>
        /// ��������ִ���¼�
        /// </summary>
        public event EventHandle FinalRunEvent;

        /*
         * ���鸲�� �����߳�����ֹͣʱ�Ĵ������񣬱����ĳЩ��ı�����������Ĭ������ӡ�������
         * 
         * @throws Exception
         */

        /// <summary>
        /// ���鸲�� �����߳�����ֹͣʱ�Ĵ������񣬱����ĳЩ��ı�����������Ĭ������ӡ�������
        /// ��FinallyDo��������������������׳����� ����Throwen������׽
        /// </summary>
        protected void FinalRun() {
            Console.WriteLine(this.GetType().Name + " run end!");
            EventArg.CallEventSafely(FinalRunEvent,this,new ThreadEventArg(this));
        }

        /// <summary>
        /// �Ƿ�ͬ���ر�
        /// </summary>
        /// <returns>���� syn</returns>
        public bool IsSynClose() {
            return syn;
        }

        /*
          * �Ƿ�Ҫ��ͬ���ر�
          * 
          * @param syn
          *            Ҫ���õ� syn��
          */

        /// <summary>
        /// �Ƿ�Ҫ��ͬ���ر�
        /// </summary>
        /// <param name="syn">Ҫ���õ�</param>
        public void SetSynClose(bool syn) {
            this.syn = syn;
        }

        /// <summary>
        /// .Netд�� �����Ƿ���Լ���ִ�� ���Ҫ��ֹ�߳� ϣ��ʹ���������ΪFalse ��������˳���� finalRun �� finallyDo
        /// </summary>
        public bool CanRun {
            get {
                return IsCanRun();
            }
            set {
                SetCanRun(value);
            }
        }

        /// <summary>
        /// �Ƿ����ִ��
        /// </summary>
        /// <returns>�����Ƿ����ִ��</returns>
        public bool IsCanRun() {
            return this.isCanRun;
        }

        /// <summary>
        /// �����Ƿ���Լ���ִ�� ���Ҫ��ֹ�߳� ϣ��ʹ���������
        /// </summary>
        /// <param name="canRun">�Ƿ���Լ���ִ��</param>
        public void SetCanRun(bool canRun) {            
            this.isCanRun = canRun;
            if(!canRun)
                if(this.syn) {
                    Tool.ObjectPulseAll(key);
                }
        }

        #region IDisposable Members

        public void Dispose() {
            this.SetCanRun(false);
        }

        #endregion    
    
        ~CircleThread() {
            this.Dispose();
        }
    }
}
