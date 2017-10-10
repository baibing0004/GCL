using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using GCL.Event;
namespace GCL.Db {
    public abstract class ConnectionArg:EventArg {
        /*
	 * ���캯��
	 * 
	 * @param host
	 *            ����λ��
	 * @param port
	 *            �˿ں�
	 * @param dbName
	 *            Ĭ�����ݿ���
	 * @param user
	 *            �û���
	 * @param pwd
	 *            ����
	 */
        public ConnectionArg(string dbDriver,string jdbc,string host,
                string port,string dbName,string user,string pwd)
            : base(new string[] { dbDriver.Trim(), jdbc.Trim(), host.Trim(),
				port.Trim(), dbName.Trim(), user.Trim(), pwd.Trim() }) {

        }

        /*
         * @return ���������� ���磺com.mysql.jdbc.Driver
         */
        public string GetDBDriver() {
            return this.ToStringOfPara(0);
        }

        /*
         * @param data
         *            ���������� ���磺com.mysql.jdbc.Driver
         */
        public void SetDBDriver(string data) {
            this.SetPara(0,data);
        }

        /*
         * @return jdbc���Ӵ� ���磺jdbc:mysql �е� mysql
         */
        public string GetJDBC() {
            return this.ToStringOfPara(1);
        }

        /*
         * @param data
         *            jdbc���Ӵ� ���磺jdbc:mysql �е� mysql
         */
        public void SetJDBC(string data) {
            this.SetPara(1,data);
        }

        /*
         * @return ������ ���磺127.0.0.1
         */
        public string GetHost() {
            return this.ToStringOfPara(2);
        }

        /*
         * @param data
         *            ������ ���磺127.0.0.1
         */
        public void SetHost(string data) {
            this.SetPara(2,data);
        }

        /*
         * // *
         * 
         * @return �˿ں� ���磺3306
         */
        public string GetPort() {
            return this.ToStringOfPara(3);
        }

        /*
         * @param data
         *            Ĭ�����ݿ��� ���磺mySQL
         */
        public void SetPort(string data) {
            this.SetPara(3,data);
        }

        /*
         * @return Ĭ�����ݿ��� ���磺mySQL
         */

        public string GetDBName() {
            return this.ToStringOfPara(4);
        }

        /*
         * @param data
         *            Ĭ�����ݿ��� ���磺mySQL
         */
        public void SetDBName(string data) {
            this.SetPara(4,data);
        }

        /*
         * @return �û��� ���磺root
         */
        public string GetUser() {
            return this.ToStringOfPara(5);
        }

        /*
         * @param data
         *            �û��� ���磺root
         */
        public void SetUser(string data) {
            this.SetPara(5,data);
        }

        /*
         * @return ���� ���磺pwd
         */
        public string GetPWD() {
            return this.ToStringOfPara(6);
        }

        /*
         * @param data
         *            ���� ���磺pwd
         */
        public void SetPWD(string data) {
            this.SetPara(6,data);
        }

        /*
         * ������GetConnection(URL)
         * 
         * @return ����ȫ���ݿ����Ӵ�
         */
        public abstract string GetConnectstring();

        /*
         * ������GetConnection(URL,USER,PWD)
         * 
         * @return ��ö����ݿ����Ӵ�
         */
        public abstract string GetShortConnectstring();

        /*
         * ��ʹ��ǰ��ע����غ�ע���������� Class.forName(GetDBDriver()); ʹ�ã�URL,User,Pwd�������½����ݿ�����
         * 
         * @return �����½��������ݿ�����
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
