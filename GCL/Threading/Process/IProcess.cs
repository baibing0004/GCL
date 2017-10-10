using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;

namespace GCL.Threading.Process {
    
    /// <summary>
    /// ��������״̬
    /// </summary>
    public enum ProcessState {
        NOSTATE=-1,
        INIT=0,
        READY=1,
        START = 2,
        STOP = 3,
        DISPOSE=4,
        EXCEPTION = 5
    }



    /// <summary>
    /// ��Ϊһ�ּܹ���� �������4�������Ҫ�� �����Ͽ�����ɶ�ҵ���߼��ĳ�ʼ������ʼ���жϣ�����Ȳ�����
    /// </summary>
    public interface IProcess:IDisposable {

        /// <summary>
        /// ���̵��¼����������ͨ��EventLevel���� (һ���Ӧ����Ĭ�϶��ĵķ���������ʹ�ð�ȫ�����¼�����) 
        /// </summary>
        event ProcessEventHandle ProcessEvent;

        /// <summary>
        /// �����ʼ������
        /// </summary>
        void Init();

        /// <summary>
        /// ����ʼ/��������
        /// </summary>
        void Start();

        /// <summary>
        /// �������/�жϽ���
        /// </summary>
        void Stop();
    }
}
