using System.Collections;

namespace GCL.Collections.Pool {

    /// <summary>
    /// �趨�صĴ�ȡ��ʽ��Ҫ�Ǿ������Ƿ�������࣬��ȡ˳��
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
