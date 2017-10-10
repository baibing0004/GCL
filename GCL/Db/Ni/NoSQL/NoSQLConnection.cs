using GCL.Bean.Middler;
using GCL.IO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Db.Ni.NoSQL {
    /// <summary>
    /// 管理对象的实例和调用方法    
    /// </summary>
    public abstract class NoSQLConnection : System.Data.IDbConnection {

        public NoSQLConnection() { this.ConnectionString = string.Empty; this.State = System.Data.ConnectionState.Closed; }

        public abstract System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il);

        public abstract System.Data.IDbTransaction BeginTransaction();

        public abstract void ChangeDatabase(string databaseName);

        public abstract void Close();

        public virtual string ConnectionString {
            get;
            set;
        }

        public virtual int ConnectionTimeout {
            get { return 0; }
        }



        public abstract void Open();

        public virtual System.Data.ConnectionState State {
            get;
            protected set;
        }

        public virtual void Dispose() {
            this.Close();
        }


        public abstract System.Data.IDbCommand CreateCommand();

        public string Database {
            get;
            protected set;
        }
    }
}