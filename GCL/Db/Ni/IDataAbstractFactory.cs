using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    /// <summary>
    /// 抽象工厂接口
    /// </summary>
    public interface IDataAbstractFactory : IDataAbstractBase {
        /// <summary>
        /// 新建一个数据库联接对象
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
    }
}
