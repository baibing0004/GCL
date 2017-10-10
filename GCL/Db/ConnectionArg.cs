using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using GCL.Event;
namespace GCL.Db {
    public abstract class ConnectionArg:EventArg {
        /*
	 * 构造函数
	 * 
	 * @param host
	 *            主机位置
	 * @param port
	 *            端口号
	 * @param dbName
	 *            默认数据库名
	 * @param user
	 *            用户名
	 * @param pwd
	 *            密码
	 */
        public ConnectionArg(string dbDriver,string jdbc,string host,
                string port,string dbName,string user,string pwd)
            : base(new string[] { dbDriver.Trim(), jdbc.Trim(), host.Trim(),
				port.Trim(), dbName.Trim(), user.Trim(), pwd.Trim() }) {

        }

        /*
         * @return 驱动程序名 比如：com.mysql.jdbc.Driver
         */
        public string GetDBDriver() {
            return this.ToStringOfPara(0);
        }

        /*
         * @param data
         *            驱动程序名 比如：com.mysql.jdbc.Driver
         */
        public void SetDBDriver(string data) {
            this.SetPara(0,data);
        }

        /*
         * @return jdbc连接串 比如：jdbc:mysql 中的 mysql
         */
        public string GetJDBC() {
            return this.ToStringOfPara(1);
        }

        /*
         * @param data
         *            jdbc连接串 比如：jdbc:mysql 中的 mysql
         */
        public void SetJDBC(string data) {
            this.SetPara(1,data);
        }

        /*
         * @return 主机名 比如：127.0.0.1
         */
        public string GetHost() {
            return this.ToStringOfPara(2);
        }

        /*
         * @param data
         *            主机名 比如：127.0.0.1
         */
        public void SetHost(string data) {
            this.SetPara(2,data);
        }

        /*
         * // *
         * 
         * @return 端口号 比如：3306
         */
        public string GetPort() {
            return this.ToStringOfPara(3);
        }

        /*
         * @param data
         *            默认数据库名 比如：mySQL
         */
        public void SetPort(string data) {
            this.SetPara(3,data);
        }

        /*
         * @return 默认数据库名 比如：mySQL
         */

        public string GetDBName() {
            return this.ToStringOfPara(4);
        }

        /*
         * @param data
         *            默认数据库名 比如：mySQL
         */
        public void SetDBName(string data) {
            this.SetPara(4,data);
        }

        /*
         * @return 用户名 比如：root
         */
        public string GetUser() {
            return this.ToStringOfPara(5);
        }

        /*
         * @param data
         *            用户名 比如：root
         */
        public void SetUser(string data) {
            this.SetPara(5,data);
        }

        /*
         * @return 密码 比如：pwd
         */
        public string GetPWD() {
            return this.ToStringOfPara(6);
        }

        /*
         * @param data
         *            密码 比如：pwd
         */
        public void SetPWD(string data) {
            this.SetPara(6,data);
        }

        /*
         * 适用于GetConnection(URL)
         * 
         * @return 返回全数据库连接串
         */
        public abstract string GetConnectstring();

        /*
         * 适用于GetConnection(URL,USER,PWD)
         * 
         * @return 获得短数据库连接串
         */
        public abstract string GetShortConnectstring();

        /*
         * 在使用前请注意加载和注册驱动程序 Class.forName(GetDBDriver()); 使用（URL,User,Pwd）方法新建数据库联接
         * 
         * @return 返回新建立的数据库联接
         * @throws Exception
         */
        public virtual DbConnection CreateConnection() {
            if(GetDBDriver().Trim().ToUpper().StartsWith("SQLOLEDB"))
                return new System.Data.SqlClient.SqlConnection(this.GetConnectstring());
            else if(GetDBDriver().Trim().ToUpper().StartsWith("ODBC"))
                return new System.Data.Odbc.OdbcConnection(this.GetConnectstring());
            else
                return new System.Data.OleDb.OleDbConnection(this.GetConnectstring());
        }
    }
}
