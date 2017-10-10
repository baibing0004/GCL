using System.Collections;

namespace GCL.Collections.Pool {

    /// <summary>
    /// 设定池的存取方式主要是决定池是否可以冗余，存取顺序
    /// </summary>
    public interface IPoolStaregy {
         object Set(object value);

         object Get();

         void Remove(object value);

         void Clear();

         bool Contains(object value);

         IPoolStaregy CreateNewInstance();
    }
}
