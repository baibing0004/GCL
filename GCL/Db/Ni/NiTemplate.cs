using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using GCL.IO.Config;
namespace GCL.Db.Ni {
    /// <summary>
    /// 泥模板，是整个包的核心。
    /// </summary>
    public class NiTemplate : IDisposable {
        protected IDataResource res;

        /// <summary>
        /// 资源
        /// </summary>
        public IDataResource Resource {
            get { return res; }
        }

        protected IDataParameters paras;
        protected NiDataResult result = new NiDataResult();

        /// <summary>
        /// 结果集，请注意在未操作时，结果集中没有数据
        /// </summary>
        public NiDataResult Result {
            get { return result; }
        }
        protected LinkedList<object[]> lstCommand = new LinkedList<object[]>();

        protected ConfigManager ma;

        /// <summary>
        /// 泥模板构造函数 需要加入数据资源类，数据参数解析类，配置管理类
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="paras"></param>
        /// <param name="ma"></param>
        public NiTemplate(IDataResource resource, IDataParameters paras, ConfigManager ma) {
            this.res = resource;
            if (res == null) throw new InvalidOperationException("数据源不能为空");
            this.paras = paras;
            if (paras == null) throw new InvalidOperationException("参数解析对象不能为空");
            this.ma = ma;
            if (this.ma != null) this.ma.ConfigManagerFillEvent += new Event.EventHandle(ma_ConfigManagerFillEvent);
        }

        /// <summary>
        /// 特别处理当配置文件发生变化时自动更新引用。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ma_ConfigManagerFillEvent(object sender, Event.EventArg e) {
            lock (this) {
                this.ma = sender as ConfigManager;
            }
        }

        /// <summary>
        /// 泥模板构造函数 需要加入数据资源类，数据参数解析类 默认没有配置管理类
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="paras"></param>
        public NiTemplate(IDataResource resource, IDataParameters paras) {
            this.res = resource;
            this.paras = paras;
        }

        #region Command类型常量

        /// <summary>
        /// 查询方法
        /// </summary>
        protected readonly static NiQueryDataCommand NIQUERYCOMMAND = new NiQueryDataCommand();

        /// <summary>
        /// 无查询方法
        /// </summary>
        protected readonly static NiNonQueryDataCommand NINONQUERYCOMMAND = new NiNonQueryDataCommand();

        /// <summary>
        /// Reader方法
        /// </summary>
        protected readonly static NiReaderDataCommand NIREADERCOMMAND = new NiReaderDataCommand();

        /// <summary>
        /// FillData方法
        /// </summary>
        protected readonly static NiFillDataCommand NIDATATABLECOMMAND = new NiFillDataCommand();

        #endregion

        protected bool isTransaction = false;
        /// <summary>
        /// 是否是事务性的
        /// </summary>
        public bool Transaction {
            get { return this.isTransaction || this.nextTemplate != null || this.priTemplate != null; }
            set { this.isTransaction = value; }
        }

        protected NiTemplate nextTemplate, priTemplate;
        /// <summary>
        /// 请注意调用这个方法可以解决不同template之间的跨库调用问题。但是必须从队列首执行。
        /// </summary>
        /// <param name="temp"></param>
        public void Next(NiTemplate temp) {
            //将原有的priTemplate关系去掉。
            if (this.nextTemplate != null)
                this.nextTemplate.priTemplate = null;
            this.nextTemplate = temp;
            //此设置用于完成双向链结构
            if (temp != null)
                temp.priTemplate = this;
        }

        #region 核心模板方法 可以用来扩展的方法

        /// <summary>
        /// 严格按照调用顺序记录调用的命令
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="lstCom"></param>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        protected virtual void AddDataCommand(IDataResource resource, LinkedList<object[]> lstCom, IDataCommand comm, string commandText, CommandType type, params IDbDataParameter[] paras) {
            lstCom.AddLast(new object[] { comm, commandText, type, paras });
        }



        /// <summary>
        /// 按照调用顺序执行已经记录的命令
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="lstCom"></param>
        /// <param name="result"></param>
        protected virtual void Excute(IDataResource resource, LinkedList<object[]> lstCom, NiDataResult result) {
            IDbConnection conn = resource.GetConnection();
            IDbTransaction tran = null;
            try {
                result.Clear();
                using (IDbCommand com = resource.CreateCommand()) {
                    if (Transaction) {
                        tran = conn.BeginTransaction();
                        com.Transaction = tran;
                    }
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
                            (coms[0] as IDataCommand).ExcuteCommand(resource, com, result);
                        } catch (Exception ex) {
                            throw new NiCommandException(com, ex);
                        }
                    }
                    if (Transaction) {
                        tran.Commit();
                        this.lstCommand.Clear();
                    } else this.lstCommand.Clear();
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

        #endregion

        //todo 还没有加上自动调用队首的方法 判断
        public NiDataResult Commit() {
            if (this.priTemplate != null)
                return this.priTemplate.Commit();
            else
                return this._Commit();
        }
        /// <summary>
        /// 事务方法提交
        /// </summary>
        /// <returns></returns>
        internal virtual NiDataResult _Commit() {
            if (Transaction) {
                this.Excute(this.res, this.lstCommand, this.result);
                this.lstCommand.Clear();
                return result;
            } else
                throw new InvalidOperationException("不处于事务状态，不能使用本方法!");
        }

        #region 调用核心模板方法的多态版本

        /// <summary>
        /// 记录调用的命令的本地方式
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        protected void AddDataCommand(IDataCommand comm, string commandText, CommandType type, IDbDataParameter[] paras) {
            AddDataCommand(this.res, this.lstCommand, comm, commandText, type, paras);
        }

        /// <summary>
        /// 执行已经记录的命令的本地方式
        /// </summary>
        /// <returns></returns>
        protected virtual NiDataResult Excute() {
            if (!Transaction) {
                Excute(this.res, this.lstCommand, this.result);
                this.lstCommand.Clear();
            }
            return result;
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            AddDataCommand(comm, commandText, type, this.paras.GetParas(commandText, this.res, paras, idicValue));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            AddDataCommand(comm, commandText, type, this.paras.GetParas(commandText, this.res, paras, entity));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, CommandType type, object entity) {
            AddDataCommand(comm, commandText, type, this.paras.GetParas(commandText, this.res, entity));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, CommandType type) {
            AddDataCommand(comm, commandText, type, null);
            return Excute();
        }

        /// <summary>
        /// 根据命令判断是否属于调用存储过程！
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected virtual bool IsStoredProcedure(string commandText) {
            commandText = commandText.Trim();
            return commandText.IndexOf(" ") < 0 || (commandText.Split(' ').Length <= 2 && commandText.StartsWith("exec", true, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected virtual CommandType GetCommandType(string commandText) {
            return IsStoredProcedure(commandText) ? CommandType.StoredProcedure : CommandType.Text;
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            AddDataCommand(comm, commandText, GetCommandType(commandText), this.paras.GetParas(commandText, this.res, paras, idicValue));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, ParameterEntity[] paras, object entity) {
            AddDataCommand(comm, commandText, GetCommandType(commandText), this.paras.GetParas(commandText, this.res, paras, entity));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, object entity) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            IDbDataParameter[] dbps = this.paras.GetParas(commandText, this.res, entity);
            if (command != null) {
                StringBuilder sb = new StringBuilder(command.CommandText);
                for (int w = 0; w < (command.Parameters != null ? command.Parameters.Length : 0); w++)
                    //定义自定义SQL功能
                    if (command.Parameters[w].DBTypeName.Equals("Param", StringComparison.CurrentCultureIgnoreCase))
                        sb.Replace("{" + command.Parameters[w].ParameterName + "}", dbps[w].Value.ToString());
                AddDataCommand(comm, sb.ToString(), command.CommandType, dbps);
                sb.Remove(0, sb.Length);
                sb = null;
            } else
                AddDataCommand(comm, commandText, GetCommandType(commandText), dbps);
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText, IDictionary idicValue) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (command != null) {
                var res = this.res;
                IDbDataParameter[] dbps = this.paras.GetParas(commandText, this.res, command.Parameters, idicValue);
                StringBuilder sb = new StringBuilder(command.CommandText);
                for (int w = 0; w < (command.Parameters != null ? command.Parameters.Length : 0); w++)
                    if (command.Parameters[w].DBTypeName.Equals("Param", StringComparison.CurrentCultureIgnoreCase))
                        sb.Replace("{" + command.Parameters[w].ParameterName + "}", dbps[w].Value.ToString());
                AddDataCommand(comm, sb.ToString(), command.CommandType, dbps);
                sb.Remove(0, sb.Length);
                sb = null;
            } else
                throw new InvalidOperationException("没有在Ni文件中找到该定义:" + commandText);
            //AddDataCommand(comm, commandText, GetCommandType(commandText), this.paras.GetParas(commandText, this.res, idicValue));
            return Excute();
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected virtual NiDataResult Excute(IDataCommand comm, string commandText) {
            //需要加入自定义配置文件的设置
            ParameterCommand command = ma == null ? null : ma.GetConfigValue("Ni", commandText) as ParameterCommand;
            if (command != null)
                return Excute(comm, command.CommandText, command.CommandType);

            AddDataCommand(comm, commandText, GetCommandType(commandText), null);
            return Excute();
        }

        #endregion

        #region ExcuteScalar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIQUERYCOMMAND, commandText, type, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            return this.Excute(NIQUERYCOMMAND, commandText, type, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, CommandType type, object entity) {
            return this.Excute(NIQUERYCOMMAND, commandText, type, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, CommandType type) {
            return this.Excute(NIQUERYCOMMAND, commandText, type);
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIQUERYCOMMAND, commandText, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, ParameterEntity[] paras, object entity) {
            return this.Excute(NIQUERYCOMMAND, commandText, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, IDictionary idicValue) {
            return this.Excute(NIQUERYCOMMAND, commandText, idicValue);
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText, object entity) {
            return this.Excute(NIQUERYCOMMAND, commandText, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string commandText) {
            return this.Excute(NIQUERYCOMMAND, commandText);
        }

        #endregion

        #region ExcuteQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIDATATABLECOMMAND, commandText, type, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            return this.Excute(NIDATATABLECOMMAND, commandText, type, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, CommandType type, object entity) {
            return this.Excute(NIDATATABLECOMMAND, commandText, type, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, CommandType type) {
            return this.Excute(NIDATATABLECOMMAND, commandText, type);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIDATATABLECOMMAND, commandText, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, ParameterEntity[] paras, object entity) {
            return this.Excute(NIDATATABLECOMMAND, commandText, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, IDictionary idicValue) {
            return this.Excute(NIDATATABLECOMMAND, commandText, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText, object entity) {
            return this.Excute(NIDATATABLECOMMAND, commandText, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string commandText) {
            return this.Excute(NIDATATABLECOMMAND, commandText);
        }

        #endregion

        #region ExcuteNonQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NINONQUERYCOMMAND, commandText, type, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            return this.Excute(NINONQUERYCOMMAND, commandText, type, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, CommandType type, object entity) {
            return this.Excute(NINONQUERYCOMMAND, commandText, type, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, CommandType type) {
            return this.Excute(NINONQUERYCOMMAND, commandText, type);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NINONQUERYCOMMAND, commandText, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, ParameterEntity[] paras, object entity) {
            return this.Excute(NINONQUERYCOMMAND, commandText, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, IDictionary idicValue) {
            return this.Excute(NINONQUERYCOMMAND, commandText, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText, object entity) {
            return this.Excute(NINONQUERYCOMMAND, commandText, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string commandText) {
            return this.Excute(NINONQUERYCOMMAND, commandText);
        }

        #endregion

        #region ExcuteReader

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIREADERCOMMAND, commandText, type, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            return this.Excute(NIREADERCOMMAND, commandText, type, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, CommandType type, object entity) {
            return this.Excute(NIREADERCOMMAND, commandText, type, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, CommandType type) {
            return this.Excute(NIREADERCOMMAND, commandText, type);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            return this.Excute(NIREADERCOMMAND, commandText, paras, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, ParameterEntity[] paras, object entity) {
            return this.Excute(NIREADERCOMMAND, commandText, paras, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, IDictionary idicValue) {
            return this.Excute(NIREADERCOMMAND, commandText, idicValue);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText, object entity) {
            return this.Excute(NIREADERCOMMAND, commandText, entity);
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string commandText) {
            return this.Excute(NIREADERCOMMAND, commandText);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 首先关闭结果集，其次关闭命令组合，最后关闭资源
        /// </summary>
        public void Dispose() {
            //this.result.Dispose();
            this.lstCommand.Clear();
        }

        #endregion
    }
}
