using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections;
using GCL.Threading;

namespace GCL.Threading.Process {

    /// <summary>
    /// �������߳�
    /// </summary>
    public class CreaterThread : CircleThread {

        /// <summary>
        /// �������ɶ���Ĵ���ʵ��
        /// </summary>
        private CreaterAction createobject;

        /// <summary>
        /// ��Ҫ������߳� ������������� �ᶨʱ�ȴ���
        /// </summary>
        private LimitQueue queue;

        /// <summary>
        /// �ȴ�ʱ�� ���� Ĭ�ϰ�����
        /// </summary>
        private int waitTime = 500;


        /// <summary>
        /// �ж��Ƿ��еȴ�ʱ��
        /// </summary>
        /// <returns></returns>
        public bool HasWaitTime() {
            return this.waitTime > -1;
        }


        /// <summary>
        /// ���� waitTime��
        /// </summary>
        /// <returns></returns>
        public int GetWaitTime() {
            return waitTime;
        }
        /// <summary>
        /// �ȴ�ʱ��
        /// </summary>
        public int WaitTime {
            get {
                return GetWaitTime();
            }
        }

        /// <summary>
        /// CreaterAction�����̺߳ţ����ƶ��ж��� �����µĴ����߶��� Ĭ�ϵȴ�������
        /// </summary>
        /// <param name="cod">������ò�Ʒ���������</param>
        /// <param name="num">�̺߳�</param>
        /// <param name="queue">���ƶ��ж���</param>
        public CreaterThread(CreaterAction cod, int num, LimitQueue queue)
            : this(cod, num, queue, 500) {
        }

        /// <summary>
        /// CreaterAction�����̺߳ţ����ƶ��ж��󣬵ȴ�ʱ�� �����µĴ����߶���
        /// </summary>
        /// <param name="cod">������ò�Ʒ���������</param>
        /// <param name="num">�̺߳�</param>
        /// <param name="queue">���ƶ��ж���</param>
        /// <param name="waitTime">�ȴ�ʱ��</param>
        public CreaterThread(CreaterAction cod, int num, LimitQueue queue, int waitTime)
            : base() {
            this.protRun = this.CreateTRun;
            this.createobject = cod;
            this.SetNum(num);
            this.queue = queue;
            this.waitTime = waitTime;
        }

        /// <summary>
        /// ���ڴ�������Ĺ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CreateTRun(object sender, EventArg e) {
            object data = this.Createobject(sender, e);
            while (this.IsCanRun())
                try {
                    this.queue.Enqueue(data);
                    break;
                } catch {
                    if (this.HasWaitTime())
                        Tool.ObjectWait(this.queue, waitTime);
                    else
                        Tool.ObjectWait(queue);
                }

            if (!Tool.IsEnable(data))
                this.SetCanRun(false);
        }

        /// <summary>
        /// �������������ɶ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual object Createobject(object sender, EventArg e) {
            return this.createobject(sender, e);
        }
    }
}
