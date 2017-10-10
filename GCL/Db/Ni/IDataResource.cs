using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni
{
    /// <summary>
    /// 数据源接口
    /// </summary>
    public interface IDataResource : IDataAbstractBase, IDisposable
    {
        /// <summary>
        /// 用于获得开放的一个数据库联接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
        /// <summary>
        /// 使用完成返回这个数据库联接，由数据源对象管理连接的关闭。
        /// </summary>
        void SetConnection(IDbConnection conn);


        string GetFacotryTypeName();
    }
}
