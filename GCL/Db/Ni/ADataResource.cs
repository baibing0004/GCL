using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
namespace GCL.Db.Ni
{
    /// <summary>
    /// 数据源父类
    /// </summary>
    public abstract class ADataResource : IDataResource
    {


        private IDataAbstractFactory fac;
        private ArrayList connstrings;
        public ADataResource(IDataAbstractFactory factory, ArrayList connstrings)
        {
            this.fac = factory;
            this.connstrings = connstrings;
        }

        public ADataResource(IDataAbstractFactory factory, string connstring)
            : this(factory, new ArrayList(new string[] { connstring }))
        {
        }

        /// <summary>
        /// 建议进行健康检查方式处理
        /// </summary>
        /// <returns></returns>
        protected IDbConnection CreateConnection()
        {
            IDbConnection conn = this.fac.CreateConnection();
            int times = 0;
            Exception ex = null;
            do
            {
                times++;
                try
                {
                    conn.ConnectionString = Convert.ToString(connstrings[GetRandomNum(connstrings.Count)]);
                    conn.Open();
                }
                catch (Exception e)
                {
                    ex = e;
                }
            } while (conn.State != ConnectionState.Open && times < 3);
            if (conn.State == ConnectionState.Open)
                return conn;
            else
                throw ex != null ? ex : new Exception("未能获得正常的连接!");
        }

        /// <summary>
        /// 这里简单实现随机读库。建议可以添加方法实现健康检查等方式进行处理
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected int GetRandomNum(int length)
        {
            if (length > 1)
            {
                return new Random(new Guid().GetHashCode()).Next(length - 1);
            }
            else return 0;
        }

        #region IDataAbstractBase Members

        public IDbCommand CreateCommand()
        {
            return this.fac.CreateCommand();
        }

        public IDbDataAdapter CreateAdapter()
        {
            return this.fac.CreateAdapter();
        }

        public IDbDataParameter CreateParameter()
        {
            return this.fac.CreateParameter();
        }

        public DbType ParseType(string type)
        {
            return this.fac.ParseType(type);
        }

        #endregion

        #region IDataResource Members

        public abstract IDbConnection GetConnection();

        public abstract void SetConnection(IDbConnection conn);

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion


        /// <summary>
        /// 说明诸如变量名定义符号譬如@ & $等等
        /// </summary>
        public string ParamSign
        {
            get { return this.fac.ParamSign; }
        }


        public string GetFacotryTypeName()
        {
            return this.fac.GetType().Name;
        }
    }
}
