using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GCL.Db.Ni.NoSQL;

namespace GCL.Db.Ni.MemcacheDB {
    public class MemDbCommand : GCL.Db.Ni.NoSQL.NoSQLCommand {
        public MemDbCommand(ISqlParser parser)
            : base(parser) {

            this.Parameters = new MemDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        public MemDbCommand()
            : base(LinqSqlParser.Instance()) {
            this.Parameters = new MemDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        protected override IDataReader CreateReader(DataSet dt, CommandBehavior behavior) {
            return new MemDbReader(dt, behavior);
        }

        protected override IDataReader CreateReader(DataSet dt) {
            return new MemDbReader(dt);
        }

        public override IDbDataParameter CreateParameter() {
            return new MemDbParameter();
        }

        public override object Clone() {
            return new MemDbCommand() {
                Connection = this.Connection,
                CommandText = this.CommandText,
                CommandTimeout = this.CommandTimeout,
                CommandType = this.CommandType,
                Transaction = null,
                QueryType = this.QueryType,
                Parameters = ((MemDbParameters)this.Parameters).Clone() as IDataParameterCollection
            };
        }

        public override string ParamSign {
            get { return "@"; }
        }

        protected override void Invoke(NoSQL.QueryEntity[] entitis, NoSQLConnection conn, DataSet ds) {
            MemDbConnection mcon = conn as MemDbConnection;
            if (mcon == null || mcon.State != ConnectionState.Open) { throw new Exception("数据库连接不可用!"); }
            if (entitis != null) {
                entitis.Where(p => {
                    #region 计算键值

                    string cacheKey = p.Table;
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
                            var value = Convert.ToString(mcon.Client.Get(cacheKey));
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
                            int datetime = p.DateTimeParam.Keys.Count() > 0 ? Convert.ToInt32(p.DateTimeParam.Values.First()) : mcon.DateTime;
                            if (datetime > 0) {
                                    isSucc = mcon.Client.Set(cacheKey, DBTool.SerializeToString(p.MethodParam), DateTime.Now.AddSeconds(datetime));
                            } else {
                                isSucc = mcon.Client.Set(cacheKey, DBTool.SerializeToString(p.MethodParam));
                            }
                            if (isSucc) {
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
                            bool isSucc2 = false;
                            if (mcon.Client.KeyExists(cacheKey)) {
                                isSucc2 = mcon.Client.Delete(cacheKey);
                            } else
                                if (p.WhereParam.Count() == 0) {
                                    //没有Where条件就删除所有 这个似乎有点过
                                    isSucc2 = mcon.Client.FlushAll();
                                }
                            if (isSucc2) {
                                DataTable dt = new DataTable() { Columns = { new DataColumn("value", typeof(int)) } };
                                dt.TableName = cacheKey;
                                dt.Rows.Add(1);
                                ds.Tables.Add(dt);
                                ds.AcceptChanges();
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