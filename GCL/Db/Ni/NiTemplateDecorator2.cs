using GCL.IO.Config;
using MongoDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni {
    public class NiTempldateDecorator2 : NiTemplate {
        //关系型数据库DBFactory
        private IDataResource realResource;
        //指向缓存DBFactory
        protected IDataResource cacheResource;
        //参数集
        protected IDataParameters paras;
        //遍历ni文件(ni文件属性需始终复制)
        protected ConfigManager ma;
        private NiDataResult DataResult;
        private string sql;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paras">参数集</param>
        /// <param name="ma">所有ni文件</param>
        /// <param name="cacheresource">指向缓存DBFactory</param>
        /// <param name="realResource">指向SQLSERVER的DBFactory</param>
        public NiTempldateDecorator2(IDataResource cacheresource, IDataResource realResource, IDataParameters paras, ConfigManager ma)
            : base(realResource, paras, ma) {
            this.cacheResource = cacheresource;
            this.realResource = realResource;
        }

        /// <summary>
        /// ni文件中缓存是否支持此命令
        /// </summary>
        /// <param name="commandName">命令行Name</param>
        /// <returns>true:有缓存命令;false:没有缓存命令</returns>
        public bool IsCache(string commandName) {
            object obj = this.ma.GetConfig("Ni").GetValue(commandName + ".Cache");
            if (obj != null) return true;
            else return false;
        }
        /// <summary>
        /// ni文件中此命令是否可以放入缓存
        /// </summary>
        /// <param name="commandName">命令行Name</param>
        /// <returns>true:有;false:没有</returns>
        public bool IsSet(string commandName) {
            object obj = this.ma.GetConfig("Ni").GetValue(commandName + ".Delete");
            if (obj != null) return true;
            else return false;
        }
        /// <summary>
        /// ni文件中此命令是否可以放入缓存
        /// </summary>
        /// <param name="commandName">命令行Name</param>
        /// <returns>true:有;false:没有</returns>
        public bool IsDel(string commandName) {
            object obj = this.ma.GetConfig("Ni").GetValue(commandName + ".Set");
            if (obj != null) return true;
            else return false;
        }
        /// <summary>
        /// ni文件中此命令是否可以放入缓存
        /// </summary>
        /// <param name="commandName">命令行Name</param>
        /// <returns>true:有;false:没有</returns>
        public bool IsGet(string commandName) {
            object obj = this.ma.GetConfig("Ni").GetValue(commandName + ".Get");
            if (obj != null) return true;
            else return false;
        }
        /// <summary>
        /// 装饰器执行ExcuteQuery,判断是否是缓存获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>

        #region 勤俭节约闹革命不成
        /*
        protected override NiDataResult Excute(IDataCommand comm, string commandText, IDictionary idicValue)
        {
            if (this.ma.GetConfig("Ni").GetValue(commandText + ".Get") != null)
            {
                MongoDBDecorator cache = new MongoDBDecorator(this.cacheResource, this.paras, this.ma);
                cache.DataResult = cache.Excute(NIDATATABLECOMMAND, commandText + ".Get", idicValue);
                if (cache.DataResult.DataSet.Tables.Count > 0) return cache.DataResult;
            }
            bool b = true;
            //判断是否有缓存
            if (IsCache(commandText))
            {
                MongoDBDecorator cacheDec = new MongoDBDecorator(this.cacheResource, this.paras, this.ma);
                if (commandText.EndsWith(".Set"))
                {
                    cacheDec.Excute(NIDATATABLECOMMAND, commandText + ".DS", idicValue);
                    b = false;
                }
                else cacheDec.DataResult = cacheDec.Excute(NIDATATABLECOMMAND, commandText + ".Cache", idicValue);
                if (cacheDec.DataResult.DataSet.Tables.Count > 0 && !commandText.EndsWith(".Set"))
                {
                    return cacheDec.DataResult;
                }
                else
                {
                    if (b)
                    {
                        MongoDBDecorator relDec = new MongoDBDecorator(this.realResource, this.paras, this.ma);
                        NiDataResult temp = base.Excute(NIDATATABLECOMMAND, commandText, idicValue);
                        if (IsSet(commandText))
                        {
                            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
                            MongoDBDecorator dec = middler.GetObjectByAppName("Ni", "decorator") as MongoDBDecorator;
                            var r = dec.ExcuteQuery(commandText + ".Set", new Hashtable
                           {
                             {"ds",JsonHelper.SerializeToString(temp.DataSet)}
                           });
                        }
                        this.DataResult = temp;
                    }
                    return this.DataResult;
                }
            }
            else return base.Excute(NIDATATABLECOMMAND, commandText, idicValue);
        } 
         */
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region 调用核心模板方法的多态版本
        /// <summary>
        /// 按照调用顺序执行已经记录的命令
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="lstCom"></param>
        /// <param name="result"></param>
        protected override void Excute(IDataResource resource, LinkedList<object[]> lstCom, NiDataResult result) {
            string str = this.sql.Trim().ToLower();
            if (str.Contains(".select") || str.Contains(".update") || str.Contains(".insert") || str.Contains(".delete") || str.Contains(".where")) {
                base.Excute(this.cacheResource, lstCom, result);
            } else {
                base.Excute(this.realResource, lstCom, result);
            }

        }
        bool b = true;
        //存放参数hashcode
        Dictionary<string, int> dicHs = new Dictionary<string, int>();
        //事务时存放命令行
        List<string> DelCom = new List<string>();
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        protected override NiDataResult Excute(IDataCommand comm, string commandText, IDictionary idicValue) {
            //构建命令参数的哈希
            ParameterCommand commandhs = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (commandhs != null && !dicHs.ContainsKey(commandText) && !commandText.EndsWith(".Cache") && !commandText.EndsWith(".Set") && !commandText.EndsWith(".Get") && !commandText.EndsWith(".Delete")) {
                StringBuilder sh = new StringBuilder();
                for (int w = 0; w < (commandhs.Parameters != null ? commandhs.Parameters.Length : 0); w++) {
                    sh.AppendFormat("{0}-", commandhs.Parameters[w].ParameterName);
                }
                string s = sh.Remove(sh.Length - 1, 1).ToString().TrimStart('@');
                dicHs.Add(commandText, s.GetHashCode());
            } else
                throw new InvalidOperationException("没有在Ni文件中找到该定义:" + commandText);
            //判断是否是事务
            if (base.Transaction) {
                //判断是否有删除缓存命令
                if (IsDel(commandText) && !DelCom.Contains(commandText) && IsGet(commandText)) {
                    DelCom.Add(commandText);
                }
            }
            //判断是否是Get
            if (IsGet(commandText) && b && !base.Transaction) {
                NiDataResult niRes = GetMethod(commandText);
                if (niRes.DataSet.Tables.Count > 0) {
                    this.DataResult.DataSet.Merge(DBTool.Serializer.Deserialize<DataSet>(niRes.DataSet.Tables[0].Rows[0][0].ToString()));
                    return this.DataResult;
                }
            }
            //判断是否是缓存
            if (IsCache(commandText) && b && !base.Transaction) commandText = commandText + ".Cache";

            //构建命令行SQL等
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (command != null) {
                IDbDataParameter[] dbps;
                if (commandText.EndsWith(".Cache") || commandText.EndsWith(".Get") || commandText.EndsWith(".Set")) dbps = this.paras.GetParas(commandText, this.cacheResource, command.Parameters, idicValue);
                else dbps = this.paras.GetParas(commandText, this.realResource, command.Parameters, idicValue);
                StringBuilder sb = new StringBuilder(command.CommandText);
                for (int w = 0; w < (command.Parameters != null ? command.Parameters.Length : 0); w++) {
                    if (command.Parameters[w].DBTypeName.Equals("Param", StringComparison.CurrentCultureIgnoreCase)) sb.Replace("{" + command.Parameters[w].ParameterName + "}", dbps[w].Value.ToString());
                }
                this.sql = sb.ToString();
                AddDataCommand(comm, sb.ToString(), command.CommandType, dbps);
                sb.Remove(0, sb.Length);
                sb = null;
            } else
                throw new InvalidOperationException("没有在Ni文件中找到该定义:" + commandText);
            NiDataResult ni = Excute();
            if (!commandText.EndsWith(".Set") && !commandText.EndsWith(".Cache") && !commandText.EndsWith(".Get") && ni.DataSet.Tables.Count > 0 && !base.Transaction) {
                if (DelCom.Contains(commandText)) DelMethod(commandText);
                SetMethod(commandText, ni);
            }
            if (commandText.EndsWith(".Cache") && ni.DataSet.Tables.Count <= 0 && !base.Transaction) {
                b = false;
                this.Excute(comm, commandText.Replace(".Cache", ""), idicValue);
            }
            return ni;
        }
        #endregion

        /// <summary>
        /// Cache的Get方法
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private NiDataResult GetMethod(string commandText) {
            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
            var dec = middler.GetObjectByAppName<NiTemplate>("Ni", "mongo");
            NiDataResult niRes = dec.ExcuteQuery(commandText + ".Get", new Hashtable
                                       {
                                         {"hs",dicHs[commandText]}
                                       });
            return niRes;
        }
        /// <summary>
        /// Cache的删除方法
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private NiDataResult DelMethod(string commandText) {
            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
            var dec = middler.GetObjectByAppName<NiTemplate>("Ni", "mongo");
            NiDataResult niRes = dec.ExcuteQuery(commandText + ".Delete", new Hashtable
                                       {
                                         {"hs",dicHs[commandText]}
                                       });
            return niRes;
        }
        /// <summary>
        /// Cache的加入方法
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="res"></param>
        private void SetMethod(string commandText, NiDataResult res) {
            var middler = new GCL.Bean.Middler.Middler(GCL.IO.Config.ConfigManagerFactory.GetApplicationConfigManager());
            var tempp = middler.GetObjectByAppName<NiTemplate>("Ni", "mongo");
            string ds = DBTool.Serializer.Serialize(res.DataSet);
            if (IsSet(commandText)) {
                tempp.ExcuteQuery(commandText + ".Set", new Hashtable
                                           {
                                             {"ds",ds},
                                             {"hs",dicHs[commandText]}
                                           });
            }
        }

    }

}
