using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 尽量继承以实现退出时关闭资源
    /// </summary>
    public interface IClosable : IDisposable {
        void Close();
    }
}
