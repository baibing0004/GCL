using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GCL.Db.Ni.NoSQL;

namespace GCL.Db.Ni.RedisDB {
    public class RedisDbCommand : GCL.Db.Ni.NoSQL.NoSQLCommand {
        public RedisDbCommand(ISqlParser parser)
            : base(parser) {
            this.Parameters = new RedisDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        public RedisDbCommand()
            : base(LinqSqlParser.Instance()) {
            this.Parameters = new RedisDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        protected override IDataReader CreateReader(DataSet dt, CommandBehavior behavior) {
            return new RedisDbReader(dt, behavior);
        }

        protected override IDataReader CreateReader(DataSet dt) {
            return new RedisDbReader(dt);
        }

        public override IDbDataParameter CreateParameter() {
            return new RedisDbParameter();
        }

        public override object Clone() {
            return new RedisDbCommand() {
                Connection = this.Connection,
                CommandText = this.CommandText,
                CommandTimeout = this.CommandTimeout,
                CommandType = this.CommandType,
                Transaction = null,
                QueryType = this.QueryType,
                Parameters = ((RedisDbParameters)this.Parameters).Clone() as IDataParameterCollection
            };
        }

        public override string ParamSign {
            get { return "@"; }
        }

        protected override void Invoke(NoSQL.QueryEntity[] entitis, NoSQLConnection conn, DataSet ds) {
            RedisDbConnection rcon = conn as RedisDbConnection;
            if (rcon == null || rcon.State != ConnectionState.Open) { throw new Exception("数据库连接不可用!"); }
            if (entitis != null) {
                entitis.Where(p => {
                    #region 计算键值

                    string cacheKey = "";
                    //参数化之后的值
                    if (p.IDs.Count() > 0 || this.Parameters.Contains(this.ParamSign + "cacheKey"))
                        cacheKey += ("K" + DBTool.GetCRCHashCode(string.Join("_",
                            p.IDs.Count() > 0 ? p.IDs.Values.ToArray() :
                            new string[] { Convert.ToString((this.Parameters[this.ParamSign + "cacheKey"] as IDataParameter).Value) })));
                    else
                        throw new Exception(string.Format("表未定义<{0}主键>,或者传入键值中未找到{0}cacheKey", this.ParamSign));

                    #endregion

                    #region 开始单条语句的处理
                    switch (p.Method.ToLower().Trim()) {
                        case "select":
                            #region 查询
                            var value = Convert.ToString(rcon.Client.GetValueFromHash(p.Table, cacheKey));
                            if (!string.IsNullOrEmpty(value)) {
                                IDictionary<string, object> idic = DBTool.DeserializeToObject<IDictionary<string, object>>(value);
                                if (idic != null && idic.Count() > 0) {
                                    DataTable dt = new DataTable();
                                    ds.Tables.Add(dt);
                                    idic.Keys.Where(f => {
                                        if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f))
                                            dt.Columns.Add(new DataColumn(f, idic[f].GetType()));
                                        return false;
                                    }).Count();
                                    DataRow dr = dt.NewRow();
                                    idic.Keys.Where(f => {
                                        if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f))
                                            dr[f] = idic[f];
                                        return false;
                                    }).Count();
                                    dt.Rows.Add(dr);
                                    ds.AcceptChanges();
                                }
                            }
                            #endregion
                            break;
                        case "insert":
                        case "update":
                            #region 增加 和 更新一致
                            bool isSucc = false;
                            isSucc = rcon.Client.SetEntryInHashIfNotExists(p.Table, cacheKey, DBTool.SerializeToString(p.MethodParam));
                            if (isSucc) {
                                int datetime = p.DateTimeParam.Keys.Count() > 0 ? Convert.ToInt32(p.DateTimeParam.Values.First()) : rcon.DateTime;
                                if (datetime > 0) {
                                    rcon.Client.ExpireEntryAt(p.Table, DateTime.Now.AddSeconds(Convert.ToInt32(datetime)));
                                }
                                DataTable dt = new DataTable() { Columns = { new DataColumn("value", typeof(int)) } };
                                dt.TableName = cacheKey;
                                dt.Rows.Add(1);
                                ds.Tables.Add(dt);
                                ds.AcceptChanges();
                            }
                            #endregion
                            break;
                        case "delete":
                            #region 删除
                            if (rcon.Client.RemoveEntryFromHash(p.Table, cacheKey)) {
                                DataTable dt = new DataTable() { Columns = { new DataColumn("value", typeof(int)) } };
                                dt.TableName = cacheKey;
                                dt.Rows.Add(1);
                                ds.Tables.Add(dt);
                                ds.AcceptChanges();
                            } else
                                if (p.WhereParam.Count() == 0) {
                                    //整体删除似乎好像有点狠
                                    rcon.Client.RemoveEntry(p.Table);
                                }
                            #endregion
                            break;

                    }
                    #endregion
                    return false;
                }).Count();
            }
        }
    }
}