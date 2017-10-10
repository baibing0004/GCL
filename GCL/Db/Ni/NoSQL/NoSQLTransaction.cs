using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace GCL.Db.Ni.NoSQL {
    public abstract class NoSQLTransaction : System.Data.IDbTransaction {

        protected LinkedList<NoSQLCommand> commands = new LinkedList<NoSQLCommand>();

        public virtual void Commit() {
            if (this.DataResult == null) throw new InvalidOperationException("未设置DataResult属性，无法处理此项功能!");
            lock (commands) {
                NoSQLCommand[] coms = commands.ToArray();
                //仅限Ni框架使用
                foreach (var com in coms) {
                    com.Transaction = null;
                    switch (com.QueryType) {
                        case EnumQueryType.ExecuteNonQuery:
                            NiNonQueryDataCommand.FillDataTable(this.DataResult.DataSet, com, "NonQuery", com.ExecuteNonQuery());
                            break;
                        case EnumQueryType.ExecuteQuery:
                            this.DataResult.DataSet.Merge(com.ExecuteQuery());
                            break;
                        case EnumQueryType.ExecuteReader:
                            this.DataResult.DoReader(com.ExecuteReader());
                            break;
                        case EnumQueryType.ExecuteScalar:
                            NiQueryDataCommand.FillDataTable(this.DataResult.DataSet, com, "Query", com.ExecuteScalar());
                            break;
                        case EnumQueryType.Undefined:
                            //说明该命令啥也没做
                            break;
                    }
                    this.commands.Clear();
                }
            }
        }

        public virtual void AddCommand(NoSQLCommand cmd) {
            commands.AddLast(cmd);
        }

        protected NoSQLConnection conn;

        public virtual System.Data.IDbConnection Connection {
            get {
                return conn;
            }
            set {
                if (value == null || value is NoSQLConnection) this.conn = value as NoSQLConnection;
                else
                    throw new InvalidOperationException("请使用NoSQLConnection的实例!");
            }
        }

        public virtual System.Data.IsolationLevel IsolationLevel {
            get;
            internal set;
        }

        public virtual void Rollback() {
            throw new NotImplementedException();
        }

        public virtual void Dispose() {
            if (this.commands != null) {
                lock (this) {
                    this.commands.Clear();
                    this.commands = null;
                }
            }
        }

        public virtual NiDataResult DataResult { get; set; }
    }
}