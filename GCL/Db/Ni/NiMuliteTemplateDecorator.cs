using GCL.Bean.Middler;
using GCL.IO.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni
{
    /// <summary>
    /// 通过设置
    /// </summary>
    public class NiMuliteTemplateDecorator : NiTemplateDecorator
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paras">参数集</param>
        /// <param name="ma">所有ni文件</param>
        /// <param name="cacheresource">指向缓存DBFactory</param>
        /// <param name="realResource">指向SQLSERVER的DBFactory</param>
        public NiMuliteTemplateDecorator(IDataResource resCache, IDataResource resReal, IDataParameters paras, ConfigManager ma, Middler middler, string appName)
            : base(resCache, resReal, paras, ma)
        {
            this.niba = new NiTemplateManager(middler, DBTool.GetValue(appName, "Ni"));
            this.idcTemplate = new Dictionary<int, object>();
            this.InitFunction();
        }

        /// <summary>
        /// 需要一个ConfigManager对象
        /// </summary>
        /// <param name="cm"></param>
        public NiMuliteTemplateDecorator(IDataResource resCache, IDataResource resReal, IDataParameters paras, ConfigManager ma, ConfigManager self) : this(resCache, resReal, paras, ma, new Middler(self), null) { }


        protected virtual void InitFunction()
        {
            this.QueryFunction = new Event.DynamicFunc<NiDataResult>(objs =>
            {
                string commandName = Convert.ToString(objs[1]);
                string templateName = Convert.ToString(objs[2]);
                object value = objs[3];
                if (value != null)
                {
                    if (value is System.Collections.IDictionary)
                        return niba.ExcuteScalar(templateName, commandName, value as System.Collections.IDictionary);
                    else return niba.ExcuteScalar(templateName, commandName, value);
                }
                else
                {
                    return niba.ExcuteScalar(templateName, commandName);
                }
            });
            this.NoQueryFunction = new Event.DynamicFunc<NiDataResult>(objs =>
            {
                string commandName = Convert.ToString(objs[1]);
                string templateName = Convert.ToString(objs[2]);
                object value = objs[3];
                if (value != null)
                {
                    if (value is System.Collections.IDictionary)
                        return niba.ExcuteNonQuery(templateName, commandName, value as System.Collections.IDictionary);
                    else return niba.ExcuteNonQuery(templateName, commandName, value);
                }
                else
                {
                    return niba.ExcuteNonQuery(templateName, commandName);
                }
            });
            this.ReaderFunction = new Event.DynamicFunc<NiDataResult>(objs =>
            {
                string commandName = Convert.ToString(objs[1]);
                string templateName = Convert.ToString(objs[2]);
                object value = objs[3];
                if (value != null)
                {
                    if (value is System.Collections.IDictionary)
                        return niba.ExcuteReader(templateName, commandName, value as System.Collections.IDictionary);
                    else return niba.ExcuteReader(templateName, commandName, value);
                }
                else
                {
                    return niba.ExcuteReader(templateName, commandName);
                }
            });
            this.FillFunction = new Event.DynamicFunc<NiDataResult>(objs =>
            {
                string commandName = Convert.ToString(objs[1]);
                string templateName = Convert.ToString(objs[2]);
                object value = objs[3];
                if (value != null)
                {
                    if (value is System.Collections.IDictionary)
                        return niba.ExcuteQuery(templateName, commandName, value as System.Collections.IDictionary);
                    else return niba.ExcuteQuery(templateName, commandName, value);
                }
                else
                {
                    return niba.ExcuteQuery(templateName, commandName);
                }
            });
        }



        protected Dictionary<int, object> idcTemplate;
        protected NiTemplateManager niba;
        protected Event.DynamicFunc<NiDataResult> QueryFunction;
        protected Event.DynamicFunc<NiDataResult> NoQueryFunction;
        protected Event.DynamicFunc<NiDataResult> ReaderFunction;
        protected Event.DynamicFunc<NiDataResult> FillFunction;

        /// <summary>
        /// 判断并记录缓存信息
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        protected override void PrepareCacheCommand(IDataCommand comm, string commandText, object value, ParameterCommand command)
        {
            if (command != null && !string.IsNullOrEmpty(command.Template))
            {
                //已经找到了缓存对象 应该设置进入一个操作列表并记录当前的缓存信息需要 todo Excute后将idcCache清空     
                int w = 0;
                if (comm == NIDATATABLECOMMAND) w = 3;
                else if (comm == NINONQUERYCOMMAND) w = 1;
                else if (comm == NIQUERYCOMMAND) w = 0;
                else if (comm == NIREADERCOMMAND) w = 2;
                if (value != null)
                {
                    if (value is System.Collections.IDictionary)
                    {
                        IDictionary idic = new Hashtable();
                        IO.IOTool.Transport(value as System.Collections.IDictionary, idic);
                        //防止Paras做緩存
                        this.idcTemplate.Add(this.lstCommand.Count(), new object[] { w, commandText, command.Template, idic });
                    }
                    else
                    {
                        this.idcTemplate.Add(this.lstCommand.Count(), new object[] { w, commandText, command.Template, value });
                    }
                }
                else
                    this.idcTemplate.Add(this.lstCommand.Count(), new object[] { w, commandText, command.Template, null });
            }
            base.PrepareCacheCommand(comm, commandText, value, command);
        }



        internal override NiDataResult _Commit()
        {
            if (Transaction)
            {
                this.idcTemplate.Clear();
                return result;
            }
            else
                throw new InvalidOperationException("不处于事务状态，不能使用本方法!");
        }

        protected override NiDataResult Excute()
        {
            if (!Transaction)
            {
                Excute(this.res, this.lstCommand, this.result);
                this.idcTemplate.Clear();
            }
            return result;
        }

        /// <summary>
        /// 按照调用顺序执行已经记录的命令
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="lstCom"></param>
        /// <param name="result"></param>
        protected override void Excute(IDataResource resource, LinkedList<object[]> lstCom, NiDataResult result)
        {
            if (this.idcTemplate.Count() == 0) base.Excute(resource, lstCom, result);
            else
            {
                if (Transaction && lstCom.Count() != this.idcTemplate.Count()) throw new InvalidOperationException("多数据源装饰器不支持跨数据源事务!");
                //仅剩下lstCom与idcTemplate一致的情况，也就是说全是Template代理方式执行
                try
                {
                    result.Clear();
                    int w = 0;
                    lstCom.Where(p =>
                    {
                        object[] coms = p as object[];
                        string commandText = Convert.ToString(coms[1]);
                        if (this.idcCache.Count() > 0 && this.idcCache.ContainsKey(w))
                        {
                            //进入缓存处理时间
                            this.ExcuteCacheCommand(w, result, new GCL.Event.DynamicEventFunc(q =>
                            {
                                var ret = q[0] as NiDataResult;
                                if (this.idcTemplate.Count() > 0 && this.idcTemplate.ContainsKey(w))
                                {
                                    object[] tems = this.idcTemplate[w] as object[];
                                    NiDataResult ret2 = null;
                                    switch (Convert.ToInt32(tems[0]))
                                    {
                                        case 0:
                                            ret2 = this.QueryFunction.Invoke(tems);
                                            break;
                                        case 1:
                                            ret2 = this.NoQueryFunction.Invoke(tems);
                                            break;
                                        case 2:
                                            ret2 = this.ReaderFunction.Invoke(tems);
                                            break;
                                        case 3:
                                            ret2 = this.FillFunction.Invoke(tems);
                                            break;
                                    }

                                    #region 数据合并
                                    ret.DataSet.Merge(ret2.DataSet);
                                    if (ret.OutParameter.Count > 0)
                                    {
                                        ret.OutParameter.Where(f =>
                                        {
                                            if (ret2.OutParameter.ContainsKey(f.Key))
                                            {
                                                f.Value.Value = ret2.OutParameter[f.Key].Value;
                                            }
                                            return false;
                                        }).Count();
                                    }
                                    #endregion

                                }
                                else throw new Exception(string.Format("未找到Ni文件{0}中对应的template属性定义", commandText));
                            }));
                        }
                        else
                        {
                            object[] tems = this.idcTemplate[w] as object[];
                            NiDataResult ret2 = null;
                            switch (Convert.ToInt32(tems[0]))
                            {
                                case 0:
                                    ret2 = this.QueryFunction.Invoke(tems);
                                    break;
                                case 1:
                                    ret2 = this.NoQueryFunction.Invoke(tems);
                                    break;
                                case 2:
                                    ret2 = this.ReaderFunction.Invoke(tems);
                                    break;
                                case 3:
                                    ret2 = this.FillFunction.Invoke(tems);
                                    break;
                            }

                            #region 数据合并
                            result.DataSet.Merge(ret2.DataSet);
                            if (result.OutParameter.Count > 0)
                            {
                                result.OutParameter.Where(f =>
                                {
                                    if (ret2.OutParameter.ContainsKey(f.Key))
                                    {
                                        f.Value.Value = ret2.OutParameter[f.Key].Value;
                                    }
                                    return false;
                                }).Count();
                            }
                            #endregion
                        }

                        w++;
                        return false;
                    }).Count();
                    if (this.nextTemplate != null)
                        this.nextTemplate._Commit();

                }
                finally
                {
                }
            }
        }
    }
}
