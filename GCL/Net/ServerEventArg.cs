using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using GCL.Event;

namespace GCL.Net {
    public delegate void ServerEventHandle(object sender,ServerEventArg e);
    public class ServerEventArg:EventArg {
        //��Ϊ�Ƿ��¼����� ���Բ��ṩ�������
        /*
        /// <summary>
        /// Ĭ�ϵ��¼�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _ServerEventHandleDefault(object sender,ServerEventArg e) {
        }

        /// <summary>
        /// �����Ϊ�¼� ��������� ��������Ϊ�¼���û������
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
        /// ͨ��ʹ��Socket��ʼ��ServerEventArg������	 
        /// </summary>
        /// <param name="socket"></param>
        public ServerEventArg(Socket socket)
            : base(socket) {
        }

        /// <summary>
        ///���socket����         
        /// </summary>
        /// <returns></returns>
        public Socket getSocket() {
            return (Socket)base.GetPara(0);
        }
    }
}
