using GCL.Event;
namespace GCL.Collections.Pool {

    /// <summary>
    /// һ�㴦��ؿ�ȱ���ҿ������ʱ�Զ��½��͹رչ���
    /// </summary>
    public interface IPoolValueFactory {
        /// <summary>
        /// �������洢���͵��½�����
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        object CreateObject(object e);

        /// <summary>
        /// �������洢���͵Ĺرղ���
        /// </summary>
        /// <param name="obj"></param>
        void CloseObject(object obj);
    }
}
