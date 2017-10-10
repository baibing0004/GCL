using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using GCL.IO.Log;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// Socket服务器端池处理类
    /// 继承APoolServer后在Custom方法中获得是TcpClient对象
    ///<add key="你的类.ServerIP" value="10.0.100.70"/>
    ///<add key="你的类.Port" value="5678"/>
    /// </summary>
    public abstract class ASocketPoolServer : APoolServer {

        private TcpListener listener = null;
        private IPEndPoint serPoint = null;
        private int RSTimeOut = 3000;
        private int RSBuffer = 8192;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIp">监听地址</param>
        /// <param name="port">监听端口</param>
        /// <param name="rsTimeOut">接收超时时间</param>
        /// <param name="rsBuffer">接收缓冲区</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public ASocketPoolServer(string serverIp, int port, int rsTimeOut, int rsBuffer, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : this(serverIp, port, rsTimeOut, rsBuffer, createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, 0) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIp">监听地址</param>
        /// <param name="port">监听端口</param>
        /// <param name="rsTimeOut">接收超时时间</param>
        /// <param name="rsBuffer">接收缓冲区</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public ASocketPoolServer(string serverIp, int port, int rsTimeOut, int rsBuffer, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime) {
            serPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            RSTimeOut = rsTimeOut;
            RSBuffer = 1024 * rsBuffer;
            listener = new TcpListener(serPoint);
            listener.Server.SendTimeout = RSTimeOut;
            listener.Server.ReceiveTimeout = RSTimeOut;
            listener.Server.ReceiveBufferSize = RSBuffer;
            //listener.Server.SendBufferSize = RSBuffer;
        }

        public override void Init() {
            base.Init();
            this.CallProcessEventSimple(LogType.RELEASE, 101, string.Format("获得绑定地址：{0}:{1}", serPoint.Address.ToString(), serPoint.Port));
            this.CallProcessEventSimple(LogType.RELEASE, 101, string.Format("收发超时时间:{0}毫秒", RSTimeOut));
            this.CallProcessEventSimple(LogType.RELEASE, 101, string.Format("接收缓存:{0}KB", RSBuffer / 1024));
            this.CallProcessEventSimple(LogType.RELEASE, 101, "监听器初始化成功!");
        }

        public override void Start() {
            try {
                listener.Start(this.poolProcess.Pool.GetCapacity());
                //listener.Start();
                this.CallProcessEventSimple(LogType.RELEASE, 102, string.Format("监听器绑定{0}:{1}成功！", serPoint.Address, serPoint.Port));
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "监听程序无法正常启动！");
            }
            base.Start();
        }

        public override void Stop() {
            try {
                listener.Stop();
                this.CallProcessEventSimple(LogType.RELEASE, 103, "监听器关闭！" + listener.Server.Connected.ToString());
            } catch (Exception ex) {
                this.CallProcessEventSimple(ex, "监听程序无法正常关闭！");
                try {
                    listener.Server.Close();
                } catch {
                }
            }
            base.Stop();
        }

        protected override bool CheckStop() {
            this.CallProcessEventSimple(LogType.DEBUG, 104, string.Format("判断监听器是否已经关闭?{0},{1},{2}", listener.Server.Blocking, listener.Server.IsBound, listener.Server.Connected));
            return listener.Server.Connected && base.CheckStop();
        }

        protected override object Create(object sender, GCL.Event.EventArg e) {
            try {
                lock (listener) {

                    if (listener.Server.IsBound && listener.Pending())
                        this.CallProcessEventSimple(LogType.DEBUG, 201, "Create成功获取客户端连接");
                    else
                        this.CallProcessEventSimple(LogType.DEBUG, 201, string.Format("Create没有成功获取客户端连接"));

                    if (listener.Server.IsBound && listener.Pending()) {
                        TcpClient client = listener.AcceptTcpClient();
                        return client;
                    }
                }
            } catch (Exception ex) {
                this.CallProcessEventSimple(LogType.RELEASE, 202, "Create获取客户端连接出现错误" + ex.ToString());
            }
            return null;
        }


        protected override void RollBack(object sender, GCL.Event.EventArg e, object value) {
            Custom(sender, e, value);
        }
    }
}
