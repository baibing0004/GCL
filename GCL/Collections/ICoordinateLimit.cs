using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Collections {
    /// <summary>
    /// �����޶�������֮���Эͬ����.
    /// </summary>
    interface ICoordinateLimit {
        //������ʹ��Virtual ��Ϊ����ʵ���� ��������ʵ�� ���Ƿ�����������ʵ���ඨ��
        void LimitNotifyAll();
    }
}
