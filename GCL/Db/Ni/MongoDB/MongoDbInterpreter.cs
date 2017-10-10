using MongoDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCL.Db.Ni.MongoDB
{
    public class MongoDbInterpreter
    {
        private string Expresstion;//String类型表达式
        public static Dictionary<string, Document> dicpars = new Dictionary<string, Document>();
        private System.Data.IDataParameterCollection paras;
        public MongoDbInterpreter(string expresstion, System.Data.IDataParameterCollection paras)
        {
            this.Expresstion = expresstion;
            this.paras = paras;
        }
        // 判断字符串是否非运算符
        public bool IsParaExp(string str)
        {
            bool ispara = false;
            byte c;
            if (str == null || str.Length == 0)
                return false;
            if (!str.Equals("=") && !str.Equals(">") && !str.Equals(">=") && !str.Equals("<") && !str.Equals("<=") && !str.Equals("<>") && !str.Equals("!=") && !str.Equals("and") && !str.Equals("or") && !str.Equals(")") && !str.Equals("("))
            {
                ispara = true;
            }
            return ispara;
        }

        // 基本一目计算
        public List<Dictionary<string, object>> account(object n1, object n2, string op)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            Dictionary<string, object> aresult = new Dictionary<string, object>();
            List<Document> ls = new List<Document>();
            foreach (MongoDbParameter item in this.paras)
            {
                if (item.ParameterName.Equals(n2.ToString()))
                {
                    n2 = item.Value;
                }
            }
            switch (op)
            {
                case "or":
                    List<Dictionary<string, object>> lt = new List<Dictionary<string, object>>();
                    lt.Add(n1 as Dictionary<string, object>);
                    if (n2 is Dictionary<string, object>)
                    {
                        lt.Add(n2 as Dictionary<string, object>);
                        foreach (Dictionary<string, object> item in lt)
                        {
                            foreach (KeyValuePair<string, object> it in item)
                            {
                                Document d = new Document();
                                d.Add(it.Key, it.Value);
                                ls.Add(d);
                            }
                        }
                        aresult.Add("$or", ls.ToArray());
                    }
                    if (n2 is Document)
                    {
                        foreach (KeyValuePair<string, object> it in n1 as Dictionary<string, object>)
                        {
                            Document d = new Document();
                            d.Add(it.Key, it.Value);
                            ls.Add(d);
                        }
                        ls.Add(n2 as Document);
                        aresult.Add("$or", ls.ToArray());
                    }

                    list.Add(aresult);
                    break;
                case "and":
                    list.Add(n1 as Dictionary<string, object>);
                    list.Add(n2 as Dictionary<string, object>);
                    break;
                case "!=":
                case "<>":
                    Document tc1 = new Document();
                    
                    tc1.Add("$ne", n2);
                    aresult.Add(n1.ToString(), tc1);
                    list.Add(aresult);
                    break;
                case "=":
                    aresult.Add(n1.ToString(), n2);
                    list.Add(aresult);
                    break;
                case "<=":
                    Document tc3 = new Document();
                    tc3.Add("$lte", n2);
                    aresult.Add(n1.ToString(), tc3);
                    list.Add(aresult);
                    break;
                case ">=":
                    Document tc4 = new Document();
                    tc4.Add("$gte", n2);
                    aresult.Add(n1.ToString(), tc4);
                    list.Add(aresult);
                    break;
                case ">":
                    Document tc5 = new Document();
                    tc5.Add("$gt", n2);
                    aresult.Add(n1.ToString(), tc5);
                    list.Add(aresult);
                    break;
                case "<":
                    Document tc6 = new Document();
                    tc6.Add("$lt", n2);
                    aresult.Add(n1.ToString(), tc6);
                    list.Add(aresult);
                    break;
            }
            return list;
        }

        // 将String类型表达式转为由操作数和运算符组成的ArrayList类型表达式
        public ArrayList Toexp_arraylist(string exp_str)
        {
            string exp_element = "", expchar, exp = string.Empty;
            ArrayList exp_arraylist = new ArrayList();

            string[] strTemp = exp_str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            strTemp.Where(p =>
            {

                if (!p.Equals("and") && !p.Equals("or") && !p.Equals(">=") && !p.Equals("<=") && !p.Equals("<>") && !p.Equals("!="))
                {
                    //遍历表达式
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (i < p.Length - 1) exp = p.Substring(i, 2);
                        if (!exp.Equals("<>") && !exp.Equals("!=") && !exp.Equals(">=") && !exp.Equals("<=") && !exp.Equals("<>"))
                        {
                            expchar = p.Substring(i, 1);
                            exp = string.Empty;
                            //如果该字符为数字,小数字或者负号(非运算符的减号）
                            if (char.IsNumber(p, i) || char.IsLetter(p, i) || expchar == "." || expchar == "@" || (expchar == "-" && (i == 0 || p.Substring(i - 1, 1) == "(")))
                            {
                                exp_element += expchar;//存为操作数
                            }
                            else//为运算符
                            {
                                //将操作数添加到ArrayList类型表达式
                                if (exp_element != "")
                                    exp_arraylist.Add(exp_element);
                                //将运算符添加到ArrayList类型表达式
                                exp_arraylist.Add(expchar);
                                exp_element = "";
                            }
                        }
                        else
                        {
                            //将操作数添加到ArrayList类型表达式
                            if (exp_element != "")
                                exp_arraylist.Add(exp_element);
                            exp_arraylist.Add(exp);
                            i++;
                            exp = string.Empty;
                            exp_element = "";
                        }
                    }
                }
                else
                {
                    //将操作数添加到ArrayList类型表达式
                    if (exp_element != "")
                        exp_arraylist.Add(exp_element);
                    //将运算符添加到ArrayList类型表达式
                    exp_arraylist.Add(p);
                    exp_element = "";
                }
                return false;
            }).Count();



            //如果还有操作数未添加到ArrayList类型表达式,则执行添加操作
            if (exp_element != "")
                exp_arraylist.Add(exp_element);
            return exp_arraylist;
        }

        //返回运算符的优先级
        private int Operatororder(string op)
        {
            switch (op)
            {
                case "and":
                    return 2;
                case "(":
                    return 4;
                case ")":
                    return 0;
                case "or":
                    return 1;

                case ">":
                case ">=":
                case "<":
                case "<=":
                case "=":
                case "!=":
                case "<>":
                    return 3;

                default:
                    return -1;
            }
        }

        private bool IsPop(string op, Stack operators)
        {
            if (operators.Count == 0)
            {
                return false;
            }
            else
            {
                if (operators.Peek().ToString() == "(" || Operatororder(op) > Operatororder(operators.Peek().ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //将ArrayList类型的中缀表达式转为ArrayList类型的后缀表达式
        public ArrayList Toexpback_arraylist(ArrayList exp)
        {
            ArrayList expback_arraylist = new ArrayList();
            Stack operators = new Stack();
            string op;
            //遍历ArrayList类型的中缀表达式
            foreach (string s in exp)
            {
                //若为数字则添加到ArrayList类型的后缀表达式
                if (IsParaExp(s))
                {
                    expback_arraylist.Add(s);
                }
                else
                {
                    switch (s)
                    {
                        //为运算符
                        case ">":
                        case ">=":
                        case "<":
                        case "<=":
                        case "=":
                        case "!=":
                        case "<>":
                        case "and":
                        case "or":
                            while (IsPop(s, operators))
                            {
                                expback_arraylist.Add(operators.Pop().ToString());
                            }
                            operators.Push(s);
                            break;
                        //为开括号
                        case "(":
                            operators.Push(s);
                            break;
                        //为闭括号
                        case ")":
                            while (operators.Count != 0)
                            {
                                op = operators.Pop().ToString();
                                if (op != "(")
                                {
                                    expback_arraylist.Add(op);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
            while (operators.Count != 0)
            {
                expback_arraylist.Add(operators.Pop().ToString());
            }
            return expback_arraylist;
        }

        //计算一个ArrayList类型的后缀表达式的值
        public Document ExpValue(ArrayList expback)
        {
            object num1;
            object num2;
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            Stack num = new Stack();
            foreach (string n in expback)
            {
                if (IsParaExp(n))
                {
                    num.Push(n);
                }
                else
                {
                    num2 = num.Pop();
                    num1 = num.Pop();
                    result = account(num1, num2, n);
                    if (!n.Equals("and") && !n.Equals("or"))
                    {
                        foreach (Dictionary<string, object> item in result)
                        {
                            num.Push(item);
                        }
                    }
                    else if (n.Equals("and"))
                    {
                        Document dc = new Document();
                        result.Where(p =>
                        {
                            p.Where(p1 =>
                            {
                                dc.Add(p1.Key, p1.Value);
                                return false;
                            }).Count();
                            return false;
                        }).Count();
                        num.Push(dc);
                    }
                }
            }

            Document d = new Document();
            foreach (Dictionary<string, object> item in result)
            {
                foreach (KeyValuePair<string, object> it in item)
                {
                    d.Add(it.Key, it.Value);

                }
            }
            return d;
        }

        //返回本类的表达式值
        public Document ExpValue()
        {
            ArrayList a1 = new ArrayList();
            ArrayList a2 = new ArrayList();
            a1 = Toexp_arraylist(Expresstion);
            a2 = Toexpback_arraylist(a1);
            return ExpValue(a2);
        }
    }
}
