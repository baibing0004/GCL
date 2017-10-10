using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using GCL.Collections;
using GCL.Common;
using GCL.Event;
using GCL.Threading;

namespace GCL.Net {

    public class ServerThread:CircleThread {
        private Socket socket = null;
        private EndPoint point = null;
        private LimitNum num = null;
        ServerEventHandle del;

        /// <summary>
        /// ���ݲ���ʵ���������߳�
        /// </summary>
        /// <param name="port">�����˿�</param>
        public ServerThread(ServerEventHandle del,int port)
            : this(del,ProtocolType.Tcp,port) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳�
        /// </summary>
        /// <param name="port">�����˿�</param>
        /// <param name="size">���Ƽ���������</param>
        public ServerThread(ServerEventHandle del,int port,int size)
            : this(del,ProtocolType.Tcp,port,size) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳� Ĭ�ϼ�������127.0.0.1
        /// </summary>
        /// <param name="type">����Э������</param>
        /// <param name="port">�˿�</param>
        public ServerThread(ServerEventHandle del,ProtocolType type,int port)
            : this(del,new Socket(AddressFamily.InterNetwork,SocketType.Stream,type),new IPEndPoint(IPAddress.Parse("127.0.0.1"),port),new LimitNum()) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳� Ĭ�ϼ�������127.0.0.1
        /// </summary>
        /// <param name="type">����Э������</param>
        /// <param name="port">�˿�</param>
        /// <param name="size">����������Ŀ</param>
        public ServerThread(ServerEventHandle del,ProtocolType type,int port,int size)
            : this(del,new Socket(AddressFamily.InterNetwork,SocketType.Stream,type),new IPEndPoint(IPAddress.Parse("127.0.0.1"),port),size) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳�
        /// </summary>
        /// <param name="socket">Ҫ������Socket</param>
        /// <param name="point">�׽��ֹ���</param>
        public ServerThread(ServerEventHandle del,Socket socket,EndPoint point)
            : this(del,socket,point,new LimitNum()) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳�
        /// </summary>
        /// <param name="socket">Ҫ������Socket</param>
        /// <param name="point">�׽��ֹ���</param>
        /// <param name="size">����������Ŀ</param>
        public ServerThread(ServerEventHandle del,Socket socket,EndPoint point,int size)
            : this(del,socket,point,new LimitNum(size)) {
        }

        /// <summary>
        /// ���ݲ���ʵ���������߳�
        /// </summary>
        /// <param name="socket">Ҫ������Socket</param>
        /// <param name="point">�׽��ֹ���</param>
        /// <param name="num">����������Ŀ</param>
        protected ServerThread(ServerEventHandle del,Socket socket,EndPoint point,LimitNum num) {
            this.del = del;
            this.socket = socket;
            this.point = point;
            this.num = num;
            this.protRun = new Run(this.proRun);
        }

        public virtual void proRun(object sender,EventArg e) {
            if(this.IsFirst()) {
                this.socket.Bind(this.point);
            }

            try {
                this.del(this,new ServerEventArg(this.socket.Accept()));                
            } catch(Exception ex) {
                this.ReadExceptionThrowen(ex);
            }
        }

        /// <summary>
        /// ��ȡSocket����ʱ����������ʹ��e.SetCancle(true)�����ж��Ƿ���Ժ��� Ĭ����true ����������ֶ�����False ��ôҲ�����׳������ж��̡߳�
        /// </summary>
        public event EventHandle ReadExceptionThrowenEvent;

        /// <summary>
        /// ��ȡSocket����ʱ����������ʹ��e.SetCancle(true)�����ж��Ƿ���Ժ��� Ĭ����False ����������ֶ�����False ��ôҲ�����׳������ж��̡߳�
        /// </summary>
        /// <param name="ex"></param>
        public virtual void ReadExceptionThrowen(Exception ex) {
            ThreadEventArg e = new ThreadEventArg(ex,this);
            EventArg.CallEventSafely(ReadExceptionThrowenEvent,this,e);
            if(!e.GetCancle(true))
                throw ex;
        }
    }
}
