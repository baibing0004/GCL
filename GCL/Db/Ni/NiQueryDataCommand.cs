using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace GCL.Db.Ni {
    public class NiQueryDataCommand : IDataCommand {

        #region IDataCommand Members

        public virtual void ExcuteCommand(IDataResource res, IDbCommand command, NiDataResult result) {
            //为ObjectDb特别处理
            if (command is INeedNiDataResult) {
                ((INeedNiDataResult)command).DataResult = result;
            }
            object v;
            if(command.CommandText.ToLower().StartsWith("delete"))  v = command.ExecuteNonQuery();
            else v = command.ExecuteScalar();
            FillDataTable(result.DataSet, command, "Query", v);
        }

        #endregion
        /// <summary>
        /// 这里主要是对表的一些设置
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void FillDataTable(DataSet ds, IDbCommand command, string name, object v) {
            DataTable dt = ds.Tables.Add();
            if (command.CommandType == CommandType.StoredProcedure && command.CommandText.IndexOf(" ") < 0) {
                if (ds.Tables.Contains(command.CommandText)) ds.Tables.Remove(command.CommandText);
                dt.TableName = command.CommandText;
            }
            command.Connection = null;
            FillDataTable(dt, name, v);
        }

        /// <summary>
        /// 这里主要处理表内的具体字段
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static DataTable FillDataTable(DataTable dt, string name, object v) {
            if (v == null) dt.Columns.Add(new DataColumn(name, typeof(string)));
            else dt.Columns.Add(new DataColumn(name, v.GetType()));
            DataRow row = dt.Rows.Add();
            row[0] = v;
            dt.Rows.Add();
            return dt;
        }
    }
}
