using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    /// <summary>
    /// 对于数据库对象只负责生成与关闭，不负责管理
    /// </summary>
    public class NiInstanceDataResource : ADataResource {

        public NiInstanceDataResource(IDataAbstractFactory fac, string connstring) : base(fac, connstring) { }
        /// <summary>
        /// 自动根据数据库连接字符串生成连通的数据库连接
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection() {
            IDbConnection conn;
            do {
                conn = this.CreateConnection();
                try {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    return conn;
                } catch {
                    SetConnection(conn);
                }
            } while (true);
        }

        /// <summary>
        /// 返回连接即可关闭
        /// </summary>
        /// <param name="conn"></param>
        public override void SetConnection(IDbConnection conn) {
            try {
                conn.Close();
            } catch {
            }
        }

        public override void Dispose() {
        }
    }
}
