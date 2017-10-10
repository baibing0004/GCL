using System;
using System.Collections.Generic;
using System.Text;
using GCL.IO.Log;

namespace GCL.Threading.Process {
    /// <summary>
    /// ����ΪIProcess�ļ̳����ṩһЩ�����ͳһ��ʵ�ַ���
    /// ע�����INIT���½�����Դ ���������CheckDispose()���� �ͷ���Դ �������������޷��رճ���
    /// </summary>
    public abstract class AbstractProcess : IProcess {

        ///// <summary>
        /////  ����ΪIProcess�ļ̳����ṩһЩ�����ͳһ��ʵ�ַ���
        ///// </summary>
        ///// <param name="type">�ⲿΨһһ�ο�����������־����Ļ���</param>
        //public AbstractProcess(LogType type)
        //    : this() {
        //    this.logType = type;
        //}

        /// <summary>
        ///  ����ΪIProcess�ļ̳����ṩһЩ�����ͳһ��ʵ�ַ���
        /// </summary>
        /// <param name="type">�ⲿΨһһ�ο�����������־����Ļ���</param>
        public AbstractProcess() {
            this.ProcessEvent += new ProcessEventHandle(ProcessEventArg._ProcessEventHandleDefault);
        }

        /// <summary>
        /// �������� ���յ��ò��׳��¼� ���Ҫ��ǰ�����¼����������ﴦ��
        /// </summary>
        /// <param name="e"></param>
        protected virtual void CallProcessEventSimple(ProcessEventArg e) {
            if (e.Level == Event.EventLevel.Importent) {
                if (e.State != ProcessState.EXCEPTION)
                    state = e.State;
                else if (state == ProcessState.INIT)
                    state = ProcessState.NOSTATE;
                else if (state == ProcessState.START)
                    state = ProcessState.STOP;
                if (state == ProcessState.STOP)
                    CheckDispose(true);
            }
            if (e.Level == GCL.Event.EventLevel.Importent || e.LogType <= this.BaseLogType)
                ProcessEventArg.CallEventSafely(this.ProcessEvent, this, e);
        }

        /// <summary>
        /// ���㴦���� ���ڳ�����������״̬
        /// </summary>
        /// <param name="state"></param>
        protected virtual void CallProcessEventSimple(ProcessState state) {
            this.CallProcessEventSimple(new ProcessEventArg(state));
        }

        /// <summary>
        /// ���㴦���� ���ڳ�������״̬�������Լ�һ�����ӵĶ���
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(Exception ex, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(ProcessState.EXCEPTION, ex, LogType.ERROR, obj));
        }

        /// <summary>
        /// ���㴦���� ���ڳ�������״̬�������Լ�һ�����ӵĶ���
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(Exception ex, object[] obj) {
            this.CallProcessEventSimple(new ProcessEventArg(ProcessState.EXCEPTION, ex, LogType.ERROR, obj));
        }

        /// <summary>
        /// ���㴦���� ˵���¼����Ѿ�����һ������
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, obj));
        }

        /// <summary>
        ///  ���㴦���� ˵���¼����Ѿ�����һ�����
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallProcessEventSimple(int eventNum, object[] objs) {
            this.CallProcessEventSimple(new ProcessEventArg(eventNum, objs));
        }

        /// <summary>
        /// ���㴦���� ˵���¼����Ѿ�����һ������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventNum"></param>
        /// <param name="obj"></param>
        protected virtual void CallProcessEventSimple(LogType type, int eventNum, object obj) {
            this.CallProcessEventSimple(new ProcessEventArg(type, eventNum, obj));
        }

        /// <summary>
        ///  ���㴦���� ˵���¼����Ѿ�����һ�����
        /// </summary>
        /// <param name="eventNum"></param>
        /// <param name="objs"></param>
        protected virtual void CallProcessEventSimple(LogType type, int eventNum, object[] objs) {
            this.CallProcessEventSimple(new ProcessEventArg(type, eventNum, objs));
        }

        /// <summary>
        /// Ĭ��ΪInfo
        /// </summary>
        private LogType logType = LogType.All;

        /// <summary>
        /// ��ȡ��Ϣ��־�ȼ�
        /// </summary>
        /// <returns></returns>
        public LogType GetLogType() {
            return logType;
        }

        /// <summary>
        /// ����ı���־�ȼ�
        /// </summary>
        /// <param name="type"></param>
        protected void SetLogType(LogType type) {
            this.logType = type;
        }

        /// <summary>
        /// ��Ϣ��־�ȼ�
        /// </summary>
        public LogType BaseLogType {
            get { return GetLogType(); }
        }

        private ProcessState state = ProcessState.NOSTATE;

        /// <summary>
        /// ����״̬
        /// </summary>
        /// <returns></returns>
        public ProcessState GetState() {
            return state;
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        public ProcessState State {
            get {
                return GetState();
            }
        }

        #region IProcess Members

        public event ProcessEventHandle ProcessEvent;

        public abstract void Init();

        public abstract void Start();

        /// <summary>
        /// ����CheckStop��ȷ�� �Ƿ񴥷�Stop״̬����Dispose״̬
        /// </summary>
        public abstract void Stop();

        #endregion

        #region IDisposable Members

        public virtual void Dispose() {
            try {
                this.isDispose = true;
                this.Stop();
                CheckDispose();
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, this.GetType().Name + "�رճ����쳣��");
            }
        }

        protected abstract bool CheckStop();

        protected virtual void CheckDispose() {
            this.CheckDispose(CheckStop() || (canDisposeByReady && this.GetState() <= ProcessState.READY));
        }

        private bool canDisposeByReady = true;

        public bool CanDisposeByReady {
            get { return canDisposeByReady; }
            set { canDisposeByReady = value; }
        }


        protected bool isDispose = false;
        protected virtual void CheckDispose(bool dispose) {
            if (this.isDispose && dispose)
                this.CallProcessEventSimple(ProcessState.DISPOSE);
        }

        #endregion
    }
}
