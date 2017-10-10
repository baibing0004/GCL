using GCL.Event;
namespace GCL.Collections.Pool {

    /// <summary>
    /// 一般处理池空缺而且可以填充时自动新建和关闭功能
    /// </summary>
    public interface IPoolValueFactory {
        /// <summary>
        /// 处理所存储类型的新建操作
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        object CreateObject(object e);

        /// <summary>
        /// 处理所存储类型的关闭操作
        /// </summary>
        /// <param name="obj"></param>
        void CloseObject(object obj);
    }
}
