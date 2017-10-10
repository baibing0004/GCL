using GCL.Bean.Middler;
using GCL.IO.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Controler;

namespace GCL.Project.VESH.Db.Ni.WebSocketDB {
    /// <summary>
    /// 管理对象的实例和调用方法    
    /// </summary>
    public class WebSocketDbConnection : GCL.Db.Ni.NoSQL.NoSQLConnection {
        /// <summary>
        /// WebSocket数据库连接 默认调用VESH.WSControler的静态变量实现DB连接
        /// </summary>
        /// <param name="middler"></param>
        public WebSocketDbConnection() {
            this.State = System.Data.ConnectionState.Closed;
        }

        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) {
            return new WebSocketDbTransaction() { Connection = this };//IsolationLevel = il,
        }

        public override System.Data.IDbTransaction BeginTransaction() {
            return new WebSocketDbTransaction() { Connection = this };
        }

        public override void ChangeDatabase(string databaseName) {
            if (this.State == System.Data.ConnectionState.Closed) {
                this.Database = databaseName;
            } else throw new InvalidOperationException("已经连接无法直接更改对象名。");
        }

        public override void Close() {
        }

        private Int32 conn = 0;
        public override string ConnectionString {
            get {
                return "" + conn;
            }
            set {
                if (this.State == System.Data.ConnectionState.Closed) {
                    if (string.IsNullOrEmpty(value)) return;
                    var conns = Convert.ToInt32(value);
                    if (conns <= 0) {
                        throw new InvalidOperationException("ConnectionString属性设置错误，请设置四位端口号，譬如8181");
                    } else
                        this.conn = conns;

                } else { throw new InvalidOperationException("Connection使用中，不可设置!"); }
            }
        }

        public override System.Data.IDbCommand CreateCommand() {
            return new WebSocketDbCommand() { Connection = this };
        }

        public override void Open() {
            switch (this.State) {
                case System.Data.ConnectionState.Closed:
                    lock (this) {
                        this.State = System.Data.ConnectionState.Connecting;
                        if (WSControler.idic != null && WSControler.idic.ContainsKey(this.conn)) {
                            this.State = System.Data.ConnectionState.Open;
                        } else {
                            this.State = System.Data.ConnectionState.Closed;
                            throw new InvalidOperationException(string.Format("没有启动WS初始化对象，等待客户端启动{0}端口访问", this.conn));
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

        internal bool Invoke(string name, IDictionary<string, object> paras) {
            if (paras != null && paras.Count < 2 && (!paras.ContainsKey("_id") || !paras.ContainsKey("content"))) throw new InvalidOperationException("参数应为 id(null),content");
            if (this.State != System.Data.ConnectionState.Open) throw new InvalidOperationException("请先Open数据库连接!");
            try {
                this.State = System.Data.ConnectionState.Executing;
                if (WSControler.idic.ContainsKey(this.conn) && WSControler.idic[this.conn].MethodSession != null && WSControler.idic[this.conn].MethodSession.ContainsKey(name)) {
                    var session = WSControler.idic[this.conn].MethodSession[name];
                    if (!string.IsNullOrEmpty("" + paras["_id"]) && session.ContainsKey(Convert.ToString(paras["_id"]))) {
                        session[Convert.ToString(paras["_id"])].Conn.Send(GCL.Common.Tool.SerializeToString(new { _id = Convert.ToString(paras["_id"]), response = Convert.ToString(paras["content"]) }));
                    } else {
                        session.Where(p => {
                            p.Value.Conn.Send(GCL.Common.Tool.SerializeToString(new { _id = p.Key, response = Convert.ToString(paras["content"]) }));
                            return false;
                        }).Count();
                    }
                    return true;
                }
                return false;
            } finally {
                this.State = System.Data.ConnectionState.Open;
            }
        }
    }
}