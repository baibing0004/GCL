using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GCL.Common;
using GCL.Event;
using GCL.Collections;

namespace GCL.Threading.Process {

    /// <summary>
    /// ��Ҫ��������������/������ģʽ���� ���ṩ����ģʽ��ʽ�Ľ�����
    /// </summary>
    public class CCProcess : IProcess {

        /// <summary>
        /// ��������ķ�������
        /// </summary>
        protected CreaterAction crod;

        /// <summary>
        /// ���Ѷ���ķ�������
        /// </summary>
        protected CustomobjectDel cuod;

        /// <summary>
        /// ��������Ŀ
        /// </summary>
        protected int createrNum;

        /// <summary>
        /// �����������Ŀ
        /// </summary>
        /// <returns></returns>
        public int GetCreaterNum() {
            return createrNum;
        }

        /// <summary>
        /// ��������Ŀ
        /// </summary>
        public int CreaterNum {
            get {
                return GetCreaterNum();
            }
        }

        /// <summary>
        /// ��������Ŀ
        /// </summary>
        protected int customerNum;

        /// <summary>
        /// �����������Ŀ
        /// </summary>
        /// <returns></returns>
        public int GetCustomerNum() {
            return customerNum;
        }

        /// <summary>
        /// ��������Ŀ
        /// </summary>
        public int CustomerNum {
            get {
                return GetCustomerNum();
            }
        }
        /// <summary>
        /// ���ݶ������������߷��� �����߷��� ������Ϊ10
        /// </summary>
        /// <param name="crod"></param>
        /// <param name="creater"></param>
        /// <param name="cuod"></param>
        /// <param name="customer"></param>
        public CCProcess(CreaterAction crod, int creater, CustomobjectDel cuod, int customer)
            : this(crod, creater, cuod, customer, 10) {
        }

        /// <summary>
        /// ���ݶ������������߷��� �����߷���
        /// </summary>
        /// <param name="crod"></param>
        /// <param name="creater"></param>
        /// <param name="cuod"></param>
        /// <param name="customer"></param>
        public CCProcess(CreaterAction crod, int creater, CustomobjectDel cuod, int customer, int capacity) {
            if (creater <= 0 | customer <= 0 | capacity <= 0)
                throw new IndexOutOfRangeException("���������߶��У������߶��У���Ϣ���еĳ��ȶ�����С�ڵ���0");
            this.crod = crod;
            this.createrNum = creater;
            this.cuod = cuod;
            this.customerNum = customer;
            this.capacity = capacity;
        }

        /// <summary>
        /// ������д�С
        /// </summary>
        protected int capacity = 0;

        protected CreaterThread[] crT;
        protected CustomerThread[] cuT;

        /// <summary>
        /// ���õ����ƶ�����Ϣ������
        /// </summary>
        protected LimitQueue queue;

        #region IProcess Members

        public event ProcessEventHandle ProcessEvent;

        protected virtual void CallProcessEventSimple(ProcessEventArg e) {
            ProcessEventArg.CallEventSafely(ProcessEvent, this, e);
        }

        protected virtual void CallProcessEventSimple(ProcessState state) {
            this.CallProcessEventSimple(new ProcessEventArg(state));
        }

        protected virtual void CallProcessEventSimple(ProcessState state, Exception ex, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(state, ex, obj));
        }

        protected virtual void CallProcessEventSimple(int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, obj));
        }

        /// <summary>
        /// �������̵߳ȴ�ʱ��
        /// </summary>
        protected int createWaitTime = 500;

        public int GetCreateWaitTime() {
            return this.createWaitTime;
        }

        public void SetCreateWaitTime(int value) {
            if (value <= 0)
                throw new System.IndexOutOfRangeException("�Բ��𣬵ȴ�ʱ�䲻��С�ڵ����㣡");
            if (this.createWaitTime != 500)
                throw new InvalidOperationException("�ȴ�ʱ���Ѿ����� �������ٴ�����");
            this.createWaitTime = value;
        }

        /// <summary>
        /// �����ߵ�������ʱ�ȴ�ʱ�� Ĭ�ϰ����� ��ҪΪ�˶Թرղ������������ķ�Ӧ
        /// </summary>
        public int CreateWaitTime {
            get {
                return this.GetCreateWaitTime();
            }
            set {
                this.SetCreateWaitTime(value);
            }
        }

        /// <summary>
        /// �������̵߳ȴ�ʱ�� Ĭ�ϰ����� ��ҪΪ�˶Թرղ������������ķ�Ӧ
        /// </summary>
        protected int customWaitTime = 500;


        public int GetCustomWaitTime() {
            return this.customWaitTime;
        }

        public void SetCustomWaitTime(int value) {
            if (value <= 0)
                throw new System.IndexOutOfRangeException("�Բ��𣬵ȴ�ʱ�䲻��С�ڵ����㣡");
            if (this.customWaitTime != 500)
                throw new InvalidOperationException("�ȴ�ʱ���Ѿ����� �������ٴ�����");
            this.customWaitTime = value;
        }

        /// <summary>
        /// �����ߵ�������ʱ�ȴ�ʱ�� Ĭ�ϰ����� ��ҪΪ�˶Թرղ������������ķ�Ӧ
        /// </summary>
        public int CustomWaitTime {
            get {
                return this.GetCustomWaitTime();
            }
            set {
                this.SetCustomWaitTime(value);
            }
        }
        /// <summary>
        /// ��ʼ������
        /// ��� ���õ���Ϣ���еĴ�С
        /// ��� �����߶��е��½�
        /// ��� �����߶��е��½�
        /// </summary>
        /// <exception >���п����׳��Ĵ���</exception>
        public virtual void Init() {
            lock (this) {
                try {

                    //˵����ʼ��ʼ��״̬
                    this.CallProcessEventSimple(ProcessState.INIT);

                    //�½����õ���Ϣ����
                    this.queue = new LimitQueue(new System.Collections.Queue(), capacity);

                    //�½������߶���
                    this.crT = new CreaterThread[this.createrNum];
                    for (int w = 0; w < this.createrNum; w++) {
                        this.crT[w] = new CreaterThread(this.crod, w, this.queue, this.createWaitTime);
                        this.crT[w].ExceptionThrowenEvent += new EventHandle(CCProcess_CR_ExceptionThrowenEvent);
                        this.crT[w].FinallyDoEvent += new EventHandle(CCProcess_CR_FinallyDoEvent);
                    }

                    //�½������߶���
                    this.cuT = new CustomerThread[this.customerNum];
                    for (int w = 0; w < this.customerNum; w++) {
                        this.cuT[w] = new CustomerThread(this.cuod, w, this.queue, this.customWaitTime);
                        this.cuT[w].ExceptionThrowenEvent += new EventHandle(CCProcess_CU_ExceptionThrowenEvent);
                        this.cuT[w].FinallyDoEvent += new EventHandle(CCProcess_CU_FinallyDoEvent);
                    }

                    //˵����ʼ���ɹ�״̬
                    this.CallProcessEventSimple(ProcessState.READY);

                    return;
                } catch (Exception ex) {
                    //�׳���ʼ������
                    this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.INIT);
                }
            }
        }

        #region ���߳��¼�����ɽ����¼�

        private void CCProcess_CR_FinallyDoEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(201, string.Format("{0}���������̴߳�����ɽ���!", ((ProtectThread)sender).Num));
            this.DecideStop();
        }

        private void CCProcess_CR_ExceptionThrowenEvent(object sender, EventArg e) {
            ThreadEventArg te = (ThreadEventArg)e;
            ProcessEventArg pro = new ProcessEventArg(GCL.IO.Log.LogType.ERROR, 203, new object[] { string.Format("{0}���������̷߳�������!�������Ӱ��������������ʹ��e.Cancle=true", te.GetThread().Num), te.GetException() });
            this.CallProcessEventSimple(pro);
            //�ж��Ƿ�������������
            if (pro.GetCancle(true))
                ((ProtectThread)sender).CreateThread().Start();
        }

        private void CCProcess_CU_FinallyDoEvent(object sender, EventArg e) {
            this.CallProcessEventSimple(202, string.Format("{0}���������̴߳�����ɽ���!", ((ProtectThread)sender).Num));
            this.DecideStop();
        }

        private void CCProcess_CU_ExceptionThrowenEvent(object sender, EventArg e) {
            ThreadEventArg te = (ThreadEventArg)e;
            ProcessEventArg pro = new ProcessEventArg(GCL.IO.Log.LogType.ERROR, 204, new object[] { string.Format("{0}���������̷߳�������! �������Ӱ��������������ʹ��e.Cancle=true", te.GetThread().Num), te.GetException() });
            this.CallProcessEventSimple(pro);
            //�ж��Ƿ�������������
            if (pro.GetCancle(true))
                ((ProtectThread)sender).CreateThread().Start();
        }

        #endregion

        /// <summary>
        /// �����Ƿ�ֹͣ
        /// </summary>
        private void DecideStop() {

            if (this.isStop)
                return;

            foreach (CircleThread thread in crT) {
                if (thread.IsCanRun())
                    return;
            }

            foreach (CircleThread thread in cuT) {
                if (thread.IsCanRun())
                    return;
            }

            this.isStop = true;
            this.CallProcessEventSimple(ProcessState.STOP);
        }

        /// <summary>
        /// ������������ ������������ ������ͣ/��������
        /// </summary>
        public virtual void Start() {
            try {
                this.isStop = false;
                //������������
                for (int w = 0; w < this.cuT.Length; w++) {
                    //�ж��߳��Ƿ���������
                    if (cuT[w].IsCanRun() && Tool.IsEnable(cuT[w].Thread) && cuT[w].Thread.ThreadState == ThreadState.Running) {
                        //����߳�����������ô��������
                    } else {
                        //�������̲߳��������е����߳����������� �����߳��������е����߳����в������Ļ���û�������� ����Ҫ��������
                        //�߳�������������Ҫǿ�йر�
                        if (Tool.IsEnable(cuT[w].Thread) && cuT[w].Thread.ThreadState == ThreadState.Running) {
                            cuT[w].SetCanRun(false);
                            Tool.StopThread(cuT[w].Thread);
                        }
                        cuT[w].SetCanRun(true);
                        cuT[w].CreateThread().Start();
                    }
                }

                //�����ߺ�����
                for (int w = 0; w < this.crT.Length; w++) {
                    //�ж��߳��Ƿ���������
                    if (crT[w].IsCanRun() && Tool.IsEnable(crT[w].Thread) && crT[w].Thread.ThreadState == ThreadState.Running) {
                        //����߳�����������ô��������
                    } else {
                        //�������̲߳��������е����߳����������� �����߳��������е����߳����в������Ļ���û�������� ����Ҫ��������
                        //�߳�������������Ҫǿ�йر�
                        if (Tool.IsEnable(crT[w].Thread) && crT[w].Thread.ThreadState == ThreadState.Running) {
                            crT[w].SetCanRun(false);
                            Tool.StopThread(crT[w].Thread);
                        }
                        crT[w].SetCanRun(true);
                        crT[w].CreateThread().Start();
                    }
                }
                this.CallProcessEventSimple(ProcessState.START);
                return;
            } catch (Exception ex) {
                //�׳���������
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.START);
            }
        }
        /// <summary>
        /// ˵���ǲ�������ֹͣ �Ӷ���ֹ�������
        /// </summary>
        private bool isStop = false;
        public virtual void Stop() {
            try {
                this.isStop = true;
                for (int w = 0; w < crT.Length; w++)
                    crT[w].SetCanRun(false);

                for (int w = 0; w < cuT.Length; w++)
                    cuT[w].SetCanRun(false);

                this.CallProcessEventSimple(ProcessState.STOP);
                return;
            } catch (Exception ex) {
                //�׳���������
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.STOP);
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose() {
            try {
                this.isStop = true;
                for (int w = 0; w < crT.Length; w++) {
                    crT[w].SetCanRun(false);
                    Tool.StopThread(crT[w].Thread);
                }

                for (int w = 0; w < cuT.Length; w++) {
                    cuT[w].SetCanRun(false);
                    Tool.StopThread(cuT[w].Thread);
                }

                this.queue.Clear();
                this.CallProcessEventSimple(ProcessState.DISPOSE);
                return;
            } catch (Exception ex) {
                //�׳���������
                this.CallProcessEventSimple(ProcessState.EXCEPTION, ex, ProcessState.DISPOSE);
            }
        }

        #endregion
    }
}
