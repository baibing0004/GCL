using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GCL.Db.Ni.NoSQL;
using System.Text.RegularExpressions;

namespace GCL.Project.VESH.Db.Ni.WebSocketDB {
    public class WebSocketDbCommand : GCL.Db.Ni.NoSQL.NoSQLCommand {
        public WebSocketDbCommand()
            : base(LinqSqlParser.Instance()) {
            this.Parameters = new WebSocketDbParameters();
            this.State = EnumCommandState.UnReady;
        }


        public override System.Data.IDbDataParameter CreateParameter() {
            return new WebSocketDbParameter();
        }

        protected DataSet Result { get; set; }

        private static Regex ParamSignRegex = new Regex("@[a-zA-Z0-9_-]+");
        protected override DataSet Invoke() {
            this.Prepare();
            if (this.Transaction != null) {
                //在会话情况下无法返回数据，需要保留此时的状态
                ((WebSocketDbTransaction)this.Transaction).AddCommand(this.Clone() as WebSocketDbCommand);
                return new DataSet();
            }
            WebSocketDbConnection conn = Connection as WebSocketDbConnection;
            WebSocketDbParameter[] paras = new WebSocketDbParameter[this.Parameters.Count];
            this.Parameters.CopyTo(paras, 0);
            try {
                this.State = EnumCommandState.Excuting;
                //针对命令进行填充和修改 改为符合 方法._id的通信方式
                //paras作为下发参数
                this.Result = new DataSet();
                this.Result.Tables.Add(new DataTable());
                this.Result.Tables[0].Columns.Add("result", typeof(string));
                this.Result.Tables[0].Rows.Add(this.Result.Tables[0].NewRow());
                string commandtext = this.CommandText.Trim();
                IDictionary<string, object> idic = new Dictionary<string, object>();
                paras.Where(p => { idic[p.ParameterName] = p.Value; return false; }).Count();
                if (commandtext.IndexOf("@") >= 0) {
                    commandtext = ParamSignRegex.Replace(commandtext, p => {
                        var ret = p.Value.Trim('@');
                        if (idic.ContainsKey(ret)) return "" + idic[ret];
                        else
                            return "";
                    });
                }
                this.Result.Tables[0].Rows[0]["result"] = conn.Invoke(commandtext.ToLower(), idic) ? "ok" : "not found";
                this.Result.AcceptChanges();
                this.State = EnumCommandState.Success;
                return this.Result;
            } catch {
                this.State = EnumCommandState.Error;
                throw;
            }
        }

        public override object Clone() {
            return new WebSocketDbCommand() {
                Connection = this.Connection,
                CommandText = this.CommandText,
                CommandTimeout = this.CommandTimeout,
                CommandType = this.CommandType,
                Transaction = null,
                QueryType = this.QueryType,
                Parameters = ((WebSocketDbParameters)this.Parameters).Clone() as IDataParameterCollection
            };
        }

        protected override IDataReader CreateReader(DataSet dt, CommandBehavior behavior) {
            return new WebSocketDbReader(dt, behavior);
        }

        protected override IDataReader CreateReader(DataSet dt) {
            return new WebSocketDbReader(dt);
        }

        public override string ParamSign {
            get { return ""; }
        }

        protected override void Invoke(QueryEntity[] entitis, NoSQLConnection conn, DataSet ds) {
            throw new NotImplementedException();
        }
    }
}