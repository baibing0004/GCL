
namespace GCL.Collections.Pool {
    /// <summary>
    /// 
    /// </summary>
    public interface IPoolRefreshStaregy : IPoolStaregy {
        /// <summary>
        /// �Ƿ����ɾ��
        /// ע������Ȼ�̳�IPoolStaregy��ʵ���Ϻ���һ���ӿڴ�������鲻ͬ�����Ҹ����ӡ�֪ʶ����ǡ����ͬ
        /// </summary>
        /// <returns></returns>
        object AllowDel();
    }
}
