using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GCL.Db.Ni.NoSQL {
    /// <summary>
    /// 陶晓飞
    /// 类SQL语句处理
    /// </summary>
    public class LSqlParser : LinqSqlParser {
        public LSqlParser() : base() { }
        public override QueryEntity ParseSigalSQL(string p, string sign) {
            #region 将类SQL转换为类LinqSQL todo
            string sss = Regex.Replace(Regex.Replace(p, "[(]", " ( "), "[)]", " ) ");
            string sRegex = "[ ]+";
            //将多个空格,换行换成一个空格
            string item1 = Regex.Replace(Regex.Replace(sss, "\r\n", " "), sRegex, " ");
            //取关键点索引位置
            string temp = item1.ToLower();
            int from1 = temp.IndexOf(" from ");
            int where1 = temp.IndexOf(" where ");
            int skip1 = temp.IndexOf(" skip ");
            int limit1 = temp.IndexOf(" limit ");
            int order1 = temp.IndexOf(" order ");
            int by1 = temp.IndexOf(" by ");
            int set1 = temp.IndexOf(" set ");
            int into1 = temp.IndexOf(" into ");
            int values1 = temp.IndexOf(" values ");
            int dateTime1 = temp.IndexOf(" datetime ");
            string whereMember = string.Empty;
            string method = string.Empty;
            string methodMember = string.Empty;
            string tableStr = string.Empty;
            string orderby = string.Empty;
            object sk = null;
            object lm = null;
            object date = null;
            StringBuilder sb = new StringBuilder();
            //语句中包含from
            if (from1 != -1) {
                string[] s = item1.Substring(0, from1).Trim().Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Count() >= 2) {
                    method = s[0].Trim().ToLower();
                    methodMember = s[1].Trim();
                } else method = s[0].Trim().ToLower();

            }
            //语句中不包含from但包含set,update语句处理
            if (set1 != -1 && from1 == -1) {
                string[] s = item1.Substring(0, set1).Trim().Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                method = s[0].Trim().ToLower();
                tableStr = s[1].Trim();
                if (where1 != -1) {
                    methodMember = item1.Substring(set1 + 4, where1 - set1 - 4).Trim();
                } else {
                    methodMember = item1.Substring(set1 + 4).Trim();
                }
            }
            //插入语句处理
            if (from1 == -1 && into1 != -1 && set1 == -1) {
                method = item1.Substring(0, into1).Trim().ToLower();
                if (values1 != -1) {
                    string[] s = item1.Substring(into1 + 5, values1 - into1 - 5).Trim().Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    tableStr = s[0].Trim();
                    for (int i = 1; i < s.Length; i++) {
                        methodMember += s[i].Trim();
                    }


                    string[] tp;
                    if (dateTime1 != -1) {
                        tp = item1.Substring(values1 + 7, dateTime1 - values1 - 7).Replace("(", "").Replace(")", "").Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        date = item1.Substring(dateTime1 + 9).Trim(';').Trim();
                    } else
                        tp = item1.Substring(values1 + 7).Replace("(", "").Replace(")", "").Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    StringBuilder tt = new StringBuilder();
                    string[] mem = methodMember.Replace("(", "").Replace(")", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < mem.Length; i++) {
                        tt.AppendFormat("{0}={1},", mem[i], tp[i]);
                    }
                    methodMember = tt.Remove(tt.Length - 1, 1).ToString();
                }
            }
            //取表名
            if (where1 != -1 && from1 != -1) tableStr = item1.Substring(from1 + 5, where1 - from1 - 5).Trim();
            if (where1 == -1 && set1 == -1 && from1 != -1) tableStr = item1.Substring(from1 + 5).Trim();
            //只支持增删改查
            if (!method.Equals("select") && !method.Equals("delete") && !method.Equals("update") && !method.Equals("insert")) throw new InvalidOperationException("只支持select,insert,update,delete.请检查查询语句");
            sb.Append(tableStr + "." + method);
            if (!(methodMember.Contains("*") || string.IsNullOrEmpty(methodMember) || string.IsNullOrWhiteSpace(methodMember))) sb.Append("(" + methodMember + ")");
            if (dateTime1 != -1 && date != null) sb.AppendFormat(".datetime({0})", date);
            //排序处理
            if (order1 != -1) {
                whereMember = item1.Substring(where1 + 6, order1 - where1 - 6).Trim();
                sb.Append(".where(" + whereMember + ")");
                if (skip1 != -1) {
                    orderby = item1.Substring(by1 + 3, skip1 - by1 - 3).Trim();
                    if (orderby.Contains(sign))
                        throw new InvalidOperationException("排序中不允许参数化");
                }
                if (skip1 == -1 && limit1 != -1) {
                    orderby = item1.Substring(by1 + 3, limit1 - by1 - 3).Trim();
                }
                if (skip1 == -1 && limit1 == -1 && dateTime1 == -1) {
                    orderby = item1.Substring(by1 + 3).Trim();
                }
                if (orderby.Contains(sign))
                    throw new InvalidOperationException("排序中不允许参数化");
                sb.Append(".order(" + orderby + ")");
            }
            if (order1 == -1 && skip1 == -1 && limit1 == -1 && into1 == -1 && dateTime1 == -1 && where1 != -1) {
                whereMember = item1.Substring(where1 + 6).Trim();
                sb.Append(".where(" + whereMember + ")");
            }
            if (order1 == -1 && skip1 != -1) {
                whereMember = item1.Substring(where1 + 6, skip1 - where1 - 6).Trim();
                sb.Append(".where(" + whereMember + ")");
            }
            if (order1 == -1 && skip1 == -1 && limit1 != -1) {
                whereMember = item1.Substring(where1 + 6, limit1 - where1 - 6).Trim();
                sb.Append(".where(" + whereMember + ")");
            }
            //跳转处理
            if (skip1 != -1) {
                if (limit1 != -1) {
                    sk = item1.Substring(skip1 + 5, limit1 - 5 - skip1).Trim();
                    sb.AppendFormat(".skip({0})", sk);
                } else if (dateTime1 == -1 && limit1 == -1) {
                    sk = item1.Substring(skip1 + 5).Trim();
                    sb.AppendFormat(".skip({0})", sk);
                }
            }
            //限制条数处理
            if (limit1 != -1 && dateTime1 == -1) {
                lm = item1.Substring(limit1 + 6).Trim();
                sb.AppendFormat(".limit({0})", lm);
            }
            p = sb.ToString();
            #endregion
            return base.ParseSigalSQL(p, sign);
        }
    }
}
