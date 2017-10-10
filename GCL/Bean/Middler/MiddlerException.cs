using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    public class MiddlerException : Exception {
        private string dll, type;
        private CreateMethod method;
        public MiddlerException(string dll, string type, CreateMethod method, object[] paras, Exception innerException) : base(GetExceptionDesc(dll, type, method, paras), innerException) { }
        public MiddlerException(string dll, string type, CreateMethod method, object[] paras, string text, Exception innerException) : base(GetExceptionDesc(dll, type, method, paras) + text, innerException) { }
        public static string GetExceptionDesc(string dll, string type, CreateMethod method, object[] paras) {
            StringBuilder sb = new StringBuilder();
            sb.Append("Dll:").AppendLine(dll);
            sb.Append("Type:").AppendLine(type);
            sb.Append("method:").AppendLine(method.ToString());
            for (int w = 0; paras != null && w < paras.Length; w++) {
                sb.AppendFormat("para{0}:\t{1}", (w + 1), BeanTool.GetValue(paras[w], "Null")).AppendLine();
            }
            string v = sb.ToString();
            sb.Remove(0, sb.Length);
            return v;
        }
    }
}


