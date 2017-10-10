using GCL.Bean.Middler;
using GCL.IO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Db.Ni.ObjectDB {
    /// <summary>
    /// 管理对象的实例和调用方法    
    /// </summary>
    public class ObjectDbConnection : NoSQL.NoSQLConnection {
        /// <summary>
        /// 需要一个Middler对象
        /// </summary>
        /// <param name="middler"></param>
        public ObjectDbConnection(Middler middler) {
            this.middler = middler;
            this.State = System.Data.ConnectionState.Closed;
        }

        /// <summary>
        /// 需要一个ConfigManager对象
        /// </summary>
        /// <param name="cm"></param>
        public ObjectDbConnection(ConfigManager cm) : this(new Middler(cm)) { }

        /// <summary>
        /// 内部使用的Middler
        /// </summary>
        internal Middler middler { get; set; }



        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) {
            return new ObjectDbTransaction() { IsolationLevel = il, Connection = this };
        }

        public override System.Data.IDbTransaction BeginTransaction() {
            return new ObjectDbTransaction() { Connection = this };
        }

        public override void ChangeDatabase(string databaseName) {
            if (string.IsNullOrEmpty(this.App) && this.State == System.Data.ConnectionState.Closed) {
                this.Database = databaseName;
            } else throw new InvalidOperationException("已经连接无法直接更改对象名。");
        }

        public override void Close() {
            if (this.obj != null) {
                lock (this.obj) {
                    this.middler.SetObjectByAppName(this.App, this.Database, this.obj);
                    this.obj = null;
                    this.State = System.Data.ConnectionState.Closed;
                }
            }
        }

        private string connectionString = null;
        public override string ConnectionString {
            get {
                return connectionString;
            }
            set {
                if (this.State == System.Data.ConnectionState.Closed) {
                    this.connectionString = value;
                    if (string.IsNullOrEmpty(this.connectionString)) return;
                    var conns = this.connectionString.Split('/','\\');
                    if (conns.Length != 2) {
                        throw new InvalidOperationException("ConnectionString属性设置错误，请参照App/ObjectName方式进行设置，譬如VESHTest/VESHModule.Class1");
                    }
                    this.App = conns[0];
                    this.Database = conns[1];
                } else { throw new InvalidOperationException("Connection使用中，不可设置!"); }
            }
        }

        public override System.Data.IDbCommand CreateCommand() {
            return new ObjectDbCommand() { Connection = this };
        }
        /// <summary>
        /// 主要的服务对象 应该只开放Invoke方法
        /// </summary>
        private Object obj;

        public override void Open() {
            switch (this.State) {
                case System.Data.ConnectionState.Closed:
                    if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("ConnectionString未设置!");
                    lock (this) {
                        try {

                            this.State = System.Data.ConnectionState.Connecting;
                            this.obj = middler.GetObjectByAppName(this.App, this.Database);
                            if (obj != null) { this.State = System.Data.ConnectionState.Open; } else {
                                throw new InvalidOperationException(string.Format("没找到{0}对象", this.connectionString));
                            }
                        } catch {
                            this.State = System.Data.ConnectionState.Closed;
                            throw;
                        }
                    }
                    break;
                case System.Data.ConnectionState.Open:
                case System.Data.ConnectionState.Connecting:
                case System.Data.ConnectionState.Executing:
                    break;
                case System.Data.ConnectionState.Broken:
                case System.Data.ConnectionState.Fetching:
                    //预留处理
                    break;
            }
        }


        public void Dispose() {
            this.Close();
        }

        internal System.Data.DataSet Invoke(string name, object[] paras) {
            if (this.State != System.Data.ConnectionState.Open) throw new InvalidOperationException("请先Open数据库连接!");
            try {
                this.State = System.Data.ConnectionState.Executing;
                return GCL.Bean.BeanTool.Invoke(this.obj, name, paras) as System.Data.DataSet;
            } finally {
                this.State = System.Data.ConnectionState.Open;
            }
        }

        public string App { get; set; }

        public string Database { get; set; }
    }
}