using MongoDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GCL.Db.Ni.MongoDB
{
    public class BeanTool
    {
        //存储commandText里独立语句
        internal static Dictionary<string, QueryEntity> dicParas = new Dictionary<string, QueryEntity>();
        //存储commandText字典信息
        internal static Dictionary<string, List<QueryEntity>> dicPa = new Dictionary<string, List<QueryEntity>>();
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="commandText">命令行</param>
        /// <param name="paras">参数集</param>
        /// <param name="mongoDb">MongoDB实例</param>
        /// <returns>得到的DataSet结果</returns>
        internal static System.Data.DataSet Invoke(string commandText, System.Data.IDataParameterCollection paras, IMongoDatabase mongoDb)
        {
            if (!dicPa.ContainsKey(commandText))
            {
                //存储QueryEntity供dicPa字典使用
                List<QueryEntity> lq = new List<QueryEntity>();
                string[] ct = commandText.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in ct)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        #region MyRegion
                        /*
                          select * from table where a=1;
                          select * from table where a=1;
                          update table set a=2,b=3 where a=1;
                          update table set a=@a,b=@b where a=1;
                          delete from table where a=1;
                          insert into table (a,b) values (1,2);
                          insert into table (a,b) values (@a1,2);
                          select * from table where a=1 order by a ;
                          select * from table where a=1 order by a desc;
                          select * from table where a=1 order by a,b desc,c,d desc;
                          select * from table where a=1 order by @a;
                          select * from table where a=1 order by a desc skip 23;
                          select * from table where a=1 order by a,b desc,c,d desc skip @b;
                          select * from table where a=1 order by b asc skip 2 limit 3;
                          seelct * from table where a=@c order by b asc skip @d limit @e;
                          select * from table where a=@c order by b asc skip @c limit @c;
                          seelct * from table where a=1 and b<2;
                          select * from table where a=1 and b<2 or c>3;
                          select * from table where a=1 or b<2 and c>3;
                          select * from table where not a=@a and not (b<@b or c>@c) and not d>=@d and not (e<=@e or f<>6);
                         */

                        /*
                         * table.select.where(a=1);
                           table<a,b,c,d>.select.where(a=1);
                           table.update(a=2,b=3).where(a=1);
                           table.update(a=@a,b=@b).where(a=1);
                           table.delete.where(a=1);
                           table.insert(a=1,b=2);
                           table.insert(a=@a1,b=2);
                           table.select.where(a=1).order(a);
                           table.select.where(a=1).order(a desc);
                           table.select.where(a=1).order(a,b desc,c,d desc);
                           table.select.where(a=1).order(@a);
                           table.select.where(a=1).order(a desc).skip(23);
                           table.select.where(a=1).order(a,b desc,c,d desc).skip(@b);
                           table.select.where(a=1).order(b asc).skip(2).limit(3);
                           table.select.where(a=@c).order(b asc).skip(@d).limit(@e);
                           table.select.where(a=@c).order(b asc).skip(@c).limit(@c);
                           table.select.where(a=1 and b<2);
                           table.select.where(a=1 and b<2 or c>3);
                           table.select.where(a=1 or b<2 and c>3);
                           table.select.where(not a=@a and not (b<@b or c>@c) and not d>=@d and not (e<=@e or f<>6));
                         */
                        #endregion
                        //select * from table where a=1;
                        string sRegex = "[ ]+";
                        //将多个空格,换行换成一个空格
                        string item1 = Regex.Replace(Regex.Replace(item, "\r\n", " "), sRegex, " ");
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
                        string whereMember = string.Empty;
                        string method = string.Empty;
                        string methodMember = string.Empty;
                        string tableStr = string.Empty;
                        string orderby = string.Empty;
                        object sk = null;
                        object lm = null;
                        StringBuilder sb = new StringBuilder();
                        //语句中包含from
                        if (from1 != -1)
                        {
                            string[] s = item1.Substring(0, from1).Trim().Split(new char[] { ' ','\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (s.Count() >= 2)
                            {
                                method = s[0].Trim().ToLower();
                                methodMember = s[1].Trim();
                            }
                            else method = s[0].Trim().ToLower();

                        }
                        //语句中不包含from但包含set,update语句处理
                        if (set1 != -1 && from1 == -1)
                        {
                            string[] s = item1.Substring(0, set1).Trim().Split(new char[] { ' ','\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
                            method = s[0].Trim().ToLower();
                            tableStr = s[1].Trim();
                            if (where1 != -1)
                            {
                                methodMember = item1.Substring(set1 + 4, where1 - set1 - 4).Trim();
                            }
                            else
                            {
                                methodMember = item1.Substring(set1 + 4).Trim();
                            }
                        }
                        //插入语句处理
                        if (from1 == -1 && into1 != -1 && set1 == -1)
                        {
                            method = item1.Substring(0, into1).Trim().ToLower();
                            if (values1 != -1)
                            {
                                string[] s = item1.Substring(into1 + 5, values1-into1-5).Trim().Split(new char[] { ' ','\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
                                tableStr = s[0].Trim();
                                methodMember = s[1].Trim();
                                StringBuilder stab = new StringBuilder();
                                tableStr = stab.Append(tableStr + methodMember.Replace('(', '<').Replace(')', '>')).ToString();
                                string[] tp = item1.Substring(values1 + 7).Replace("(", "").Replace(")", "").Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                StringBuilder tt = new StringBuilder();
                                string[] mem = methodMember.Replace("(", "").Replace(")", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < mem.Length; i++)
                                {
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
                        //排序处理
                        if (order1 != -1)
                        {
                            whereMember = item1.Substring(where1 + 6, order1 - where1 - 6).Trim();
                            sb.Append(".where(" + whereMember + ")");
                            if (skip1 != -1)
                            {
                                orderby = item1.Substring(by1 + 3, skip1 - by1 - 3).Trim();
                                if (orderby.Contains("@")) 
                                    throw new InvalidOperationException("排序中不允许参数化");
                            }
                            if (skip1 == -1 && limit1 != -1)
                            {
                                orderby = item1.Substring(by1 + 3, limit1 - by1 - 3).Trim();
                            }
                            if (skip1 == -1 && limit1 == -1)
                            {
                                orderby = item1.Substring(by1 + 3).Trim();
                            }
                            if (orderby.Contains("@"))
                                throw new InvalidOperationException("排序中不允许参数化");
                            sb.Append(".order(" + orderby + ")");
                        }
                        if (order1 == -1 && skip1 == -1 && limit1 == -1 && into1 == -1)
                        {
                            whereMember = item1.Substring(where1 + 6).Trim();
                            sb.Append(".where(" + whereMember + ")");
                        }
                        if (order1 == -1 && skip1 != -1)
                        {
                            whereMember = item1.Substring(where1 + 6, skip1 - where1 - 6).Trim();
                            sb.Append(".where(" + whereMember + ")");
                        }
                        if (order1 == -1 && skip1 == -1 && limit1 != -1)
                        {
                            whereMember = item1.Substring(where1 + 6, limit1 - where1 - 6).Trim();
                            sb.Append(".where(" + whereMember + ")");
                        }
                        //跳转处理
                        if (skip1 != -1)
                        {
                            if (limit1 != -1)
                            {
                                sk = item1.Substring(skip1 + 5, limit1 - 5 - skip1).Trim();
                                sb.AppendFormat(".skip({0})", sk);
                            }
                            else
                            {
                                sk = item1.Substring(skip1 + 5).Trim();
                                sb.AppendFormat(".skip({0})", sk);
                            }
                        }
                        //限制条数处理
                        if (limit1 != -1)
                        {
                            lm = item1.Substring(limit1 + 6).Trim();
                            sb.AppendFormat(".limit({0})", lm);
                        }
                        File.AppendAllLines("1.txt", new string[] { sb.ToString() });


                        continue;
                        #region 切分命令行
                        //实例化查询实体
                        QueryEntity qe = new QueryEntity();
                        string key = item.Trim(new char[] { '\r', '\n', '\t', ' ' }).Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
                        if (!dicParas.ContainsKey(key))
                        {
                            //按.号拆分语句,语句规则:实体.动作(包括增删改查).where().sort().skip().limit();
                            string[] cms = item.Trim().Split(new char[] { '.', ';', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            qe.Table = cms[0];
                            cms.Where(p =>
                            {
                                if (!string.IsNullOrWhiteSpace(p))
                                {
                                    if (p.Contains("insert"))
                                    {
                                        qe.Method = "insert";
                                        CreateQueryDocument(qe, p, 6);
                                    }
                                    if (p.Contains("select") && !p.Contains("*") && !p.ToLower().Contains("top1"))
                                    {
                                        qe.Method = "select";
                                        CreateQueryDocumentByFind(qe, p, 6);
                                    }
                                    if (p.Contains("select") && !p.Contains("*") && p.ToLower().Contains("top1"))
                                    {
                                        qe.Method = "findone";
                                    }
                                    if (p.Contains("select") && p.Contains("*"))
                                    {
                                        qe.Method = "findall";
                                    }
                                    if (p.Contains("update"))
                                    {
                                        qe.Method = "update";
                                        CreateQueryDocument(qe, p, 6);
                                    }
                                    if (p.Contains("delete"))
                                    {
                                        qe.Method = "delete";
                                    }
                                    if (p.Contains("where"))
                                    {
                                        string ws = p.Substring(5, p.Length - 5);
                                        MongoDbInterpreter mi = new MongoDbInterpreter(ws);
                                        qe.WhereDocument = mi.ExpValue();
                                    }
                                    if (p.Contains("sort"))
                                    {
                                        Document doc = new Document();
                                        string s = p.Substring(5, p.Length - 6);
                                        doc.Add(s, 1);
                                        qe.SortDocument = doc;
                                    }
                                    if (p.Contains("skip"))
                                    {
                                        int skip = 0;
                                        string s = p.Substring(5, p.Length - 6);
                                        if (int.TryParse(s, out skip)) qe.SkipInt = skip;
                                        else qe.SkipInt = s;
                                    }
                                    if (p.Contains("limit"))
                                    {
                                        int limit = int.MaxValue;
                                        string st = p.Substring(6, p.Length - 7);
                                        if (int.TryParse(st, out limit)) qe.LimitInt = limit;
                                        else qe.LimitInt = st;
                                    }
                                }
                                return false;
                            }).Count();
                        }
                        QueryEntity q = new QueryEntity();
                        if (dicParas.ContainsKey(key))
                        {
                            q = dicParas[key];
                        }
                        else
                        {
                            dicParas.Add(key, qe);
                            q = qe.Clone();
                        }
                        lq.Add(q);
                        #endregion
                    }
                }
                dicPa.Add(commandText, lq);
            }
            DataSet ds = new DataSet(commandText.GetHashCode().ToString());
            foreach (QueryEntity temp in dicPa[commandText])
            {
                QueryEntity qy = temp.Clone();
                List<Document> lt = new List<Document>();
                if (qy.QueryDocument != null && qy.Method.ToLower().Equals("update")) qy.QueryDocument = MakeDocument(qy.QueryDocument, paras, new Document());
                if (qy.WhereDocument != null) qy.WhereDocument = MakeDocument(qy.WhereDocument, paras, new Document());
                if (qy.ListInsert.Count > 0)
                {
                    foreach (Document item in qy.ListInsert)
                    {
                        lt.Add(MakeDocument(item, paras, new Document()));
                    }
                    qy.ListInsert = lt;
                }
                int numTest;
                if (!int.TryParse(qy.SkipInt.ToString(), out numTest) || !int.TryParse(qy.LimitInt.ToString(), out numTest))
                {
                    foreach (MongoDbParameter item in paras)
                    {
                        if (item.ParameterName.Equals(qy.SkipInt.ToString())) qy.SkipInt = item.Value;
                        if (item.ParameterName.Equals(qy.LimitInt.ToString())) qy.LimitInt = item.Value;
                    }
                }
                int limit = Convert.ToInt32(qy.LimitInt);
                //返回表
                DataTable dt1 = new DataTable();
                //定义返回列
                DataColumn dc1 = new DataColumn("value", typeof(int));

                //处理类型
                #region 处理类型
                switch (qy.Method)
                {
                    case "select":
                        if (limit > 0) limit = -limit;
                        ICursor findres = mongoDb.GetCollection(qy.Table).Find(qy.WhereDocument, limit, Convert.ToInt32(qy.SkipInt), qy.QueryDocument).Sort(qy.SortDocument);

                        if (!findres.Documents.Any()) break;
                        IEnumerable<Document> enumDoc = findres.Documents;
                        List<Document> ld = new List<Document>();
                        foreach (var item in enumDoc)
                        {
                        }
                        int num = ld[0].Keys.Count;
                        string[] strColumn = new string[num];
                        ld[0].Keys.CopyTo(strColumn, 0);
                        //构建列名
                        for (int i = 0; i < num; i++)
                        {
                            DataColumn dcnew = new DataColumn();
                            dcnew.ColumnName = strColumn[i];
                            dt1.Columns.Add(dcnew);
                        }
                        //构建行
                        for (int j = 0; j < ld.Count; j++)
                        {
                            DataRow dr = dt1.NewRow();
                            object[] strj = new object[num];
                            ld[j].Values.CopyTo(strj, 0);
                            for (int i = 0; i < num; i++)
                            {
                                dr[i] = strj[i];
                            }
                            dt1.Rows.Add(dr);
                        }
                        ds.Tables.Add(dt1);
                        break;
                    case "findall":
                        var result = mongoDb.GetCollection(qy.Table).FindAll();
                        if (!result.Documents.Any()) break;
                        //构建列名
                        for (int i = 0; i < result.Documents.FirstOrDefault().Keys.Count; i++)
                        {
                            DataColumn dcnew = new DataColumn();
                            string[] str = new string[result.Documents.FirstOrDefault().Keys.Count];
                            result.Documents.FirstOrDefault().Keys.CopyTo(str, 0);
                            dcnew.ColumnName = str[i];
                            dt1.Columns.Add(dcnew);
                        }
                        //构建行
                        for (int j = 0; j < result.Documents.LongCount(); j++)
                        {
                            DataRow dr = dt1.NewRow();
                            object[] strj = new object[result.Documents.AsQueryable().ElementAt(j).Values.Count];
                            result.Documents.AsQueryable().ElementAt(j).Values.CopyTo(strj, 0);
                            for (int i = 0; i < result.Documents.FirstOrDefault().Keys.Count; i++)
                            {
                                dr[i] = strj[i];
                            }
                            dt1.Rows.Add(dr);
                        }
                        ds.Tables.Add(dt1);
                        break;
                    case "findone":
                        Document res = mongoDb.GetCollection(qy.Table).FindOne(qy.WhereDocument);
                        if (res == null) break;
                        DataRow drone = dt1.NewRow();
                        for (int i = 0; i < res.Count; i++)
                        {
                            DataColumn dcnew = new DataColumn();
                            dcnew.ColumnName = res.AsQueryable().ElementAt(i).Key;
                            dt1.Columns.Add(dcnew);

                            drone[i] = res.AsQueryable().ElementAt(i).Value;

                        }
                        dt1.Rows.Add(drone);
                        ds.Tables.Add(dt1);
                        break;
                    case "update":
                        var findup = mongoDb.GetCollection(qy.Table).Find(qy.WhereDocument, limit, Convert.ToInt32(qy.SkipInt), qy.QueryDocument).Sort(qy.SortDocument);
                        if (!findup.Documents.Any()) break;
                        dt1.Columns.Add(dc1);
                        dt1.Rows.Add(findup.Documents.LongCount());
                        while (findup.Documents.LongCount() > 0)
                        {
                            mongoDb.GetCollection(qy.Table).FindAndModify(qy.QueryDocument, qy.WhereDocument);
                            findup = mongoDb.GetCollection(qy.Table).Find(qy.WhereDocument, limit, Convert.ToInt32(qy.SkipInt), qy.QueryDocument).Sort(qy.SortDocument);
                            if (!findup.Documents.Any()) break;
                        }
                        ds.Tables.Add(dt1);
                        break;
                    case "insert":
                        mongoDb.GetCollection(qy.Table).Insert(qy.ListInsert, true);
                        dt1.Columns.Add(dc1);
                        dt1.Rows.Add(qy.ListInsert.Count);
                        ds.Tables.Add(dt1);
                        break;
                    case "delete":
                        mongoDb.GetCollection(qy.Table).Remove(qy.WhereDocument, true);
                        dt1.Columns.Add(dc1);
                        dt1.Rows.Add(1);
                        ds.Tables.Add(dt1);
                        break;
                    default:
                        throw new InvalidOperationException("暂不支持!");
                }
                #endregion
            }
            return ds;
        }
        /// <summary>
        /// 生成select的QueryDocument
        /// </summary>
        /// <param name="paras">参数</param>
        /// <param name="qe">QueryEntity实体</param>
        /// <param name="str">命令行</param>
        /// <param name="i">命令行括号前的标识,如:insert的长度,以截取</param>
        private static void CreateQueryDocumentByFind(QueryEntity qe, string str, int i)
        {
            string[] p = str.Substring(i).Split(new char[] { '(', ')', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            Document c = new Document();
            c.Add("_id", 0);
            foreach (string item in p)
            {
                c.Add(item.Trim(), 1);
            }
            qe.QueryDocument = c;
        }

        //{ "a1": "@a1", "$or": [ { "b": "@b2" }, { "c": "@b3" } ] }
        //{ "a1": "@a1", "$or": [ { "age": { "$gt": "@age" } }, { "c": "@c" } ] }
        //{ "a1": "@a1", { "age": { "$gt": "@age" } }}
        /// <summary>
        /// Document参数化
        /// </summary>
        /// <param name="dt">含参数的Document</param>
        /// <param name="paras">参数集</param>
        /// <param name="dcm">填充结果的Document</param>
        /// <returns>dcm</returns>
        private static Document MakeDocument(Document dt, IDataParameterCollection paras, Document dcm)
        {
            foreach (var item in dt)
            {
                Document dc = new Document();
                if (item.Value is Document)
                {
                    dcm.Add(item.Key, MakeDocument(item.Value as Document, paras, dc));
                }
                else if (item.Value is Document[])
                {
                    List<Document> list = new List<Document>();
                    foreach (Document t in item.Value as Document[])
                    {
                        Document ddd = new Document();
                        list.Add(MakeDocument(t, paras, ddd));
                    }
                    dcm.Add(item.Key, list.ToArray());
                }
                foreach (MongoDbParameter temp in paras)
                {
                    string s1 = item.Value.ToString().Trim();
                    string s2 = temp.ParameterName.Trim();
                    if (s1.Equals(s2)) dcm.Add(item.Key.Trim(), temp.Value);
                }
            }
            return dcm;
        }
        /// <summary>
        ///  生成ListInsert或填充Update方法的QueryDocument
        /// </summary>
        /// <param name="paras">参数</param>
        /// <param name="qe">QueryEntity实体</param>
        /// <param name="str">命令行字符串</param>
        /// <param name="i"></param>
        private static void CreateQueryDocument(QueryEntity qe, string str, int i)
        {
            Document dic = new Document();
            string[] pars = str.Substring(i).Split(new char[] { '(', ')', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in pars)
            {
                string[] p = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                dic.Add(p[0], p[1]);
            }
            if (qe.Method == "update") qe.QueryDocument = dic;

            else qe.ListInsert.Add(dic);

        }

        //public static void RecordLog(string p2)
        //{
        //    int p1 = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //    File.AppendAllText(@"F:\log" + p1 + ".txt", string.Format("{0}------{1}:{2}{3}", GCL.Common.Tool.FormatNow(), p1, p2, Environment.NewLine));
        //}

        //public static void RemoveLog()
        //{
        //    for (int i = 0; i < 20; i++)
        //    {
        //        File.Delete(@"F:\log" + i + ".txt");
        //    }
        //}
    }
}
