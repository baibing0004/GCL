using GCL.Bean.Middler;
using GCL.IO.Config;
using Memcached.ClientLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GCL.Db.Ni.MemcacheDB {
    /// <summary>
    /// 管理对象的实例和调用方法    
    /// </summary>
    public class MemDbConnection : GCL.Db.Ni.NoSQL.NoSQLConnection {
        protected MemcachedClient mc = null;
        public MemcachedClient Client { get { return mc; } }
        /// <summary>
        /// 带服务端连接字符串的构造函数
        /// </summary>
        /// <param name="connParams"></param>
        public MemDbConnection() {
            this.State = System.Data.ConnectionState.Closed;
            InitConnections = 3;
            MinConnections = 3;
            MaxConnections = 5;

            SocketConnectTimeout = 1000;
            SocketTimeout = 3000;

            MaintenanceSleep = 30;
            Failover = true;

            Nagle = false;
            EnableCompression = false;
        }
        /// <summary>
        /// 带指定事务锁的开启事务
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) {
            return new MemDbTransaction() { IsolationLevel = il, Connection = this };
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbTransaction BeginTransaction() {
            return new MemDbTransaction() { Connection = this };
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
            if (this.mc != null) {
                lock (this.strCK) {
                    this.mc = null;
                    this.State = System.Data.ConnectionState.Closed;
                }
            }
        }
        protected string connectionString = null;
        public override string ConnectionString {
            get {
                return connectionString;
            }
            set {
                if (this.State == System.Data.ConnectionState.Closed) {
                    this.connectionString = value;
                    if (string.IsNullOrEmpty(this.connectionString)) return;
                    string[] conns = this.connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!conns[0].Contains("ServerIp")) {
                        throw new InvalidOperationException("ConnectionString属性设置错误,如ServerIp=127.0.0.1:11211,10.0.0.132:11211;DateTime=1000");
                    }
                    conns.Where(p => {
                        var paras = p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (paras.Length < 2) return false;
                        switch (paras[0].Trim().ToLower()) {
                            case "serverip":
                                this.ServerIp = paras[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                break;
                            case "datetime":
                                this.DateTime = Convert.ToInt32(paras[1]);
                                break;
                            case "initconnections":
                                this.InitConnections = Convert.ToInt32(paras[1]);
                                break;
                            case "minconnections":
                                this.MinConnections = Convert.ToInt32(paras[1]);
                                break;
                            case "maxconnections":
                                this.MaxConnections = Convert.ToInt32(paras[1]);
                                break;
                            case "socketconnecttimeout":
                                this.SocketConnectTimeout = Convert.ToInt32(paras[1]);
                                break;
                            case "sockettimeout":
                                this.SocketTimeout = Convert.ToInt32(paras[1]);
                                break;
                            case "maintenancesleep":
                                this.MaintenanceSleep = Convert.ToInt32(paras[1]);
                                break;
                            case "failover":
                                this.Failover = Convert.ToBoolean(paras[1]);
                                break;
                            case "nagle":
                                this.Nagle = Convert.ToBoolean(paras[1]);
                                break;
                            case "enablecompression":
                                this.EnableCompression = Convert.ToBoolean(paras[1]);
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
            return new MemDbCommand() { Connection = this };
        }

        /// <summary>
        /// 主要的服务对象 应该只开放Invoke方法
        /// </summary>
        //protected Object obj;
        public static SockIOPool POOL;
        public static readonly object POOLKey = System.DateTime.Now;
        public static void InitPool(MemDbConnection conn) {
            if (POOL == null) {
                lock (POOLKey) {
                    if (POOL == null) {
                        //初始化池
                        SockIOPool pool = SockIOPool.GetInstance();
                        pool.SetServers(conn.ServerIp);

                        pool.InitConnections = conn.InitConnections;
                        pool.MinConnections = conn.MinConnections;
                        pool.MaxConnections = conn.MaxConnections;

                        pool.SocketConnectTimeout = conn.SocketConnectTimeout;
                        pool.SocketTimeout = conn.SocketTimeout;

                        pool.MaintenanceSleep = conn.MaintenanceSleep;
                        pool.Failover = conn.Failover;

                        pool.Nagle = conn.Nagle;
                        pool.Initialize();
                        POOL = pool;
                    }
                }
            }
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
                            InitPool(this);
                            // 获得客户端实例
                            mc = new MemcachedClient();
                            mc.EnableCompression = this.EnableCompression;
                            if (this.mc != null) { this.State = System.Data.ConnectionState.Open; } else {
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

        public string[] ServerIp { get; protected set; }

        public int DateTime { get; protected set; }

        public int InitConnections { get; protected set; }

        public int MinConnections { get; protected set; }

        public int MaxConnections { get; protected set; }

        public int SocketConnectTimeout { get; protected set; }

        public int SocketTimeout { get; protected set; }

        public int MaintenanceSleep { get; protected set; }

        public bool Failover { get; protected set; }

        public bool Nagle { get; protected set; }

        public bool EnableCompression { get; protected set; }
    }
}