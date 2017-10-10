using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Collections {
    /// <summary>
    /// 用于限定限制类之间的协同唤醒.
    /// </summary>
    interface ICoordinateLimit {
        //不可以使用Virtual 因为不是实体类 必须允许实现 但是否允许重载视实现类定义
        void LimitNotifyAll();
    }
}
