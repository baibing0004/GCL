using GCL.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GCL.Db.Ni.NoSQL {
    /// <summary>
    /// 实现类似linq.方式的SQL解析方式，并不是要实现类似PLSql方式的SQL解析方式
    /// </summary>
    public class LinqSqlParser : ISqlParser {
        public virtual QueryEntity ParseSigalSQL(string p, string sign) {
            QueryEntity entity = new QueryEntity();
            try {
                p.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Where(p2 => {
                    p2 = p2.Trim();
                    if (string.IsNullOrEmpty(entity.Table)) {
                        entity.Table = p2;
                        try {
                            if (entity.Table.IndexOf("<") >= 0) {
                                var tabs = p2.Split('<');
                                entity.Table = tabs[0].Trim();
                                entity.IDs = new Dictionary<string, object>();
                                tabs[1].Trim('>').Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => {
                                    if (!f.StartsWith(sign)) {
                                        throw new Exception("表主键必须设置为可接收的参数值:" + f);
                                    }
                                    entity.IDs.Add(f, 1);
                                    entity.AddParams(f, entity.IDs);
                                    return false;
                                }).Count();

                            }
                        } catch (Exception ex) {
                            throw new Exception("SQL在\"" + p.Substring(p.IndexOf("<")) + "\"处解析错误!\r\n:" + ex.ToString());
                        }
                    } else if (string.IsNullOrEmpty(entity.Method)) {
                        var method = p2.ToLower();
                        if (method.StartsWith("select")) {
                            #region 查询
                            entity.Method = "select";
                            p2 = p2.Substring(6).TrimStart('(').TrimEnd(')').Trim();
                            if (!(string.IsNullOrEmpty(p2) || "*".Equals(p2))) {
                                p2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => {
                                    entity.MethodParam[f] = 1;
                                    return false;
                                }).Count();
                            }
                            //这里取消对 * 的处理默认QueryParam为空时就是全部查询
                            #endregion
                        } else if (method.StartsWith("insert")) {
                            #region 增加
                            entity.Method = "insert";
                            p2 = p2.Substring(6).TrimStart('(').TrimEnd(')').Trim();
                            if (!(string.IsNullOrEmpty(p2) || "*".Equals(p2))) {
                                p2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => {
                                    var p3 = f.Split('=');
                                    if (p3.Length < 2) throw new Exception("SQL在\"" + p.Substring(p.IndexOf(f)) + "\"处解析错误!\r\n:" + f + "没有找到=号");
                                    entity.MethodParam[p3[0]] = p3[1];
                                    if (p3[1].StartsWith(sign)) { entity.AddParams(p3[1], entity.MethodParam); }
                                    return false;
                                }).Count();
                            } else {
                                throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".")) + "\"处解析错误!\r\n:没有找到可插入的列及其值");
                            }
                            #endregion
                        } else if (method.StartsWith("update")) {
                            #region 更新
                            entity.Method = "update";
                            p2 = p2.Substring(6).TrimStart('(').TrimEnd(')').Trim();
                            if (!(string.IsNullOrEmpty(p2) || "*".Equals(p2))) {
                                p2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => {
                                    var p3 = f.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (p3.Length < 2) throw new Exception("SQL在\"" + p.Substring(p.IndexOf(f)) + "\"处解析错误!\r\n:" + f + "没有找到=号");
                                    entity.MethodParam[p3[0]] = p3[1];
                                    if (p3[1].StartsWith(sign)) { entity.AddParams(p3[1], entity.MethodParam); }
                                    return false;
                                }).Count();
                            } else {
                                throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".")) + "\"处解析错误!\r\n:没有找到可更新的列及其值");
                            }
                            #endregion
                        } else if (method.StartsWith("delete")) {
                            #region 删除
                            entity.Method = "delete";
                            p2 = p2.Substring(6).TrimStart('(').TrimEnd(')').Trim();
                            if (!(string.IsNullOrEmpty(p2) || "*".Equals(p2))) {
                                throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".")) + "\"处解析错误!\r\n:不需要设置可删除的列及其值");
                            }
                            #endregion
                        } else {
                            //错误
                            throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".")) + "\"处解析错误!\r\n:没有找到可解析的操作指令");
                        }
                    } else {
                        //这里需要判断 where skip limit
                        var p3 = p2.Trim().ToLower();
                        if (p3.StartsWith("where")) {
                            #region where(a1 = 1 and b=2 or c=3 and d=4 and ())
                            //= > >= < <= <> 第一优先级 然后是 () 然后是 and 然后是 or 再添加对not的处理
                            IDictionary<string, int> ht = new Dictionary<string, int>();
                            int w = 0;
                            //取消)号 由(号识别其结束符
                            "=,>,<,>=,<=,<>,not,and,or,(,;".Split(',').Where(f => { ht[f] = w++; return false; }).Count();

                            //转换成二叉树
                            /* 目的是 0级与2级栈都为空
                             * 按照优先级 分为 0级（数据） 1级操作 =,>,<,>=,<=,<> 逻辑表达式栈 二级操作 not,and,or,() 按照流程 1级只能操作0级 2级只能操作1级 所以二级完整后会进入一级
                             * 分别建立 0,1,2三个未完成操作栈，1个未完成操作顺序栈和1个已完成操作栈说明当前缺乏的操作是几级的，当分解的字符进入分析时 确认字符级别
                             * 然后分别进入0级 
                             * 按照1级操作会从0级操作栈中取数据完成1级操作 如果没完成那么重新入栈，并将自己压入未完成操作栈，
                             * 当0级数据再进入时，检查未完成操作栈内容，然后提交自己给未完成的操作栈完成操作，如果完成操作，那么将自己压入1级已完成操作栈
                             * 当2级数据进入，按照2级操作会从1级操作栈中获取数据完成2级操作，如果没完成那么重新入栈，并将自己压入未完成操作栈，
                             * 当0级数据三次进入时，检查未完成操作栈内容，发现自己未满足级别 那么压入0栈
                             * 当1级数据二次进入时，检查0级数据获取数据完成1级操作 如果没完成那么重新入栈，并将自己压入未完成操作栈，
                             * 当0级数据四次进入时，检查未完成操作栈内容，发现自己够级别完成操作，调用1级数据完成操作，
                             * 然后检查未完成操作栈内容如果发现有2级操作那么继续调用2级操作符完成操作，否则如果发现和自己相同的操作级别报错
                             * 0级栈出现1个以上会是错误，括号会导致 2级站有多个 也会导致 1级站出现多个
                             * //数据栈
                            Stack<string> stack = new Stack<string>();
                            //操作符栈
                            Stack<string> opeStack = new Stack<string>();
                            IDictionary<string, object> curIdic = entity.WhereParam;
                            (p3 + ';').Split(' ').Where(f => {
                                if (ht.ContainsKey(f)) {
                                    if (index < 6)
                                        opeStack.Push(f);//括号前的
                                    else {
                                        //todo
                                    }
                                } else {
                                    //首字符入栈 或者尾字符入栈触发操作
                                    if (opeStack.Count > 0) {
                                        //尾部字母入栈
                                        var ope = opeStack.Pop();
                                        if (ope == null) //错误
                                            throw new Exception("SQL在\"" + p.Substring(p.IndexOf(f)) + "\"处解析错误!\r\n:没有找到缺少可解析的操作");
                                        //根据优先级进行处理
                                        if (ht[ope] < 1) {
                                            curIdic.Add(stack.Pop(), f);
                                            if (f.StartsWith(sign)) entity.AddParams(f, curIdic);
                                        } else if (ht[ope] < 6) {
                                            //"><>=<=<>":
                                            //需要转换 字典改成
                                            curIdic.Add(ope, new Dictionary<string, object> { { stack.Pop(), f } });
                                            if (f.StartsWith(sign)) entity.AddParams(f, curIdic[ope] as IDictionary<string, object>);
                                            //throw new Exception("SQL在\"" + p.Substring(p.IndexOf(ope + " " + f)) + "\"处解析错误!\r\n:没有找到可解析的操作指令" + ope);

                                        } else if (index < 8) {

                                        } else { }
                                    } else
                                        stack.Push(f);
                                }
                                return false;
                            }).Count();
                            //数据对应的Dictionary
                            Stack<IDictionary<string, object>> idcStack = new Stack<IDictionary<string, object>>();                            
                             */
                            /*
                             * 解释器+责任链式处理思路
                             * 缓存分为 非逻辑字符 与 已完成逻辑操作两个变量 和 未完成逻辑操作栈 保留未完成的操作
                             * LogicOperate 其每次 LIncome 非逻辑字符，已完成逻辑操作 RIncome 非逻辑字符，已完成逻辑操作与未处理字符，
                             * 如果发现此逻辑字符不为自己所能处理 将自己压栈等待下一个逻辑操作完成，当下一个操作完成时自动弹出下个操作调用将自己作为其另一种右进操作，直到语句结束 
                             * 发现字符未处理完成或者栈内仍然有未处理的操作 报错，如果缓存字符，已完成逻辑操作超过1个报错
                             **/
                            OperationSession session = new OperationSession();
                            p3 = p2.Trim();
                            p3 = p3.Substring(6, p3.Length - 7);
                            p3 = p3.Replace("=", " = ");
                            p3 = p3.Replace(">", " > ");
                            p3 = p3.Replace("<", " < ");
                            p3 = p3.Replace("(", " ( ");
                            p3 = p3.Replace(")", " ) ");
                            p3 = p3.Replace("<  >", "<>");
                            p3 = p3.Replace(">  =", ">=");
                            p3 = p3.Replace("<  =", "<=");
                            p3.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(f => {
                                try {
                                    if (ht.ContainsKey(f.ToLower())) {
                                        int index = ht[f.ToLower()];
                                        ALogicOperate ope = null;
                                        if (index < 1) {
                                            //表达式
                                            ope = new EqualOperate();
                                        } else if (index < 6) {
                                            ope = new O1Operate { Ope = f };
                                        } else if (index < 7) {
                                            ope = new NotOperate();
                                        } else if (index < 8) {
                                            ope = new AndOperate();
                                        } else if (index < 9) {
                                            //处理括号
                                            ope = new O2Operate { Ope = f };
                                        } else if (index < 10) {
                                            //处理括号
                                            ope = new LCOperate();
                                        }
                                        if (ope == null) throw new Exception("发现无法处理的操作符号" + f);
                                        ope.LIncome(session);
                                    } else {
                                        if (session.OperateStack.Count() == 0) session.Word = f;
                                        else session.OperateStack.Pop().RIncome(session, f);
                                    }
                                } catch (Exception ex) {
                                    throw new Exception("SQL在\"" + p.Substring(p.IndexOf(" " + f + " ")) + "\"处解析错误!\r\n:" + ex.ToString());
                                }
                                return false;
                            }).Count();
                            if (session.IsComplete) {
                                var ope = session.CompleteOperate;
                                entity.WhereParam = ope.ToDictionary(entity, sign);
                                if (ope.Level == 1) {
                                    //仅仅补充Level为1时的状态
                                    entity.WhereParam.Values.Where(f => {
                                        AddParams.Invoke(entity.WhereParam, f, entity, sign);
                                        return false;
                                    }).Count();
                                } else {
                                    #region 声势浩大的处理字典转数组 处理的原则是这里留下二级操作的肯定是不能合并的,所以发现有带;号的Key就去;号,如果有多个相同的操作符立即合并成数组且将剩余的字典去除操作符后放入数组中，如此递归
                                    GCL.Event.DynamicEventFunc func = null;
                                    func = new GCL.Event.DynamicEventFunc(k => {
                                        var kidc = k[0] as IDictionary<string, object>;
                                        var idcParent = k[1] as IDictionary<string, object>;
                                        var keyParent = Convert.ToString(k[2]);
                                        //查找操作符的未合并字典
                                        var keys = kidc.Keys.Where(kk => kk.IndexOf(";") >= 0).ToArray();
                                        if (keys.Select(kk => kk.Split(';')[0]).GroupBy(kk => kk).Where(kg => kg.Count() > 1).Count() > 0) {
                                            //出现 开始合并数组 深度优先遍历
                                            keys.Where(kk => {
                                                func.Invoke(kidc[kk], kidc, kk);
                                                return false;
                                            }).Count();
                                            //这里注意可能出现 kidc[kk]为数组的情况出现
                                            IList<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
                                            keys.Where(kk => {
                                                var newKey = kk.Split(';')[0];
                                                list.Add(new Dictionary<string, object> { { newKey, kidc[kk] } });
                                                kidc.Remove(kk);
                                                return false;
                                            }).Count();
                                            if (kidc.Count() > 0)
                                                list.Add(kidc);
                                            if (!string.IsNullOrEmpty(keyParent)) {
                                                idcParent[keyParent] = list.ToArray();
                                                list.Clear();
                                            }
                                        } else {
                                            keys.Where(kk => {
                                                //只要能进来的它的底层肯定是IDictionary<string,object>
                                                func.Invoke(kidc[kk], kidc, kk);
                                                var newKey = kk.Split(';')[0];
                                                kidc.Add(newKey, kidc[kk]);
                                                kidc.Remove(kk);
                                                return false;
                                            }).Count();
                                        }
                                    });
                                    //默认肯定不会出现在根部
                                    func.Invoke(entity.WhereParam, entity.WhereParam, "");
                                    if (entity.WhereParam.ContainsKey("and") && entity.WhereParam.Values.First() is IDictionary<string, object>) {
                                        //只有为and时可以去壳
                                        entity.WhereParam = entity.WhereParam.Values.First() as IDictionary<string, object>;
                                    }
                                    #endregion
                                }
                            } else
                                throw new Exception("SQL在\"" + p.Substring(p.IndexOf(" " + session.Word + " ")) + "\"处解析错误!\r\n:语句未完成");
                            #endregion
                        } else if (p3.StartsWith("order")) {
                            #region order(a1 asc,b1 desc)
                            var p4 = p2.TrimEnd(')').Split('(');
                            if (p4.Count() < 2) { throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".order")) + "\"处解析错误!\r\n:skip没有找到可用的参数"); }
                            p4[1].Split(',').Where(f => {
                                var f2 = f.Split(' ');
                                entity.OrderParam[f2[0].Trim()] = f2.Length < 2 ? "asc" : f2[1].Trim();
                                return false;
                            }).Count();
                            #endregion
                        } else if (p3.StartsWith("skip")) {
                            #region skip(@page) skip(11)
                            //todo 支持表达式
                            var p4 = p2.TrimEnd(')').Split('(');
                            if (p4.Count() < 2) { throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".skip")) + "\"处解析错误!\r\n:skip没有找到可用的参数"); }
                            entity.SkipParam[p4[1]] = 1;
                            if (p4[1].StartsWith(sign)) entity.AddParams(p4[1], entity.SkipParam);
                            #endregion
                        } else if (p3.StartsWith("limit")) {
                            #region limit(@page) limit(11)
                            //todo 支持表达式
                            var p4 = p2.TrimEnd(')').Split('(');
                            if (p4.Count() < 2) { throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".limit")) + "\"处解析错误!\r\n:skip没有找到可用的参数"); }
                            entity.LimitParam[p4[1]] = 1;
                            if (p4[1].StartsWith(sign)) entity.AddParams(p4[1], entity.LimitParam);
                            #endregion
                        } else if (p3.StartsWith("datetime") || p3.StartsWith("timeout")) {
                            #region datetime(@time) datetime(200) timeout(@time) timeout(2000)
                            //todo 支持表达式
                            var p4 = p2.TrimEnd(')').Split('(');
                            if (p4.Count() < 2) { throw new Exception("SQL在\"" + p.Substring(p.IndexOf(".limit")) + "\"处解析错误!\r\n:datetime没有找到可用的参数"); }
                            entity.DateTimeParam[p4[1]] = 1;
                            if (p4[1].StartsWith(sign)) entity.AddParams(p4[1], entity.DateTimeParam);
                            #endregion
                        } else {
                            throw new Exception("SQL在\"" + p.Substring(p.IndexOf("." + p3)) + "\"处解析错误!\r\n:没有找到可解析的操作指令");
                        }
                    }
                    return false;
                }).Count();
            } catch (Exception ex) {
                throw new Exception("SQL解析错误!\r\n:" + ex.ToString());
            }
            return entity;
        }
        private static ISqlParser instance;
        private readonly static object key = DateTime.Now;
        public static ISqlParser Instance() {
            if (instance == null) {
                lock (key) {
                    if (instance == null) {
                        instance = new LinqSqlParser();
                    }
                }
            }
            return instance;
        }

        public static Event.DynamicEventFunc AddParams = new Event.DynamicEventFunc(p => {
            IDictionary<string, object> ret2 = p[0] as IDictionary<string, object>;
            object value = p[1];
            QueryEntity entity = p[2] as QueryEntity;
            string sign = Convert.ToString(p[3]);
            //仅仅补充Level为1时的状态                    
            if (value is IDictionary<string, object>) {
                ((IDictionary<string, object>)value).Values.Where(f2 => {
                    if (Convert.ToString(f2).StartsWith(sign)) {
                        entity.AddParams(Convert.ToString(f2), value as IDictionary<string, object>);
                    }
                    return false;
                }).Count();
            } else if (value is IDictionary<string, object>[]) {
                //对这种2级操作符之间的嵌套不做处理
            } else {
                if (Convert.ToString(value).StartsWith(sign)) {
                    entity.AddParams(Convert.ToString(value), ret2);
                }
            }
        });
    }

    #region 逻辑表达式
    class OperationSession {
        public OperationSession() {
            Word = string.Empty;
            CompleteOperate = null;
            OperateStack = new Stack<ALogicOperate>();
        }
        private string word = null;
        private Regex regWord = new Regex("^[a-zA-Z_\\$]+[a-zA-Z0-9_\\$]+$");
        public string Word {
            get {
                try {
                    return word;
                } finally {
                    word = null;
                }
            }
            set {
                if (!string.IsNullOrEmpty(word) && !regWord.IsMatch(word)) throw new Exception("不能放入含非法字符的列名" + word);
                if (string.IsNullOrEmpty(word)) word = value;
                else throw new Exception("不能连续放入未处理字符");
            }
        }

        private ALogicOperate completeOperate = null;
        public ALogicOperate CompleteOperate {
            get {
                try {
                    return completeOperate;
                } finally {
                    completeOperate = null;
                }
            }
            set {
                if (completeOperate == null) completeOperate = value;
                else throw new Exception("不能连续放入已处理的Operator");
            }
        }
        public Stack<ALogicOperate> OperateStack { get; protected set; }
        public bool IsComplete { get { return string.IsNullOrEmpty(word) && OperateStack.Count() == 0 && completeOperate != null; } }
    }

    abstract class ALogicOperate {
        public string Ope { get; set; }
        public abstract bool LIncome(OperationSession session);
        public abstract bool RIncome(OperationSession session, string word);
        public abstract bool RIncome(OperationSession session, ALogicOperate ope);
        public abstract IDictionary<string, object> ToDictionary(QueryEntity entity, string sign);

        public int Level { get; protected set; }
    }

    /// <summary>
    /// 剩余的1级操作
    /// </summary>
    class O1Operate : ALogicOperate {
        public O1Operate() { Level = 1; }
        protected string Key { get; set; }
        protected string Value { get; set; }
        public override bool LIncome(OperationSession session) {
            this.Key = session.Word;
            if (string.IsNullOrEmpty(Key)) throw new Exception(this.Ope + "不能没有对应的列");
            session.OperateStack.Push(this);
            return true;
        }

        public override bool RIncome(OperationSession session, string word) {
            this.Value = word;
            if (string.IsNullOrEmpty(word)) throw new Exception(this.Ope + "不能没有对应的值");
            if (session.OperateStack.Count() > 0) {
                //向上查找未完成操作的递归操作
                var ope = session.OperateStack.Pop();
                if (ope.RIncome(session, this)) return true;
                else { throw new Exception(ope.Ope + "处理错误失败:" + word); }
            } else session.CompleteOperate = this;
            return true;
        }

        public override bool RIncome(OperationSession session, ALogicOperate ope) {
            throw new NotImplementedException("1级操作符不能处理逻辑表达式之间的关系");
        }

        public override IDictionary<string, object> ToDictionary(QueryEntity entity, string sign) {
            var dic = new Dictionary<string, object> { { Key, new Dictionary<string, object> { { this.Ope, Value } } } };
            return dic;
        }
    }

    /// <summary>
    /// 专门处理等式的
    /// </summary>
    class EqualOperate : O1Operate {
        public EqualOperate() : base() { this.Ope = "="; }
        public override IDictionary<string, object> ToDictionary(QueryEntity entity, string sign) {
            var dic = new Dictionary<string, object> { { Key, Value } };
            return dic;
        }
    }


    /// <summary>
    /// 剩余的2级操作 and or not ( )
    /// </summary>
    class O2Operate : ALogicOperate {
        public O2Operate() { Level = 2; }
        internal ALogicOperate Left { get; set; }
        internal ALogicOperate Right { get; set; }
        public override bool LIncome(OperationSession session) {
            this.Left = session.CompleteOperate;
            if (this.Left == null) throw new Exception(this.Ope + " 不能直接处理非逻辑表达式");
            session.OperateStack.Push(this);
            return true;
        }

        public override bool RIncome(OperationSession session, string word) {
            session.Word = word;
            session.OperateStack.Push(this);
            return true;
        }

        public override bool RIncome(OperationSession session, ALogicOperate ope) {
            this.Right = ope;
            if (this.Right == null) throw new Exception(this.Ope + " 不能直接处理非逻辑表达式");
            if (session.OperateStack.Count() > 0) {
                //向上查找未完成操作的递归操作
                var ope2 = session.OperateStack.Pop();
                if (ope2.RIncome(session, this)) return true;
                else { throw new Exception(ope2.Ope + "处理错误失败"); }
            } else session.CompleteOperate = this;
            return true;
        }

        /// <summary>
        /// 目前只能处理1,2级混杂 不能处理连续2级
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public override IDictionary<string, object> ToDictionary(QueryEntity entity, string sign) {
            var ret = new Dictionary<string, object>();
            var dicLeft = Left.ToDictionary(entity, sign);
            var dicRight = Right.ToDictionary(entity, sign);
            //当操作符一致时去壳可以并列于一个字典中
            if (Left.Ope.Equals(this.Ope)) { dicLeft = dicLeft.Values.First() as IDictionary<string, object>; }
            if (Right.Ope.Equals(this.Ope)) { dicRight = dicRight.Values.First() as IDictionary<string, object>; }

            //左侧原始值赋值并判断是否为真实
            for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> ienu = dicLeft.GetEnumerator(); ienu.MoveNext(); ) {
                ret.Add(ienu.Current.Key, ienu.Current.Value);
                if (Left.Level == 1) {
                    LinqSqlParser.AddParams.Invoke(ret, ienu.Current.Value, entity, sign);
                }
            }
            for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> ienu = dicRight.GetEnumerator(); ienu.MoveNext(); ) {
                if (ret.ContainsKey(ienu.Current.Key)) {
                    //仅在 存在操作符一致的情况 应处理为 or:[and:{},and:{},a,b,c] 字典在处理单纯的and是可以的，单纯的or也是可以的，但是处理and与or的层级就需要用数组了 PrePare需要判断数组 我们这里也需要判断数组
                    if (Right.Ope.Equals(ienu.Current.Key)) {
                        //证明双方都是操作符且与当前操作符不同注意转化成数组
                        ret.Clear();
                        ret.Add(this.Ope, new IDictionary<string, object>[] { dicLeft.Values.First() as IDictionary<string, object>, dicRight.Values.First() as IDictionary<string, object> });
                        return ret;
                    }
                    //保证原始状态
                    IDictionary<string, object> idic = ret[ienu.Current.Key] as IDictionary<string, object>;
                    if (idic == null) {
                        //可以认为只能是相等的情况下存在
                        idic = new Dictionary<string, object> { { "=", ret[ienu.Current.Key] } };
                        ret[ienu.Current.Key] = idic;
                        LinqSqlParser.AddParams.Invoke(idic, ret[ienu.Current.Key], entity, sign);
                    }
                    //转化失败 一定是等于,不做处理 可如果是or那么也需要处理所以需要处理
                    if (ienu.Current.Value is IDictionary<string, object>) {
                        IDictionary<string, object> idcR = ienu.Current.Value as IDictionary<string, object>;
                        for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> ienu2 = idcR.GetEnumerator(); ienu2.MoveNext(); ) {
                            //处理同样的数值对方是a<1 这里是 a>2这种情况
                            idic.Add(ienu2.Current.Key, ienu2.Current.Value);
                            LinqSqlParser.AddParams.Invoke(idic, ienu2.Current.Value, entity, sign);
                        }
                    } else {
                        //可以认为只有等于一种情况
                        idic["="] = ienu.Current.Value;
                        if (Convert.ToString(ienu.Current.Value).StartsWith(sign)) {
                            entity.AddParams(Convert.ToString(ienu.Current.Value), idic);
                        }
                    }
                    //如果包含目前仅仅是处理同名参数同时存在 >1 <=2这种情况
                } else {
                    //如果不包含那么直接复制
                    ret.Add(ienu.Current.Key, ienu.Current.Value);
                }
                if (Right.Level == 1) {
                    LinqSqlParser.AddParams.Invoke(ret, ienu.Current.Value, entity, sign);
                }
            }
            //保证一定不会有键值重复的可能
            return new Dictionary<string, object> { { Ope + ";" + Guid.NewGuid().ToString(), ret } };
        }
    }

    class NotOperate : O2Operate {
        public NotOperate() : base() { this.Ope = "not"; }
        public override bool LIncome(OperationSession session) {
            if (!string.IsNullOrEmpty(session.Word) || session.CompleteOperate != null)
                throw new Exception("not 处理错误,不允许存在前置的非2级表达式");
            session.OperateStack.Push(this);
            return true;
        }

        public override IDictionary<string, object> ToDictionary(QueryEntity entity, string sign) {
            var ret = new Dictionary<string, object>();
            var dicRight = Right.ToDictionary(entity, sign);
            //左侧原始值赋值并判断是否为真实
            for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> ienu = dicRight.GetEnumerator(); ienu.MoveNext(); ) {
                ret.Add(ienu.Current.Key, ienu.Current.Value);
                if (Right.Level == 1) {
                    LinqSqlParser.AddParams.Invoke(ret, ienu.Current.Value, entity, sign);
                }
            }
            return new Dictionary<string, object> { { Ope + ";" + Guid.NewGuid().ToString(), ret } };
        }
    }

    //特别处理and 当前面是已结束的or 时可能把自己放进去其右边
    class AndOperate : O2Operate {
        public AndOperate() : base() { this.Ope = "and"; }
        public override bool LIncome(OperationSession session) {
            this.Left = session.CompleteOperate;
            if (this.Left == null) throw new Exception(this.Ope + " 不能直接处理非逻辑表达式");
            if ("or".Equals(this.Left.Ope) && (this.Left as O2Operate).Right.Level == 1) {
                var orOpe = this.Left as O2Operate;
                this.Left = orOpe.Right;
                orOpe.Right = this;
                session.OperateStack.Push(orOpe);
                session.OperateStack.Push(this);
            } else session.OperateStack.Push(this);
            return true;
        }
    }

    class LCOperate : NotOperate {
        private ALogicOperate RealLogicOperate;
        public LCOperate() : base() { this.Ope = "("; }
        public override bool LIncome(OperationSession session) {
            if (!string.IsNullOrEmpty(session.Word) || session.CompleteOperate != null)
                throw new Exception("\"(\"处理错误,不允许存在前置的非2级表达式");
            session.OperateStack.Push(this);
            return true;
        }

        public override bool RIncome(OperationSession session, ALogicOperate ope) {
            session.CompleteOperate = ope;
            session.OperateStack.Push(this);
            return true;
        }

        public override bool RIncome(OperationSession session, string word) {
            if (")".Equals(word)) {
                this.RealLogicOperate = session.CompleteOperate;
                if (session.OperateStack.Count() > 0) {
                    var ope = session.OperateStack.Pop();
                    ope.RIncome(session, this);
                } else session.CompleteOperate = this;
            } else
                return base.RIncome(session, word);
            return true;
        }

        public override IDictionary<string, object> ToDictionary(QueryEntity entity, string sign) {
            //todo 应该把$加在ope前面
            //return new Dictionary<string, object> { { this.Ope, RealLogicOperate.ToDictionary(entity, sign) } };
            return RealLogicOperate.ToDictionary(entity, sign);
        }
    }
    #endregion
}
