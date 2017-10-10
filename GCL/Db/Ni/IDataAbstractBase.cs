using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    /// <summary>
    /// 数据基本对象接口
    /// </summary>
    public interface IDataAbstractBase {
        /// <summary>
        /// 新建一个数据库命令对象
        /// </summary>
        /// <returns></returns>
        IDbCommand CreateCommand();
        /// <summary>
        /// 新建一个数据库适配器对象
        /// </summary>
        /// <returns></returns>
        IDbDataAdapter CreateAdapter();
        /// <summary>
        /// 新建一个数据库参数
        /// </summary>
        /// <returns></returns>
        IDbDataParameter CreateParameter();
        /// <summary>
        /// 用于数据库类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DbType ParseType(string type);

        /// <summary>
        /// 参数标记符号
        /// </summary>
        string ParamSign { get; }
    }
}
