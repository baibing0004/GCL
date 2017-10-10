using GCL.Bean.Middler;
using GCL.IO.Config;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GCL.Db.Ni.RedisDB {
    /// <summary>
    /// 管理对象的实例和调用方法    
    /// </summary>
    public class RedisDbConnection : GCL.Db.Ni.NoSQL.NoSQLConnection {
        private IRedisClient rc = null;

        public IRedisClient Client {
            get { return rc; }
        }
        /// <summary>
        /// 带服务端连接字符串的构造函数
        /// </summary>
        /// <param name="connParams"></param>
        public RedisDbConnection() {
            this.State = System.Data.ConnectionState.Closed;
        }
        /// <summary>
        /// 带指定事务锁的开启事务
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) {
            return new RedisDbTransaction() { IsolationLevel = il, Connection = this };
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbTransaction BeginTransaction() {
            return new RedisDbTransaction() { Connection = this };
        }
        /// <summary>
        /// 未连接时更改数据库
        /// </summary>
        /// <param name="databaseName"></param>
        public override void ChangeDatabase(string databaseName) {
            if (this.State == System.Data.ConnectionState.Closed) {
                this.Database = databaseName;
            } else throw new InvalidOperationException("已经连接无法直接更改对象名。");
        }
        private object strCK = System.DateTime.Now;
        /// <summary>
        /// 关闭连接
        /// </summary>
        public override void Close() {
            if (this.rc != null) {
                lock (this.strCK) {
                    if (this.rc != null) {
                        this.rc.Dispose();
                        this.rc = null;
                        this.State = System.Data.ConnectionState.Closed;
                    }
                }
            }
        }
        //连接字符串
        protected string connectionString = null;
        /// <summary>
        /// 连接字符串属性
        /// </summary>
        public override string ConnectionString {
            get {
                return connectionString;
            }
            set {
                if (this.State == System.Data.ConnectionState.Closed) {
                    this.connectionString = value;
                    if (string.IsNullOrEmpty(this.connectionString)) return;
                    string[] conns = this.connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!conns[0].Contains("Redis")) {
                        throw new InvalidOperationException("ConnectionString属性设置错误,如Redis=127.0.0.1:6379,10.0.0.132:6379;Database=Test;DateTime=11");
                    }
                    conns.Where(p => {
                        var paras = p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (paras.Length < 2) return false;
                        switch (paras[0].Trim().ToLower()) {
                            case "redis":
                                this.Redis = paras[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                break;
                            case "datetime":
                                this.DateTime = Convert.ToInt32(paras[1]);
                                break;
                            case "database":
                                this.Database = Convert.ToString(paras[1]);
                                break;
                        }
                        return false;
                    }).Count();
                } else { throw new InvalidOperationException("Connection使用中，不可设置!"); }
            }
        }
        /// <summary>
        /// 命令行
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbCommand CreateCommand() {
            return new RedisDbCommand() { Connection = this };
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public override void Open() {
            switch (this.State) {
                case System.Data.ConnectionState.Closed:
                    if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("ConnectionString未设置!");
                    lock (this) {
                        try {
                            this.State = System.Data.ConnectionState.Connecting;
                            IRedisClientsManager clientManager = new PooledRedisClientManager(this.Redis);
                            this.rc = clientManager.GetClient();
                            if (this.rc != null) { this.State = System.Data.ConnectionState.Open; } else {
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
        /// <summary>
        /// 销毁连接
        /// </summary>
        public override void Dispose() {
            this.Close();
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string[] Redis { get; protected set; }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public int DateTime { get; protected set; }
    }
}