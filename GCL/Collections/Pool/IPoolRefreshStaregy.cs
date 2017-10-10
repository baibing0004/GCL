
namespace GCL.Collections.Pool {
    /// <summary>
    /// 
    /// </summary>
    public interface IPoolRefreshStaregy : IPoolStaregy {
        /// <summary>
        /// 是否可以删除
        /// 注意它虽然继承IPoolStaregy但实际上和上一个接口处理的事情不同，而且更复杂。知识方法恰巧相同
        /// </summary>
        /// <returns></returns>
        object AllowDel();
    }
}
