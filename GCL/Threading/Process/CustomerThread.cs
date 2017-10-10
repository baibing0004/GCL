using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Event;
using GCL.Collections;

namespace GCL.Threading.Process {

    /// <summary>
    /// ���ɶ���Ĵ������� ��Ҫ�̰߳�ȫ��
    /// </summary>
    /// <returns></returns>
    public delegate void CustomobjectDel(object sender, EventArg e, object data);

    /// <summary>
    /// �������߳�
    /// </summary>
    public class CustomerThread : CircleThread {

        /// <summary>
        /// �������ɶ���Ĵ���ʵ��
        /// </summary>
        private CustomobjectDel customobject;

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
        /// CreateobjectDel�����̺߳ţ����ƶ��ж��� �����µĴ����߶��� Ĭ�ϵȴ�������
        /// </summary>
        /// <param name="cod">������ò�Ʒ���������</param>
        /// <param name="num">�̺߳�</param>
        /// <param name="queue">���ƶ��ж���</param>
        public CustomerThread(CustomobjectDel cod, int num, LimitQueue queue)
            : this(cod, num, queue, 500) {
        }

        /// <summary>
        /// CreateobjectDel�����̺߳ţ����ƶ��ж��󣬵ȴ�ʱ�� �����µĴ����߶���
        /// </summary>
        /// <param name="cod">������ò�Ʒ���������</param>
        /// <param name="num">�̺߳�</param>
        /// <param name="queue">���ƶ��ж���</param>
        /// <param name="waitTime">�ȴ�ʱ��</param>
        public CustomerThread(CustomobjectDel cod, int num, LimitQueue queue, int waitTime)
            : base() {
            this.protRun = this.CustomTRun;
            this.customobject = cod;
            this.SetNum(num);
            this.queue = queue;
            this.waitTime = waitTime;
        }

        /// <summary>
        /// �������Ѷ���Ĺ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CustomTRun(object sender, EventArg e) {
            object data = null;

            while (this.IsCanRun())
                try {
                    lock (this.queue) {
                        data = this.queue.Peek();
                        if (Tool.IsEnable(e))
                            this.queue.Dequeue();
                    }
                    break;
                } catch {
                    if (this.HasWaitTime())
                        Tool.ObjectWait(this.queue, waitTime);
                    else
                        Tool.ObjectWait(queue);
                }

            if (!Tool.IsEnable(data))
                this.SetCanRun(false);
            else
                this.Customobject(sender, e, data);

        }

        /// <summary>
        /// �������Դ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="data"></param>
        public virtual void Customobject(object sender, EventArg e, object data) {
            this.customobject(sender, e, data);
        }
    }
}
