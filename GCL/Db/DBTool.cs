using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using GCL.Common;
using GCL.Event;
using System.Reflection;
using System.Linq;

namespace GCL.Db {
    /// <summary>
    /// 需要大改 否则不可使用线程池与其他框架相结合
    /// </summary>
    public abstract class DBTool : Tool {
        /// <summary>
        /// 格式化联接字符串 主要将Provider=?;去掉
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static string ToSQLDBConnString(object connString) {
            return ToSQLDBConnString(ToStringValue(connString));
        }

        /// <summary>
        /// 格式化联接字符串 主要将Provider=?;去掉
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static string ToSQLDBConnString(string connString) {
            return Regex.Replace(GetValue(connString), @"Provider=\S*;", "");
        }

        /// <summary>
        /// 更新数据集到数据库中
        /// </summary>
        /// <param name="sda"></param>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void Update(DbDataAdapter sda, DbConnection conn, DataTable dt) {
            try {
                sda.InsertCommand.Connection = sda.UpdateCommand.Connection = sda.DeleteCommand.Connection = conn;
                sda.InsertCommand.CommandTimeout = sda.UpdateCommand.CommandTimeout = sda.DeleteCommand.CommandTimeout = 8000;
            } catch {
            }

            DataTable tmpDt = dt.GetChanges();
            try {
                sda.Update(tmpDt);
            } finally {
                conn.Close();
            }

            dt.DataSet.Merge(tmpDt);
        }

        /// <summary>
        /// 更新数据集到数据库中
        /// </summary>
        /// <param name="sda"></param>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void Update(DbDataAdapter sda, DbConnection conn, DataSet ds) {
            try {
                sda.InsertCommand.Connection = sda.UpdateCommand.Connection = sda.DeleteCommand.Connection = conn;
                sda.InsertCommand.CommandTimeout = sda.UpdateCommand.CommandTimeout = sda.DeleteCommand.CommandTimeout = 8000;
            } catch {
            }

            DataSet tmpDs = ds.GetChanges();
            try {
                sda.Update(tmpDs);
            } finally {
                conn.Close();
            }

            ds.Merge(tmpDs);
        }

        /// <summary>
        /// 更新数据命令到数据库中
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        public static void ExecuteNonQueryClose(DbCommand cmd, DbConnection conn) {
            cmd.Connection = conn;
            cmd.CommandTimeout = 8000;
            try {
                cmd.ExecuteNonQuery();
            } finally {
                conn.Close();
            }
        }

        /// <summary>
        /// 运行数据查询指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int ExecuteQuery(DbCommand cmd, DbConnection conn) {
            cmd.Connection = conn;
            cmd.CommandTimeout = 8000;
            int result = -1;
            try {
                string temp = cmd.ExecuteScalar().ToString();
                if (temp != null && !temp.Trim().Equals(""))
                    result = int.Parse(temp);
            } finally {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 添加新的数据到数据库中并返回id
        /// </summary>
        /// <param name="cmdInsert"></param>
        /// <param name="cmdSerMaxId"></param>
        /// <param name="conn"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static int AddNewItem(DbCommand cmdInsert, DbCommand cmdSelMaxId, DbConnection conn) {
            cmdInsert.Connection = conn;
            cmdSelMaxId.Connection = conn;
            DbTransaction tran = conn.BeginTransaction();
            cmdInsert.CommandTimeout = 8000;
            cmdSelMaxId.CommandTimeout = 8000;
            int result = -1;
            try {
                cmdInsert.Transaction = tran;
                cmdInsert.ExecuteNonQuery();

                cmdSelMaxId.Transaction = tran;
                result = int.Parse(cmdSelMaxId.ExecuteScalar().ToString());

                tran.Commit();
            } catch (Exception ex) {
                tran.Rollback();
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 添加新的数据到数据库中并返回id
        /// </summary>
        /// <param name="cmdInsert"></param>
        /// <param name="cmdSerMaxId"></param>
        /// <param name="conn"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static int AddNewItemClose(DbCommand cmdInsert, DbCommand cmdSelMaxId, DbConnection conn) {
            int result = -1;
            try {
                result = AddNewItem(cmdInsert, cmdSelMaxId, conn);
            } finally {
                conn.Close();
            }
            return result;
        }


        /// <summary>
        /// 填充数据集
        /// </summary>
        /// <param name="sda"></param>
        /// <param name="cmdSel"></param>
        /// <param name="conn"></param>
        /// <param name="ds"></param>
        public static void FillDataSet(DbDataAdapter sda, DbCommand cmdSel, DbConnection conn, DataSet ds) {
            sda.SelectCommand = cmdSel;
            cmdSel.Connection = conn;
            cmdSel.CommandTimeout = 8000;
            try {
                sda.Fill(ds);
            } finally {
                conn.Close();
            }
        }

        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="sda"></param>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void FillDataTable(DbDataAdapter sda, DbCommand cmdSel, DbConnection conn, DataTable dt) {
            sda.SelectCommand = cmdSel;
            cmdSel.Connection = conn;
            cmdSel.CommandTimeout = 8000;
            try {
                sda.Fill(dt);
            } finally {
                conn.Close();
            }
        }


        /// <summary>
        /// 返回查询的具体值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="distinct"></param>
        /// <param name="top"></param>
        /// <param name="Desc"></param>
        /// <param name="sda"></param>
        /// <param name="dt"></param>
        /// <param name="conn"></param>
        public static void Select(string table, string where, bool distinct, int top, string orderItem, bool desc, DbDataAdapter sda, DataTable dt, DbConnection conn) {
            DbCommand com = conn.CreateCommand();
            com.CommandTimeout = 8000;
            com.CommandText = SelectString(table, where, distinct, top, orderItem, desc);
            FillDataTable(sda, com, conn, dt);
        }

        /// <summary>
        /// 返回查询的具体值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="distinct"></param>
        /// <param name="top"></param>
        /// <param name="Desc"></param>
        /// <param name="sda"></param>
        /// <param name="dt"></param>
        /// <param name="conn"></param>
        public static void Select(string table, string where, bool distinct, int top, string orderItem, bool desc, DbDataAdapter sda, DataSet ds, DbConnection conn) {
            DbCommand com = conn.CreateCommand();
            com.CommandTimeout = 8000;
            com.CommandText = SelectString(table, where, distinct, top, orderItem, desc);
            FillDataSet(sda, com, conn, ds);
        }

        /// <summary>
        /// 获得查询字符串
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="distinct"></param>
        /// <param name="top"></param>
        /// <param name="orderItem"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static string SelectString(string table, string where, bool distinct, int top, string orderItem, bool desc) {
            string selStr = "Select * FROM ";

            if (distinct)
                selStr = selStr.Replace("*", "DISTINCT *");

            if (top > 0)
                selStr = selStr.Replace("*", "TOP " + top.ToString().Trim() + " *");

            selStr += table.Trim();

            string result = selStr;

            if (where != null && where.Trim().Equals("") == false)
                result = string.Format("{0} WHERE {1}", selStr, where.Trim());

            if (orderItem != null && orderItem.Trim().Equals("") == false && desc)
                result += (" ORDER BY " + orderItem.Trim() + (desc ? " DESC" : ""));

            return result;
        }


        /// <summary>
        /// 获得统计字符串
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public static string SelectCountString(string table, string where, bool distinct) {
            string selStr = "Select COUNT(*) AS Expr1 FROM ";

            if (distinct)
                selStr = selStr.Replace("COUNT(*)", "DISTINCT COUNT(*)");

            selStr += table.Trim();

            string result = selStr;

            if (where != null && where.Trim().Equals("") == false)
                result = string.Format("{0} WHERE {1}", selStr, where.Trim());

            return result;
        }


        /// <summary>
        /// 返回查询的数目
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="distinct"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int SelectCount(string table, string where, bool distinct, DbConnection conn) {
            DbCommand com = conn.CreateCommand();
            com.CommandText = SelectCountString(table, where, distinct);
            com.CommandTimeout = 8000;

            int result = -1;
            try {
                result = (int)com.ExecuteScalar();
            } finally {
                conn.Close();
            }

            return result;
        }

        /// <summary>
        /// 处理有关Reader的方法
        /// </summary>
        /// <param name="reader"></param>
        public delegate void DealReader(DbDataReader reader, EventArg e);

        /// <summary>
        /// 默认处理Reader的方法
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="e"></param>
        public static void _DealReader(DbDataReader reader, EventArg e) {
        }


        /// <summary>
        /// 处理没有Reader时的事件
        /// </summary>
        public delegate void DealNoReader(EventArg e);

        /// <summary>
        /// 默认处理没有Reader或者Reader没有结果集的方法
        /// </summary>
        /// <param name="e"></param>
        public static void _DealNoReader(EventArg e) {
        }

        /// <summary>
        /// 对Reader进行逐行处理
        /// </summary>
        /// <param name="dealReader">处理Reader的方法</param>
        /// <param name="com">相关命令对象</param>
        public static void DoQuery(DealReader dealReader, DbCommand com, EventArg e) {
            DoQuery(dealReader, _DealNoReader, com, e);
        }


        /// <summary>
        /// 对Reader进行逐行处理
        /// </summary>
        /// <param name="dealReader">处理Reader的方法</param>
        /// <param name="dealNoReader">处理无Reader的方法</param>
        /// <param name="com">相关命令对象</param>
        public static void DoQuery(DealReader dealReader, DealNoReader dealNoReader, DbCommand com, EventArg e) {
            DbDataReader reader = com.ExecuteReader();
            if (reader != null && reader.Read()) {
                do {
                    dealReader(reader, e);
                } while (reader.Read());
                try {
                    reader.Close();
                } catch {
                }
            } else
                dealNoReader(e);
        }

        /// <summary>
        /// 对Reader进行逐行处理
        /// </summary>
        /// <param name="dealReader">处理Reader的方法</param>
        /// <param name="sql">对应的SQL语句</param>
        /// <param name="con">数据库联接对象</param>
        public static void DoQuery(DealReader dealReader, string sql, DbConnection con, EventArg e) {
            DoQuery(dealReader, _DealNoReader, sql, con, e);
        }

        /// <summary>
        /// 对Reader进行逐行处理
        /// </summary>
        /// <param name="dealReader">处理Reader的方法</param>
        /// <param name="dealNoReader">处理无Reader时的方法</param>
        /// <param name="sql">对应的SQL语句</param>
        /// <param name="con">数据库联接对象</param>
        public static void DoQuery(DealReader dealReader, DealNoReader dealNoReader, string sql, DbConnection con, EventArg e) {
            DbCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            DoQuery(dealReader, dealNoReader, cmd, e);
        }

        /// <summary>
        /// 对Reader进行逐行处理
        /// </summary>
        /// <param name="dealReader">处理Reader的方法</param>
        /// <param name="dealNoReader">处理无Reader时的方法</param>
        /// <param name="sql">对应的SQL语句</param>
        /// <param name="con">数据库联接对象</param>
        public static void DoQueryClose(DealReader dealReader, DealNoReader dealNoReader, string sql, DbConnection con, EventArg e) {
            DbCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            try {
                DoQuery(dealReader, dealNoReader, cmd, e);
            } finally {
                con.Close();
            }
        }

        /*
         * TJSON 结构化的JSON表格数据 
         * 因为很多应用使用的都是列相同的结构化JSON数据这样原来设计JSON的用意就不太符合这个场景了。
         * 所以改变每行上的列定义为首行为列定义，然后就是行定义了，会节省大量的传输，一般测试会节省1/3以上,对于大数据量节省流量更多。
         * 但是需要配合JS在前台进行转换成JSON格式
         * 
         * JSON格式如下：[{"ID":"xxxxxxxxxxx","Name":"xxxx"},{"ID":"xxxxxxxxxxx","Name":"xxxx"},……]
         * TJSON格式如下：[["ID","Name"],["xxxxxxxxxxx","xxxx"],["xxxxxxxxxxx","xxxx"],……]
         * QJ.evalTJson = function(data){
         *   var res = [];
         *   //数据集
         *   $(data).each(function(w,t){
         *      //表
         *      var ret = [];
         *      $(t).each(function(i,v){
         *          //行
         *          if(i>0){
         *              var s = {};
         *              $(v).each(function(q,v2){
         *                 //列
	     *               s[t[0][q]] = v2;
         *              });
         *              ret[i-1]=s;
         *          }
	     *      });
         *      res[w] = ret;
         *   });
         *   return res;
         *}
         */

        /// <summary>
        /// 将数据转换成TJSON
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string ToTJSON(System.Data.DataTable table) {
            StringBuilder sb = new StringBuilder();
            try {
                ToTJSON(table, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        /// <summary>
        /// 将数据转换为JSON
        /// </summary>
        /// <param name="table"></param>
        public static void ToTJSON(System.Data.DataTable table, StringBuilder sb) {
            sb.Append("[");
            if (table.Rows.Count > 0) {
                StringBuilder sb2 = new StringBuilder();
                int count = table.Columns.Count;
                //加标题行
                sb.Append("[");
                for (int w = 0; w < count; w++) {
                    sb.AppendFormat("\"{0}\",", table.Columns[w].Caption);
                    sb2.Append("\"{").Append(w).Append("}\",");
                }
                sb.Remove(sb.Length - 1, 1).Append("],");

                string rowFormatting = sb2.ToString().TrimEnd(',');
                sb2.Remove(0, sb2.Length);
                sb2 = null;

                //加数据行
                foreach (DataRow dr in table.Rows) {
                    object[] data = new object[count];
                    for (int w = 0; w < count; w++) {
                        data[w] = (dr[w] != null ? dr[w].ToString().Replace("\\", "\\\\")
                            .Replace("\"", "\\\"")
                            .Replace("\n", "\\r\\n") : "NULL");
                    }
                    sb.Append("[").AppendFormat(rowFormatting, data).Append("],");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
        }

        /// <summary>
        /// 将数据转换成JSON
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static string ToTJSON(System.Data.DataView view) {
            StringBuilder sb = new StringBuilder();
            try {
                ToTJSON(view, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        /// <summary>
        /// 将数据转换为JSON
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static void ToTJSON(System.Data.DataView view, StringBuilder sb) {
            sb.Append("[");
            if (view.Count > 0) {
                StringBuilder sb2 = new StringBuilder();
                int count = view.Table.Columns.Count;
                //加标题行
                sb.Append("[");
                for (int w = 0; w < count; w++) {
                    sb.AppendFormat("\"{0}\",", view.Table.Columns[w].Caption);
                    sb2.Append("\"{").Append(w).Append("}\",");
                }
                sb.Remove(sb.Length - 1, 1).Append("],");

                string rowFormatting = sb2.ToString().TrimEnd(',');
                sb2.Remove(0, sb2.Length);
                sb2 = null;

                //加数据行
                foreach (DataRowView dr in view) {
                    object[] data = new object[count];
                    for (int w = 0; w < count; w++) {
                        data[w] = (dr[w] != null ? dr[w].ToString().Replace("\\", "\\\\")
                            .Replace("\"", "\\\"")
                            .Replace("\n", "\\r\\n") : "NULL");
                    }
                    sb.Append("[").AppendFormat(rowFormatting, data).Append("],");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
        }

        /// <summary>
        /// 将数据转换成TJSON格式
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sb"></param>
        public static void ToTJSON(System.Data.DataSet ds, StringBuilder sb) {
            sb.Append("[");
            if (ds != null && ds.Tables.Count > 0) {
                foreach (System.Data.DataTable dt in ds.Tables) { ToTJSON(dt, sb); sb.Append(","); }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
        }

        /// <summary>
        /// 将数据转换成TJSON格式
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sb"></param>
        public static string ToTJSON(System.Data.DataSet ds) {
            StringBuilder sb = new StringBuilder();
            try {
                ToTJSON(ds, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        /// <summary>
        /// 将数据转换成JSON
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string ToJSON(System.Data.DataTable table) {
            StringBuilder sb = new StringBuilder();
            try {
                ToJSON(table, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        /// <summary>
        /// 将数据转换为JSON
        /// </summary>
        /// <param name="table"></param>
        public static void ToJSON(System.Data.DataTable table, StringBuilder sb) {

            StringBuilder sb2 = new StringBuilder();
            int count = table.Columns.Count;
            for (int w = 0; w < count; w++) {
                sb2.Append("\"").Append(table.Columns[w].Caption).Append("\":\"{").Append(w).Append("}\",");
            }

            string rowFormatting = sb2.ToString().TrimEnd(',');
            sb2.Remove(0, sb2.Length);
            sb2 = null;
            sb.Append("[");
            foreach (DataRow dr in table.Rows) {
                object[] data = new object[count];
                for (int w = 0; w < count; w++) {
                    data[w] = (dr[w] != null ? dr[w].ToString().Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\r\\n") : "NULL");
                }
                sb.Append("{").AppendFormat(rowFormatting, data).Append("},");
            }
            if (table.Rows.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
        }

        public static DataTable ToTable<T>(T[] ary) where T : class {
            DataTable dt = new DataTable();
            Type type = typeof(T);
            {
                //属性判断
                var props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (props != null && props.Count() > 0) {
                    props.Where(p => {
                        switch (p.PropertyType.Name.ToLower()) {
                            case "int":
                            case "int32":
                            case "short":
                            case "int16":
                            case "byte":
                            case "long":
                            case "int64":
                            case "string":
                            case "float":
                            case "double":
                            case "decimal":
                            case "datetime":
                            case "uint":
                            case "uint16":
                            case "uint32":
                            case "uint64":
                            case "char":
                                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
                                break;
                            default:
                                dt.Columns.Add(new DataColumn(p.Name, typeof(string)));
                                break;
                        }
                        return false;
                    }).Count();
                    dt.AcceptChanges();
                    if (ary != null)
                        ary.Where(p => {
                            DataRow dr = dt.NewRow();
                            props.Where(p2 => {
                                switch (p2.PropertyType.Name.ToLower()) {
                                    case "int":
                                    case "int32":
                                        dr[p2.Name] = Convert.ToInt32(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "short":
                                    case "int16":
                                        dr[p2.Name] = Convert.ToInt16(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "byte":
                                        dr[p2.Name] = Convert.ToByte(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "long":
                                    case "int64":
                                        dr[p2.Name] = Convert.ToInt64(p2.GetValue(p, null) ?? 0);
                                        break;
                                    default:
                                    case "string":
                                        dr[p2.Name] = Convert.ToString(p2.GetValue(p, null) ?? "");
                                        break;
                                    case "float":
                                        dr[p2.Name] = Convert.ToSingle(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "double":
                                        dr[p2.Name] = Convert.ToDouble(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "decimal":
                                        dr[p2.Name] = Convert.ToDecimal(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "datetime":
                                        dr[p2.Name] = Convert.ToDateTime(p2.GetValue(p, null) ?? DateTime.MinValue);
                                        break;
                                    case "uint16":
                                        dr[p2.Name] = Convert.ToUInt16(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "uint":
                                    case "uint32":
                                        dr[p2.Name] = Convert.ToUInt32(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "uint64":
                                        dr[p2.Name] = Convert.ToUInt32(p2.GetValue(p, null) ?? 0);
                                        break;
                                    case "char":
                                        dr[p2.Name] = Convert.ToChar(p2.GetValue(p, null) ?? 0);
                                        break;
                                }
                                return false;
                            }).Count();
                            dt.Rows.Add(dr);
                            return false;
                        }).Count();
                    return dt;
                }
            }
            {
                var props = type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (props != null && props.Count() > 0) {
                    props.Where(p => {
                        switch (p.FieldType.Name.ToLower()) {
                            case "int":
                            case "int32":
                            case "short":
                            case "int16":
                            case "byte":
                            case "long":
                            case "int64":
                            case "string":
                            case "float":
                            case "double":
                            case "decimal":
                            case "datetime":
                            case "uint":
                            case "uint16":
                            case "uint32":
                            case "uint64":
                            case "char":
                                dt.Columns.Add(new DataColumn(p.Name, p.FieldType));
                                break;
                            default:
                                dt.Columns.Add(new DataColumn(p.Name, typeof(string)));
                                break;
                        }
                        return false;
                    }).Count();
                    dt.AcceptChanges();
                    if (ary != null)
                        ary.Where(p => {
                            DataRow dr = dt.NewRow();
                            props.Where(p2 => {
                                switch (p2.FieldType.Name.ToLower()) {
                                    case "int":
                                    case "int32":
                                        dr[p2.Name] = Convert.ToInt32(p2.GetValue(p) ?? 0);
                                        break;
                                    case "short":
                                    case "int16":
                                        dr[p2.Name] = Convert.ToInt16(p2.GetValue(p) ?? 0);
                                        break;
                                    case "byte":
                                        dr[p2.Name] = Convert.ToByte(p2.GetValue(p) ?? 0);
                                        break;
                                    case "long":
                                    case "int64":
                                        dr[p2.Name] = Convert.ToInt64(p2.GetValue(p) ?? 0);
                                        break;
                                    default:
                                    case "string":
                                        dr[p2.Name] = Convert.ToString(p2.GetValue(p) ?? "");
                                        break;
                                    case "float":
                                        dr[p2.Name] = Convert.ToSingle(p2.GetValue(p) ?? 0);
                                        break;
                                    case "double":
                                        dr[p2.Name] = Convert.ToDouble(p2.GetValue(p) ?? 0);
                                        break;
                                    case "decimal":
                                        dr[p2.Name] = Convert.ToDecimal(p2.GetValue(p) ?? 0);
                                        break;
                                    case "datetime":
                                        dr[p2.Name] = Convert.ToDateTime(p2.GetValue(p) ?? DateTime.MinValue);
                                        break;
                                    case "uint16":
                                        dr[p2.Name] = Convert.ToUInt16(p2.GetValue(p) ?? 0);
                                        break;
                                    case "uint":
                                    case "uint32":
                                        dr[p2.Name] = Convert.ToUInt32(p2.GetValue(p) ?? 0);
                                        break;
                                    case "uint64":
                                        dr[p2.Name] = Convert.ToUInt32(p2.GetValue(p) ?? 0);
                                        break;
                                    case "char":
                                        dr[p2.Name] = Convert.ToChar(p2.GetValue(p) ?? 0);
                                        break;
                                }
                                return false;
                            }).Count();
                            dt.Rows.Add(dr);
                            return false;
                        }).Count();
                    return dt;
                }
            }
            return dt;
        }

        /// <summary>
        /// 将数据转换成JSON
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static string ToJSON(System.Data.DataView view) {
            StringBuilder sb = new StringBuilder();
            try {
                ToJSON(view, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        /// <summary>
        /// 将数据转换为JSON
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static void ToJSON(System.Data.DataView view, StringBuilder sb) {
            StringBuilder sb2 = new StringBuilder();

            int count = view.Table.Columns.Count;
            for (int w = 0; w < count; w++) {
                sb2.Append(view.Table.Columns[w].Caption).Append(":\"{").Append(w).Append("}\",");
            }

            string rowFormatting = sb2.ToString().TrimEnd(',');
            sb2.Remove(0, sb.Length);
            sb2 = null;
            sb.Append("[");
            foreach (DataRowView dr in view) {
                object[] data = new object[count];
                for (int w = 0; w < count; w++) {
                    data[w] = (dr[w] != null ? dr[w].ToString().Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\r\\n") : "NULL");
                }
                sb.Append("{").AppendFormat(rowFormatting, data).Append("},");
            }
            if (view.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
        }

        /// <summary>
        /// 将数据转换成JSON格式
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sb"></param>
        public static void ToJSON(System.Data.DataSet ds, StringBuilder sb) {
            #region 记录输出结果
            sb.Append("[");
            if (ds != null && ds.Tables.Count > 0) {
                foreach (System.Data.DataTable dt in ds.Tables) { ToJSON(dt, sb); sb.Append(","); }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            #endregion
        }

        /// <summary>
        /// 将数据转换成JSON格式
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="sb"></param>
        public static string ToJSON(System.Data.DataSet ds) {
            StringBuilder sb = new StringBuilder();
            try {
                ToJSON(ds, sb);
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }
    }
}
