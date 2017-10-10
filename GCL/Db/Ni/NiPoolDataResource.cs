using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using GCL.Collections.Pool;
namespace GCL.Db.Ni {
    /// <summary>
    /// 池队列管理数据库连接，其对数据库开放连接进行自动管理，保证其不超过最大值，而且持续waittime时间的连接将自动关闭
    /// </summary>
    public class NiPoolDataResource : ADataResource, IPoolValueFactory {

        private RefreshPool pool = null;
        private TimeSpan timeSpan;
        /// <summary>
        /// 设定数据库池最大值，默认1分钟，空闲连接将被关闭！默认等待对象超时时间为30秒
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstring"></param>
        /// <param name="size"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, string connstring, int size)
            : this(fac, connstring, size, 30000, 60000) {
        }

        /// <summary>
        /// 设定数据库池最大值，默认1分钟，空闲连接将被关闭！
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstring"></param>
        /// <param name="size"></param>
        /// <param name="timeout"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, string connstring, int size, int timeout)
            : this(fac, connstring, size, timeout, 60000) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstring"></param>
        /// <param name="size"></param>
        /// <param name="timeout"></param>
        /// <param name="waitTime"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, string connstring, int size, int timeout, int waitTime)
            : base(fac, connstring) {
            this.pool = new RefreshPool(size, "", this, waitTime);
            this.timeSpan = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// 设定数据库池最大值，默认1分钟，空闲连接将被关闭！默认等待对象超时时间为30秒
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstrings"></param>
        /// <param name="size"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, ArrayList connstrings, int size)
            : this(fac, connstrings, size, 30000, 60000) {
        }

        /// <summary>
        /// 设定数据库池最大值，默认1分钟，空闲连接将被关闭！
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstrings"></param>
        /// <param name="size"></param>
        /// <param name="timeout"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, ArrayList connstrings, int size, int timeout)
            : this(fac, connstrings, size, timeout, 60000) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="connstrings"></param>
        /// <param name="size"></param>
        /// <param name="timeout"></param>
        /// <param name="waitTime"></param>
        public NiPoolDataResource(IDataAbstractFactory fac, ArrayList connstrings, int size, int timeout, int waitTime)
            : base(fac, connstrings) {
            this.pool = new RefreshPool(size, "", this, waitTime);
            this.timeSpan = TimeSpan.FromMilliseconds(timeout);
        }

        /// <summary>
        /// 获取数据库开放连接
        /// </summary>
        /// <returns></returns>
        public override IDbConnection GetConnection() {
            IDbConnection conn;
            do {
                conn = this.pool.Get(this.timeSpan) as DbConnection;
                try {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    return conn;
                } catch {
                    SetConnection(conn);
                }
            } while (true);
            //throw new Exception("数据库连接在不开放的情况下被释放");
        }


        /// <summary>
        /// 返回或者关闭数据库连接
        /// </summary>
        /// <param name="conn"></param>
        public override void SetConnection(IDbConnection conn) {
            if (conn.State != ConnectionState.Open) {
                try {
                    conn.Close();
                } catch {
                }
                this.pool.Remove(conn);
            } else
                this.pool.Set(conn);
        }

        public override void Dispose() {
            pool.Close();
        }

        #region IPoolValueFactory Members

        public object CreateObject(object e) {
            return this.CreateConnection();
        }

        public void CloseObject(object obj) {
            if (DBTool.IsEnable(obj) && obj is DbConnection) {
                try {
                    ((DbConnection)obj).Close();
                } catch {
                }
            } else GCL.Bean.BeanTool.Close(obj);
        }

        #endregion
    }
}
