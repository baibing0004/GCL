using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using GCL.Event;

namespace GCL.Net {
    public delegate void ServerEventHandle(object sender,ServerEventArg e);
    public class ServerEventArg:EventArg {
        //因为是非事件操作 所以不提供这个功能
        /*
        /// <summary>
        /// 默认的事件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _ServerEventHandleDefault(object sender,ServerEventArg e) {
        }

        /// <summary>
        /// 如果作为事件 是有意义的 不过不作为事件则没有意义
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallServerEventSafely(ServerEventHandle handle,object sender,ServerEventArg e)
        {
            try {
                handle(sender,e);
            } catch{
            }
        }
        */
        /// <summary>
        /// 通过使用Socket初始化ServerEventArg参数类	 
        /// </summary>
        /// <param name="socket"></param>
        public ServerEventArg(Socket socket)
            : base(socket) {
        }

        /// <summary>
        ///获得socket对象         
        /// </summary>
        /// <returns></returns>
        public Socket getSocket() {
            return (Socket)base.GetPara(0);
        }
    }
}
