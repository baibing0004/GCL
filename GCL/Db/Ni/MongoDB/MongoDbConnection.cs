using MongoDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace GCL.Db.Ni.MongoDB {
    public class MongoDbConnection : NoSQL.NoSQLConnection {


        protected Mongo _mongo;
        /// <summary>
        /// 当前连接的数据库
        /// </summary>
        protected IMongoDatabase _db;
        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) {
            return new MongoDbTransaction() { IsolationLevel = il, Connection = this };
        }

        public override System.Data.IDbTransaction BeginTransaction() {
            return new MongoDbTransaction() { Connection = this };
        }

        public override void ChangeDatabase(string databaseName) {
            if (this._mongo == null && this.State == System.Data.ConnectionState.Closed) {
                this.Database = databaseName;
            } else throw new InvalidOperationException("已经连接无法直接更改对象名。");
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        protected string connectionString = null;
        public override string ConnectionString {
            get {
                return connectionString;
            }
            set {
                if (this.State == System.Data.ConnectionState.Closed) {
                    this.connectionString = value;
                    if (string.IsNullOrEmpty(this.connectionString)) return;
                    var conns = this.connectionString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (conns.Length >= 3) {
                        this.Database = conns[2];
                    } else {
                        throw new InvalidOperationException("ConnectionString属性设置错误，请参照mongodb://fred:foobar@localhost/baz;使用用户fred和密码foobar连接，指定数据库baz");
                    }
                } else { throw new InvalidOperationException("Connection使用中，不可设置!"); }
            }
        }
        /// <summary>
        /// 保证MongoDB上下文实例是线程内唯一。
        /// </summary>
        /// <returns></returns>
        public static Mongo CreateMongoDB(string connectionString) {
            Mongo _mongo = (Mongo)CallContext.GetData("_mongo");
            if (_mongo == null) {
                _mongo = new Mongo(connectionString);
                CallContext.SetData("_mongo", _mongo);
                // 立即连接 MongoDB
                _mongo.Connect();
            }
            return _mongo;
        }

        public static void CloseMongoDB(Mongo mongo) {
            Mongo _mongo = (Mongo)CallContext.GetData("_mongo");
            if (_mongo != null && _mongo == mongo) {
                mongo.Disconnect();
                CallContext.SetData("_mongo", null);
            }
        }


        private object strCK = DateTime.Now;

        public override void Close() {
            if (this._mongo != null) {
                lock (this.strCK) {
                    if (this._mongo != null) {
                        MongoDbConnection.CloseMongoDB(this._mongo);
                        this._mongo = null;
                        this.State = System.Data.ConnectionState.Closed;
                    }
                }
            }
        }

        public override void Open() {
            switch (this.State) {
                case System.Data.ConnectionState.Closed:
                    if (string.IsNullOrEmpty(connectionString))
                        throw new InvalidOperationException("ConnectionString未设置!");
                    if (string.IsNullOrEmpty(this.Database))
                        throw new InvalidOperationException("Database未设置!");
                    lock (this) {
                        try {
                            this.State = System.Data.ConnectionState.Connecting;
                            //理论上这样处理没有意义 因为Connection对象在框架中是不会复用的，其复用只能在Connection内部实现
                            _mongo = CreateMongoDB(this.connectionString);
                            if (!string.IsNullOrEmpty(this.Database))
                                _db = _mongo.GetDatabase(this.Database);
                            if (_mongo != null) { this.State = System.Data.ConnectionState.Open; } else {
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

        public override System.Data.IDbCommand CreateCommand() {
            return new MongoDbCommand() { Connection = this };
        }

        public void Dispose() {
            if (_mongo != null) {
                _mongo.Dispose();
                _mongo = null;
            }
        }

        /// <summary>
        /// 获取当前连接的数据库
        /// </summary>
        public IMongoDatabase CurrentDb {
            get {
                if (_db == null)
                    throw new Exception("当前连接没有指定任何数据库。请在构造函数中指定数据库名或者调用UseDb()方法切换数据库。");
                return _db;
            }
        }
        /// <summary>
        /// 切换到指定的数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public IMongoDatabase UseDb(string dbName) {
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentNullException("dbName");
            if (this.State != System.Data.ConnectionState.Open) throw new InvalidOperationException("请先Open数据库连接!");
            _db = _mongo.GetDatabase(dbName);
            return _db;
        }
        /// <summary>
        /// 获取当前连接数据库的指定集合【依据类型】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>() where T : class {
            return this.CurrentDb.GetCollection<T>();
        }
    }
}
