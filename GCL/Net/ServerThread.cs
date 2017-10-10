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
        /// 根据参数实例化服务线程
        /// </summary>
        /// <param name="port">监听端口</param>
        public ServerThread(ServerEventHandle del,int port)
            : this(del,ProtocolType.Tcp,port) {
        }

        /// <summary>
        /// 根据参数实例化服务线程
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <param name="size">限制监听的数量</param>
        public ServerThread(ServerEventHandle del,int port,int size)
            : this(del,ProtocolType.Tcp,port,size) {
        }

        /// <summary>
        /// 根据参数实例化服务线程 默认监听本地127.0.0.1
        /// </summary>
        /// <param name="type">监听协议类型</param>
        /// <param name="port">端口</param>
        public ServerThread(ServerEventHandle del,ProtocolType type,int port)
            : this(del,new Socket(AddressFamily.InterNetwork,SocketType.Stream,type),new IPEndPoint(IPAddress.Parse("127.0.0.1"),port),new LimitNum()) {
        }

        /// <summary>
        /// 根据参数实例化服务线程 默认监听本地127.0.0.1
        /// </summary>
        /// <param name="type">监听协议类型</param>
        /// <param name="port">端口</param>
        /// <param name="size">限制连接数目</param>
        public ServerThread(ServerEventHandle del,ProtocolType type,int port,int size)
            : this(del,new Socket(AddressFamily.InterNetwork,SocketType.Stream,type),new IPEndPoint(IPAddress.Parse("127.0.0.1"),port),size) {
        }

        /// <summary>
        /// 根据参数实例化服务线程
        /// </summary>
        /// <param name="socket">要监听的Socket</param>
        /// <param name="point">套接字关联</param>
        public ServerThread(ServerEventHandle del,Socket socket,EndPoint point)
            : this(del,socket,point,new LimitNum()) {
        }

        /// <summary>
        /// 根据参数实例化服务线程
        /// </summary>
        /// <param name="socket">要监听的Socket</param>
        /// <param name="point">套接字关联</param>
        /// <param name="size">限制连接数目</param>
        public ServerThread(ServerEventHandle del,Socket socket,EndPoint point,int size)
            : this(del,socket,point,new LimitNum(size)) {
        }

        /// <summary>
        /// 根据参数实例化服务线程
        /// </summary>
        /// <param name="socket">要监听的Socket</param>
        /// <param name="point">套接字关联</param>
        /// <param name="num">限制连接数目</param>
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
        /// 获取Socket连接时出错，并可以使用e.SetCancle(true)方法判断是否可以忽略 默认是true 但是如果不手动设置False 那么也不会抛出错误中断线程。
        /// </summary>
        public event EventHandle ReadExceptionThrowenEvent;

        /// <summary>
        /// 获取Socket连接时出错，并可以使用e.SetCancle(true)方法判断是否可以忽略 默认是False 但是如果不手动设置False 那么也不会抛出错误中断线程。
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
