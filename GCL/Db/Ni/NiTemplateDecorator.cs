using GCL.IO.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni {
    /// <summary>
    /// 通过获取和判断其缓存在执行过程中返回其缓存值或者直接返回真值
    /// 切记cache命令中的缓存输入值名字分别为 cacheKey,cacheValue
    /// </summary>
    public class NiTemplateDecorator : NiTemplate {
        protected IDataResource resCache;
        protected Dictionary<int, object[]> idcCache;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paras">参数集</param>
        /// <param name="ma">所有ni文件</param>
        /// <param name="cacheresource">指向缓存DBFactory</param>
        /// <param name="realResource">指向SQLSERVER的DBFactory</param>
        public NiTemplateDecorator(IDataResource resCache, IDataResource resReal, IDataParameters paras, ConfigManager ma)
            : base(resReal, paras, ma) {
            this.resCache = resCache;
            this.idcCache = new Dictionary<int, object[]>();
        }

        /// <summary>
        /// 判断并记录缓存信息
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="value"></param>
        protected virtual void PrepareCacheCommand(IDataCommand comm, string commandText, object value, ParameterCommand command) {
            ParameterCommand cmdCache = ma.GetConfigValue("Ni", commandText + ".Cache") as ParameterCommand;
            cmdCache = cmdCache == null ? ma.GetConfigValue("Ni", commandText + ".Clear") as ParameterCommand : cmdCache;
            if (cmdCache != null) {
                //已经找到了缓存对象 应该设置进入一个操作列表并记录当前的缓存信息需要 todo Excute后将idcCache清空
                if (value != null) {
                    if (value is System.Collections.IDictionary) {
                        IDictionary idic = new Hashtable();
                        IO.IOTool.Transport(value as System.Collections.IDictionary, idic);
                        //防止Paras做緩存
                        this.idcCache.Add(this.lstCommand.Count(), new object[] { comm, commandText + ".Set", cmdCache, (ma.GetConfigValue("Ni", commandText + ".Set") as ParameterCommand), this.paras.GetParas(commandText + ".Set1", this.resCache, command.Parameters, idic) });
                    } else {
                        this.idcCache.Add(this.lstCommand.Count(), new object[] { comm, commandText + ".Set", cmdCache, (ma.GetConfigValue("Ni", commandText + ".Set") as ParameterCommand), this.paras.GetParas(commandText + ".Set1", this.resCache, command.Parameters, value) });
                    }
                } else
                    this.idcCache.Add(this.lstCommand.Count(), new object[] { comm, commandText + ".Set", cmdCache, ma.GetConfigValue("Ni", commandText + ".Set") as ParameterCommand, null });
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected override NiDataResult Excute(IDataCommand comm, string commandText) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (command != null) {
                //首先保证能找到对应的命令找不到对应命令的不缓存
                this.PrepareCacheCommand(comm, commandText, null, command);
            }
            return base.Excute(comm, commandText);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override NiDataResult Excute(IDataCommand comm, string commandText, object entity) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            //IDbDataParameter[] dbps = this.paras.GetParas(commandText, this.res, entity);
            if (command != null) {
                //首先保证能找到对应的命令找不到对应命令的不缓存
                this.PrepareCacheCommand(comm, commandText, entity, command);
            }
            return base.Excute(comm, commandText, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        protected override NiDataResult Excute(IDataCommand comm, string commandText, IDictionary idicValue) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (command != null) {
                //首先保证能找到对应的命令找不到对应命令的不缓存
                this.PrepareCacheCommand(comm, commandText, idicValue, command);
            }
            return base.Excute(comm, commandText, idicValue);
        }

        internal override NiDataResult _Commit() {
            if (Transaction) {
                this.Excute(this.res, this.lstCommand, this.result);
                this.lstCommand.Clear();
                this.idcCache.Clear();
                return result;
            } else
                throw new InvalidOperationException("不处于事务状态，不能使用本方法!");
        }

        protected override NiDataResult Excute() {
            if (!Transaction) {
                Excute(this.res, this.lstCommand, this.result);
                this.lstCommand.Clear();
                this.idcCache.Clear();
            }
            return result;
        }

        /// <summary>
        /// 按照调用顺序执行已经记录的命令
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="lstCom"></param>
        /// <param name="result"></param>
        protected override void Excute(IDataResource resource, LinkedList<object[]> lstCom, NiDataResult result) {
            IDbConnection conn = resource.GetConnection();
            IDbTransaction tran = null;
            try {
                result.Clear();
                using (IDbCommand com = resource.CreateCommand()) {
                    if (Transaction) {
                        tran = conn.BeginTransaction();
                        com.Transaction = tran;
                    }
                    int w = 0;
                    foreach (object[] coms in lstCom) {
                        com.Connection = conn;
                        //勤俭节约闹革命 就用一个command
                        com.Parameters.Clear();
                        com.CommandText = Convert.ToString(coms[1]);
                        com.CommandType = (CommandType)Convert.ToInt32(coms[2]);
                        IDbDataParameter[] paras = coms[3] as IDbDataParameter[];
                        if (paras != null)
                            foreach (IDbDataParameter para in paras) {
                                com.Parameters.Add(para);
                                if (para.Direction == ParameterDirection.Output)
                                    result.OutParameter[para.ParameterName] = para;
                            }
                        try {
                            if (this.idcCache.Count() > 0 && this.idcCache.ContainsKey(w)) {
                                //进入缓存处理时间
                                this.ExcuteCacheCommand(w, result, new GCL.Event.DynamicEventFunc(p => {
                                    var ret = p[0] as NiDataResult;
                                    (coms[0] as IDataCommand).ExcuteCommand(resource, com, ret);
                                }));
                            } else
                                (coms[0] as IDataCommand).ExcuteCommand(resource, com, result);
                        } catch (Exception ex) {
                            throw new NiCommandException(com, ex);
                        }
                        w++;
                    }
                    if (Transaction) {
                        tran.Commit();
                    }
                    if (this.nextTemplate != null)
                        this.nextTemplate._Commit();
                }
            } catch (Exception ex) {
                if (Transaction)
                    try {
                        tran.Rollback();
                    } catch {
                    }
                throw ex;
            } finally {
                if (conn.State == ConnectionState.Open)
                    resource.SetConnection(conn);
            }
        }

        protected virtual void ExcuteCacheCommand(int w, NiDataResult result, GCL.Event.DynamicEventFunc action) {
            if (this.idcCache.ContainsKey(w)) {
                var objs = this.idcCache[w];
                #region 勤俭节约闹革命第二曲

                #region 执行sql
                GCL.Event.DynamicEventFunc funcSQL = null;
                funcSQL = new GCL.Event.DynamicEventFunc(p => {
                    //funcSQL.Invoke(NiTemplate.NIQUERYCOMMAND, cmdCache, paras, ret);
                    ParameterCommand pcom = p[1] as ParameterCommand;
                    NiDataResult ret2 = p[3] as NiDataResult;
                    if (pcom == null) return;
                    IDbConnection conn = this.resCache.GetConnection();
                    //缓存不处理事务
                    //IDbTransaction tran = null;
                    try {
                        using (IDbCommand com2 = resCache.CreateCommand()) {
                            com2.Connection = conn;
                            //勤俭节约闹革命 就用一个command
                            com2.Parameters.Clear();

                            com2.CommandText = pcom.CommandText;
                            com2.CommandType = pcom.CommandType;
                            IDbDataParameter[] paras2 = p[2] as IDbDataParameter[];
                            if (paras2 != null)
                                foreach (IDbDataParameter para in paras2) {
                                    com2.Parameters.Add(para);
                                    if (para.Direction == ParameterDirection.Output)
                                        ret2.OutParameter[para.ParameterName] = para;
                                }
                            try {
                                (p[0] as IDataCommand).ExcuteCommand(resCache, com2, ret2);
                            } catch (Exception ex) {
                                throw new NiCommandException(com2, ex);
                            }
                        }
                    } finally {
                        if (conn.State == ConnectionState.Open)
                            resCache.SetConnection(conn);
                    }
                });
                #endregion

                NiDataResult ret = new NiDataResult();
                IDataCommand com = objs[0] as IDataCommand;
                string cacheKey = Convert.ToString(objs[1]);
                ParameterCommand cmdCache = objs[2] as ParameterCommand;
                ParameterCommand cmdSet = objs[3] as ParameterCommand;
                IDbDataParameter[] paras = objs[4] as IDbDataParameter[];

                #region 生成CacheKey
                StringBuilder sb = new StringBuilder();
                try {
                    List<IDbDataParameter> list = new List<IDbDataParameter>();
                    if (paras != null) {
                        list.AddRange(paras);
                        paras.Where(p => { sb.Append(p.Value).Append("_"); return false; }).Count();
                        cacheKey += DBTool.GetCRCHashCode(sb.ToString());
                    }
                    var param = this.resCache.CreateParameter();
                    param.ParameterName = this.resCache.ParamSign + "cacheKey";
                    param.DbType = DbType.String;
                    param.Value = cacheKey;
                    list.Add(param);
                    paras = list.ToArray();
                } finally {
                    sb.Clear();
                    sb = null;
                }
                #endregion

                //开始获取缓存
                funcSQL.Invoke(NiTemplate.NIDATATABLECOMMAND, cmdCache, paras, ret);
                int _ret = 0;
                string _value = ret.DataSet.Tables.Count > 0 && ret.DataSet.Tables[0].Columns.Contains("cacheValue") ? Convert.ToString(ret.GetCell("cacheValue")) : null;
                if (!string.IsNullOrEmpty(_value) && !int.TryParse(Convert.ToString(_value), out _ret)) {
                    //证明缓存数据有效
                    DataSet ds = DBTool.DeserializeToObject<DataSet>(Convert.ToString(_value));
                    result.DataSet.Merge(ds);
                    ret.Clear();
                } else {
                    //证明数据无效
                    ret.Clear();
                    action.Invoke(ret);
                    Result.DataSet.Merge(ret.DataSet);


                    if (cmdSet != null && ret.DataSet.Tables.Count > 0) {
                        DataTable[] dts = new DataTable[ret.DataSet.Tables.Count];
                        ret.DataSet.Tables.CopyTo(dts, 0);
                        if (dts.Any(f => f.Rows.Count > 0)) {
                            //准备记入缓存
                            var param = this.resCache.CreateParameter();
                            param.ParameterName = this.resCache.ParamSign + "cacheValue";
                            param.DbType = DbType.String;
                            param.Value = DBTool.SerializeToString(ret.DataSet);
                            ret.Clear();
                            paras = paras.Concat(new IDbDataParameter[] { param }).ToArray();
                            funcSQL.Invoke(NiTemplate.NINONQUERYCOMMAND, cmdSet, paras, ret);
                        }
                    }
                }
                #endregion
            } else action.Invoke(this.Result);
        }
    }
}
