using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using GCL.Db.Ni.NoSQL;

namespace GCL.Db.Ni.MongoDB {
    public class MongoDbCommand : GCL.Db.Ni.NoSQL.NoSQLCommand {
        public MongoDbCommand(ISqlParser parser)
            : base(parser) {
            this.Parameters = new MongoDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        public MongoDbCommand()
            : base(LinqSqlParser.Instance()) {
            this.Parameters = new MongoDbParameters();
            this.State = EnumCommandState.UnReady;
        }
        protected override IDataReader CreateReader(DataSet dt, CommandBehavior behavior) {
            return new MongoDbReader(dt, behavior);
        }

        protected override IDataReader CreateReader(DataSet dt) {
            return new MongoDbReader(dt);
        }

        public override IDbDataParameter CreateParameter() {
            return new MongoDbParameter();
        }

        public override object Clone() {
            return new MongoDbCommand() {
                Connection = this.Connection,
                CommandText = this.CommandText,
                CommandTimeout = this.CommandTimeout,
                CommandType = this.CommandType,
                Transaction = null,
                QueryType = this.QueryType,
                Parameters = ((MongoDbParameters)this.Parameters).Clone() as IDataParameterCollection
            };
        }

        public override string ParamSign {
            get { return "@"; }
        }

        protected override void Invoke(NoSQL.QueryEntity[] entitis, NoSQLConnection conn, DataSet ds) {
            MongoDbConnection mcon = conn as MongoDbConnection;
            if (mcon == null || mcon.State != ConnectionState.Open) { throw new Exception("数据库连接不可用!"); }
            if (entitis != null) {
                entitis.Where(p => {

                    #region 计算键值

                    string cacheKey = p.Table + ";" + Guid.NewGuid().ToString();

                    #endregion

                    #region 开始单条语句的处理
                    switch (p.Method.ToLower().Trim()) {
                        case "select":
                            #region 查询
                            if (p.WhereParam.Count() == 0) {
                                #region findAll
                                var result = mcon.CurrentDb.GetCollection(p.Table).FindAll();
                                DataTable dt = new DataTable();
                                if (result.Documents.Any()) {
                                    result.Documents.First().ToArray().Where(f => {
                                        if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f.Key))
                                            dt.Columns.Add(new DataColumn(f.Key, f.Value.GetType()));
                                        return false;
                                    }).Count();
                                    result.Documents.ToArray().Where(f => {
                                        DataRow row = dt.NewRow();
                                        f.ToArray().Where(f2 => {
                                            if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f2.Key))
                                                row[f2.Key] = f2.Value;
                                            return false;
                                        }).Count();
                                        dt.Rows.Add(row);
                                        return false;
                                    }).Count();
                                    dt.AcceptChanges();
                                    ds.AcceptChanges();
                                    ds.Tables.Add(dt);
                                }
                                #endregion
                            } else if ((p.LimitParam.Count() > 0 ? Math.Abs(Convert.ToInt32(p.LimitParam.Values.First())) : 0) == 1) {
                                #region findOne
                                Document res = mcon.CurrentDb.GetCollection(p.Table).FindOne(ToDocument(p.WhereParam));
                                if (res != null) {
                                    DataTable dt = new DataTable { TableName = cacheKey };
                                    var data = res.ToArray();
                                    data.Where(f => {
                                        if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f.Key))
                                            dt.Columns.Add(new DataColumn(f.Key, f.Value.GetType()));
                                        return false;
                                    }).Count();
                                    DataRow dr = dt.Rows.Add();
                                    data.Where(f2 => {
                                        if (p.MethodParam.Count() == 0 || p.MethodParam.ContainsKey(f2.Key))
                                            dr[f2.Key] = f2.Value;
                                        return false;
                                    }).Count();
                                    dt.Rows.Add(dr);
                                    dt.AcceptChanges();
                                    ds.AcceptChanges();
                                    ds.Tables.Add(dt);
                                }
                                #endregion
                            } else {
                                #region find

                                Document docSelect = ToDocument(p.MethodParam);
                                //取消Mongo默认的ID列
                                docSelect.Add("-id", -1);
                                var result = mcon.CurrentDb.GetCollection(p.Table).Find(
                                    ToDocument(p.WhereParam),
                                    p.LimitParam.Count() > 0 ? -1 * Convert.ToInt32(p.LimitParam.Values.First()) : 0,
                                    p.SkipParam.Count() > 0 ? Convert.ToInt32(p.SkipParam.Values.First()) : 0,
                                    docSelect
                                );
                                DataTable dt = new DataTable();
                                if (result.Documents.Any()) {
                                    result.Documents.First().ToArray().Where(f => {
                                        dt.Columns.Add(new DataColumn(f.Key, f.Value.GetType()));
                                        return false;
                                    }).Count();
                                    result.Documents.ToArray().Where(f => {
                                        DataRow row = dt.NewRow();
                                        f.ToArray().Where(f2 => {
                                            row[f2.Key] = f2.Value;
                                            return false;
                                        }).Count();
                                        dt.Rows.Add(row);
                                        return false;
                                    }).Count();
                                    dt.AcceptChanges();
                                    ds.AcceptChanges();
                                    ds.Tables.Add(dt);
                                }
                                #endregion
                            }
                            #endregion
                            break;
                        case "insert":
                            #region 增加
                            mcon.CurrentDb.GetCollection(p.Table).Insert(ToDocumentList(p.MethodParam), true);{
                                DataTable dt = new DataTable() { TableName = cacheKey, Columns = { new DataColumn("value", typeof(int)) } };
                                ds.Tables.Add(dt);
                                dt.Rows.Add(1);
                                dt.AcceptChanges();
                                ds.AcceptChanges();
                            }
                            #endregion
                            break;
                        case "update":
                            #region 更新
                            //建议可以测试 
                            var docWhere = ToDocument(p.WhereParam);
                            var count = mcon.CurrentDb.GetCollection(p.Table).Find(docWhere).Documents.LongCount();
                            if (count > 0) {
                                mcon.CurrentDb.GetCollection(p.Table).UpdateAll(ToDocument(p.MethodParam), docWhere);
                                //因为不能保证更新的范围只能以查询范围来统计
                                DataTable dt = new DataTable() { TableName = cacheKey, Columns = { new DataColumn("value", typeof(long)) } };
                                ds.Tables.Add(dt);
                                dt.Rows.Add(count);
                                dt.AcceptChanges();
                                ds.AcceptChanges();
                            }
                            /*原版
                            ICursor findup = null;
                            do {
                                findup = mcon.CurrentDb.GetCollection(p.Table).Find(
                                      ToDocument(p.WhereParam),
                                      p.LimitParam.Count() > 0 ? -1 * Convert.ToInt32(p.LimitParam.Keys.First()) : 0,
                                      p.SkipParam.Count() > 0 ? Convert.ToInt32(p.SkipParam.Keys.First()) : 0,
                                      ToDocument(p.MethodParam)
                                  ).Sort(ToDocument(p.OrderParam));
                                if (!findup.Documents.Any()) break;
                                if (!ds.Tables.Contains(cacheKey)) {
                                    DataTable dt = new DataTable() { Columns = { new DataColumn("value", typeof(int)) } };
                                    dt.TableName = cacheKey;
                                    dt.Rows.Add(findup.Documents.LongCount());
                                    ds.Tables.Add(dt);
                                    ds.AcceptChanges();
                                }
                            } while (findup.Documents.Any());*/
                            #endregion
                            break;
                        case "delete":
                            #region 删除
                            if (p.WhereParam.Count() == 0) {
                                mcon.CurrentDb.GetCollection(p.Table).Remove(new Document(), true);
                            } else
                                mcon.CurrentDb.GetCollection(p.Table).Remove(ToDocument(p.WhereParam), true); {
                                DataTable dt = new DataTable() { Columns = { new DataColumn("value", typeof(int)) } };
                                dt.TableName = cacheKey;
                                dt.Rows.Add(1);
                                ds.Tables.Add(dt);
                                dt.AcceptChanges();
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

        protected static readonly IDictionary<string, string> DICOperate = new Dictionary<string, string>();
        static MongoDbCommand() {
            "=;,>;$gt,<;$lt,>=;$gte,<=;$lte,<>;$ne,not;$not,and;,or;$or".Split(',').Where(f => { var f2 = f.Split(';'); DICOperate[f2[0]] = f2[1]; return false; }).Count();
        }

        /// <summary>
        /// 仅限一级操作使用
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Document> ToDocumentList(IDictionary<string, object> dictionary) {
            IList<Document> lt = new List<Document>();
            var doc = new Document();
            lt.Add(doc);
            dictionary.Where(p => {
                doc.Add(p.Key, p.Value);
                return false;
            }).Count();
            return lt;
        }

        /// <summary>
        /// 将整个QueryEntity中的字典转化成Document
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        protected virtual Document ToDocument(IDictionary<string, object> dictionary) {
            Document doc = new Document();
            string key = "";
            dictionary.Where(p => {
                key = DICOperate.ContainsKey(p.Key) ? DICOperate[p.Key] : p.Key;
                if (p.Value is IDictionary<string, object>) {
                    doc.Add(key, ToDocument(p.Value as IDictionary<string, object>));
                } else if (p.Value is IList<IDictionary<string, object>>) {
                    var values = new List<Document>();
                    doc.Add(key, values);
                    (p.Value as IList<IDictionary<string, object>>).Where(f => {
                        values.Add(ToDocument(f));
                        return false;
                    }).Count();
                } else {
                    doc.Add(key, p.Value);
                }
                return false;
            }).Count();
            return doc;
        }
    }

}
