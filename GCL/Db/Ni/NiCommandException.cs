using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    public class NiCommandException : Exception {
        public NiCommandException(IDbCommand com, string message, Exception innerException) : base(GetExceptionDesc(com) + message, innerException) { }
        public NiCommandException(IDbCommand com, Exception innerException) : base(GetExceptionDesc(com), innerException) { }
        public static string GetExceptionDesc(IDbCommand com) {
            StringBuilder sb = new StringBuilder();
            try {
                sb.AppendLine("调用命令:" + com.CommandText);
                sb.AppendLine("调用类型:" + com.CommandType.ToString());
                sb.AppendLine("是否事务:" + com.Transaction == null ? "否" : "是");

                if (com.Parameters != null) {
                    sb.AppendLine("参数列表:");
                    sb.AppendLine("=================================================================");
                    sb.AppendLine("|名称\t值\t类型\t长度\t参数方向\t|");
                    sb.AppendLine("=================================================================");
                    foreach (IDbDataParameter para in com.Parameters)
                        sb.AppendFormat("|{0}\t{1}\t{2}\t{3}\t{4}\t|\r\n", new object[] { para.ParameterName, para.Value, para.DbType, para.Size, para.Direction });
                    sb.AppendLine("=================================================================");
                } else
                    sb.AppendLine("无参数!");
            } catch {
                sb.Append("Command对象可能已释放，请查看InnerException");
            }
            string v = sb.ToString();
            sb.Remove(0, sb.Length);
            return v;
        }
    }
}
