using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GCL.Db.Ni.NoSQL;

namespace GCL.Db.Ni.ObjectDB {
    public class ObjectDbCommand : NoSQL.NoSQLCommand {

        public ObjectDbCommand()
            : base(LinqSqlParser.Instance()) {
            this.Parameters = new ObjectDbParameters();
            this.State = EnumCommandState.UnReady;
        }


        public override System.Data.IDbDataParameter CreateParameter() {
            return new ObjectDbParameter();
        }

        protected DataSet Result { get; set; }

        protected override DataSet Invoke() {
            this.Prepare();
            if (this.Transaction != null) {
                //在会话情况下无法返回数据，需要保留此时的状态
                ((ObjectDbTransaction)this.Transaction).AddCommand(this.Clone() as ObjectDbCommand);
                return new DataSet();
            }
            ObjectDbConnection conn = Connection as ObjectDbConnection;
            ObjectDbParameter[] paras = new ObjectDbParameter[this.Parameters.Count];
            this.Parameters.CopyTo(paras, 0);

            try {
                this.State = EnumCommandState.Excuting;
                this.Result = conn.Invoke(this.CommandText, (from ps in paras select ps.Value).ToArray());
                this.State = EnumCommandState.Success;
                return this.Result;
            } catch {
                this.State = EnumCommandState.Error;
                throw;
            }
        }

        public override object Clone() {
            return new ObjectDbCommand() {
                Connection = this.Connection,
                CommandText = this.CommandText,
                CommandTimeout = this.CommandTimeout,
                CommandType = this.CommandType,
                Transaction = null,
                QueryType = this.QueryType,
                Parameters = ((ObjectDbParameters)this.Parameters).Clone() as IDataParameterCollection
            };
        }

        protected override IDataReader CreateReader(DataSet dt, CommandBehavior behavior) {
            return new ObjectDbReader(dt, behavior);
        }

        protected override IDataReader CreateReader(DataSet dt) {
            return new ObjectDbReader(dt);
        }

        public override string ParamSign {
            get { return ""; }
        }

        protected override void Invoke(QueryEntity[] entitis, NoSQLConnection conn, DataSet ds) {
            throw new NotImplementedException();
        }
    }
}