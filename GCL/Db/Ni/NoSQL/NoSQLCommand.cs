using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;
using GCL.IO;

namespace GCL.Db.Ni.NoSQL
{
    /// <summary>
    /// 根据命令执行过程修改状态
    /// </summary>
    public enum EnumCommandState
    {
        UnReady,
        Ready,
        Excuting,
        Success,
        Error,
    }

    /// <summary>
    /// NoSQL基类
    /// 流程是
    /// Prepare检查输入参数
    /// Parse 进行SQL翻译和缓存
    /// PreapareParam 进行参数化值填充
    /// Invoke 真正调用数据库连接进行处理
    /// </summary>
    public abstract class NoSQLCommand : System.Data.IDbCommand, ICloneable, INeedNiDataResult
    {

        public NoSQLCommand(ISqlParser sqlParser)
        {
            this.State = EnumCommandState.UnReady;
            this.parser = sqlParser;
        }

        public virtual void Cancel()
        {
            if (this.State == EnumCommandState.Excuting) { throw new InvalidOperationException("不可终止"); }
        }

        public virtual string CommandText { get; set; }

        public virtual int CommandTimeout { get; set; }

        public virtual System.Data.CommandType CommandType { get; set; }

        protected NoSQLConnection conn;

        public virtual System.Data.IDbConnection Connection
        {
            get
            {
                return conn;
            }
            set
            {
                if (value == null || value is NoSQLConnection) this.conn = value as NoSQLConnection;
                else
                    throw new InvalidOperationException("请使用ObjectConnection的实例!");
            }
        }

        public abstract System.Data.IDbDataParameter CreateParameter();

        /// <summary>
        /// 采用默认方式先解析SQL,然后直接参数化 最后执行真实操作的顺序进行处理
        /// ParseSQL解析 表名<主键,主键>.insert/delete/update/select/(列=@变量).where(列=@变量).order(列 asc/desc).skip(页*页码).limit(总数)方式进行判断
        /// </summary>
        /// <returns></returns>
        protected virtual DataSet Invoke()
        {
            this.Prepare();
            if (this.Transaction != null)
            {
                //在会话情况下无法返回数据，需要保留此时的状态
                ((NoSQLTransaction)this.Transaction).AddCommand(this.Clone() as NoSQLCommand);
                return new DataSet();
            }
            NoSQLConnection conn = Connection as NoSQLConnection;
            NoSQLParameter[] paras = new NoSQLParameter[this.Parameters.Count];
            this.Parameters.CopyTo(paras, 0);
            try
            {
                this.State = EnumCommandState.Excuting;
                QueryEntity[] entitis = this.Parse(this.CommandText);
                this.PrepareParam(entitis, this.Parameters);
                DataSet ds = new DataSet();
                this.Invoke(entitis, conn, ds);
                this.State = EnumCommandState.Success;
                return ds;
            }
            catch
            {
                this.State = EnumCommandState.Error;
                throw;
            }
        }

        /// <summary>
        /// 命令当前状态
        /// </summary>
        public virtual EnumCommandState State { get; protected set; }
        /// <summary>
        /// 查询命令
        /// </summary>
        public virtual EnumQueryType QueryType { get; protected set; }

        public virtual int ExecuteNonQuery()
        {
            this.QueryType = EnumQueryType.ExecuteNonQuery;
            DataSet ds = this.Invoke();
            return ds.Tables.Count;
        }

        protected abstract System.Data.IDataReader CreateReader(DataSet dt, CommandBehavior behavior);
        protected abstract System.Data.IDataReader CreateReader(DataSet dt);
        public virtual System.Data.IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
        {
            this.QueryType = EnumQueryType.ExecuteReader;
            return CreateReader(this.Invoke(), behavior);
        }

        public virtual System.Data.IDataReader ExecuteReader()
        {
            this.QueryType = EnumQueryType.ExecuteReader;
            return CreateReader(this.Invoke());
        }

        public virtual object ExecuteScalar()
        {
            this.QueryType = EnumQueryType.ExecuteScalar;
            DataSet ds = this.Invoke();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Count > 0)
                return ds.Tables[0].Rows[0][0];
            else return string.Empty;
        }

        /// <summary>
        /// 一般只有DataAdapter可以调用
        /// </summary>
        /// <returns></returns>
        public virtual DataSet ExecuteQuery()
        {
            this.QueryType = EnumQueryType.ExecuteQuery;
            return this.Invoke();
        }

        public virtual System.Data.IDataParameterCollection Parameters { get; protected set; }

        /// <summary>
        /// 这里没有任何用处不会提前提交命令
        /// </summary>
        public virtual void Prepare()
        {
            if (this.Connection == null) throw new InvalidOperationException("请先设置Connection");
            if (string.IsNullOrEmpty(this.CommandText)) throw new InvalidOperationException("请先设置CommandText");
            this.CommandText = this.CommandText.Trim();
            this.State = EnumCommandState.Ready;
        }

        public virtual System.Data.IDbTransaction Transaction { get; set; }

        public virtual System.Data.UpdateRowSource UpdatedRowSource { get; set; }

        public virtual void Dispose()
        {
            if (this.State == EnumCommandState.Excuting)
            {
                throw new InvalidOperationException("执行中不可停止");
            }
        }

        public abstract object Clone();

        public NiDataResult DataResult
        {
            get
            {
                if (this.Transaction != null)
                {
                    return ((GCL.Db.Ni.NoSQL.NoSQLTransaction)this.Transaction).DataResult;
                }
                else return null;
            }
            set
            {
                if (this.Transaction != null)
                {
                    ((GCL.Db.Ni.NoSQL.NoSQLTransaction)this.Transaction).DataResult = value;
                }
            }
        }
        #region Sql转换
        protected ISqlParser parser;
        public static IDictionary<string, QueryEntity[]> StaticQueryEntity = new Dictionary<string, QueryEntity[]>();

        protected virtual QueryEntity[] Parse(string sql)
        {
            string key = "K" + GCL.Common.Tool.GetCRCHashCode(sql);
            if (!StaticQueryEntity.ContainsKey(key))
            {
                lock (StaticQueryEntity)
                {
                    if (!StaticQueryEntity.ContainsKey(key))
                    {
                        QueryEntity[] entitis = this.ParseSQL(sql);
                        if (entitis != null)
                            StaticQueryEntity.Add(key, entitis);
                    }
                }
            }
            QueryEntity[] ret = StaticQueryEntity[key];
            return ret.Select(p => p.Clone(ParamSign)).ToArray();
        }


        protected Regex regex = new Regex("\\s+");
        /// <summary>
        /// 将SQL语句转换成QueryEntity数组
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected virtual QueryEntity[] ParseSQL(string sql)
        {
            sql = string.IsNullOrEmpty(sql) ? "" : regex.Replace(sql.Trim(), " ");
            if (string.IsNullOrEmpty(sql)) return new QueryEntity[0];
            IList<QueryEntity> lstQuery = new List<QueryEntity>();
            sql.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(p =>
            {
                lstQuery.Add(ParseSigalSQL(p, this.ParamSign));
                return false;
            }).Count();
            return lstQuery.ToArray();
        }

        /// <summary>
        /// 管理单条SQL的解析
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        protected virtual QueryEntity ParseSigalSQL(string p, string sign)
        {
            return this.parser.ParseSigalSQL(p, sign);
        }
        public abstract string ParamSign { get; }
        #endregion
        protected virtual void PrepareParam(QueryEntity[] entitis, IDataParameterCollection cols)
        {
            if (entitis != null)
            {
                IDictionary<string, object> idic = new Dictionary<string, object>();
                foreach (IDbDataParameter para in cols)
                {
                    idic.Add(para.ParameterName, para.Value);
                }
                entitis.Where(p =>
                {
                    p.Params.Where(p2 =>
                    {
                        //todo 有的是查询key 需要进行修改 有的是查询Values需要进行修改
                        //规定p.Params必须存放最后一级Dictionary
                        if (idic.ContainsKey(p2.Key))
                        {
                            //一定注意 Params仅仅记录一级 其Key或者Value必有一项是p2.Key而且只修改Value为真实数据
                            p2.Value.Where(p3 =>
                            {
                                //针对 order skip 等字段
                                if (p3.ContainsKey(p2.Key)) { p3[p2.Key] = idic[p2.Key]; }
                                p3.Where(p4 => Convert.ToString(p4.Value).Equals(p2.Key)).Select(p4 => p4.Key).ToArray()
                                    .Where(p4 => { p3[p4] = idic[p2.Key]; return false; }).Count();
                                return false;
                            }).Count();
                        }
                        else throw new ArgumentException("没有找到变量\"" + p2.Key + "\"对应的值");
                        return false;
                    }).Count();
                    return false;
                }).Count();
            }
        }

        /// <summary>
        /// 需要各个NoSQL对象实现真正的Invoke操作 但是需要处理Transaction
        /// </summary>
        /// <param name="entitis"></param>
        /// <param name="conn"></param>
        /// <param name="result"></param>
        protected abstract void Invoke(QueryEntity[] entitis, NoSQLConnection conn, DataSet ds);
    }
}