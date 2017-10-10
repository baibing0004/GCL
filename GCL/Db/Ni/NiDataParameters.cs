using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
namespace GCL.Db.Ni
{
    public class NiDataParameters : IDataParameters
    {


        #region IDataParameters Members

        private static readonly Type NiDataParameterAttributeType = typeof(NiDataParameterAttribute);


        /// <summary>
        /// 根据对象是否有属性定义进行相关赋值和设置
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="res"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual IDbDataParameter[] GetParas(string cacheKey, IDataResource res, ParameterEntity[] paras, object entity)
        {

            IDictionary idic = new Hashtable();

            bool hasParameterAttributeType = false;

            #region 有属性定义
            foreach (PropertyInfo property in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public))
            {
                if (property.GetCustomAttributes(NiDataParameterAttributeType, true).Length > 0)
                {
                    hasParameterAttributeType = true;
                    ParameterEntity ent = (property.GetCustomAttributes(NiDataParameterAttributeType, true)[0] as NiDataParameterAttribute).Entity as ParameterEntity;
                    ent.ParameterName = string.IsNullOrEmpty(ent.ParameterName) ? "" : ((ent.ParameterName.StartsWith(res.ParamSign) ? "" : res.ParamSign) + ent.ParameterName);
                    idic[DBTool.GetValue(ent.ParameterName.Trim().ToLower(), res.ParamSign + property.Name.Trim().ToLower())] = property.GetValue(entity, null);
                }
            }
            if (hasParameterAttributeType)
                return GetParas(cacheKey, res, paras, idic);
            #endregion

            #region 无属性定义
            foreach (ParameterEntity ent in paras)
                idic[ent.ParameterName.Trim().ToLower()] = Bean.BeanTool.GetPropertyValueSP(entity, ent.ParameterName.TrimStart('@', '$'), null, null, GCL.Bean.BeanBandingFlags.Property | GCL.Bean.BeanBandingFlags.Field);
            return GetParas(cacheKey, res, paras, idic);
            #endregion
        }

        /// <summary>
        /// 根据小写参数 赋值并生成IDbDataParameter数组
        /// </summary>
        /// <param name="res"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public virtual IDbDataParameter[] GetParas(string cacheKey, IDataResource res, ParameterEntity[] paras, System.Collections.IDictionary idicValue)
        {
            //将cacheKey加上资源类型的标示进行参数缓存!
            cacheKey += (res.GetFacotryTypeName() + res.ParamSign);

            IDictionary _idic = new Hashtable();
            //进行参数key最小化 从而实现忽略参数大小写的工能
            foreach (object key in idicValue.Keys)
                _idic[(key.ToString().StartsWith(res.ParamSign) ? key.ToString().ToLower().Substring(res.ParamSign.Length) : key.ToString().ToLower())] = idicValue[key];
            idicValue = _idic;
            //有缓存
            if (IsAutoCacheParameters && DBTool.IsEnable(cacheKey) && parmCache.Contains(cacheKey))
            {
                IDbDataParameter[] dps = GetCachedParameters(cacheKey);
                char sign = !string.IsNullOrEmpty(res.ParamSign) && res.ParamSign.Length > 0 ? res.ParamSign[0] : ' ';
                foreach (IDbDataParameter dp in dps)
                {
                    dp.Value = DBTool.GetValue(idicValue[dp.ParameterName.Trim().ToLower().TrimStart(sign)], DBNull.Value);
                }
                return dps;
            }

            //无缓存
            ArrayList rets = new ArrayList();
            ArrayList cacheParas = new ArrayList();
            foreach (ParameterEntity entity in DBTool.IsEnable(paras) ? paras : new ParameterEntity[0])
            {
                IDbDataParameter dp = res.CreateParameter();
                entity.ParameterName = entity.ParameterName.StartsWith(res.ParamSign) ? entity.ParameterName.Substring(res.ParamSign.Length) : entity.ParameterName;
                if (entity.DBTypeName.Equals("Param", StringComparison.CurrentCultureIgnoreCase))
                {
                    //特别处理Param类型参数 默认采用defaultValue用,分隔的字符串组，由参数输入Index选择内容。当超出范围或者未定义defaultValue时，直接替换成输入的值。
                    string value = Convert.ToString(DBTool.GetValue(idicValue[entity.ParameterName.Trim().ToLower()], ""));
                    int vs = 0;
                    if (!DBNull.Value.Equals(entity.DefaultValue) && int.TryParse(value, out vs))
                    {
                        string[] dvalues = Convert.ToString(entity.DefaultValue).Split(',');
                        if (dvalues.Length > vs) value = dvalues[vs];
                    }
                    entity.FillDbParameter(dp, value, DbType.String, res.ParamSign);
                }
                else
                {
                    entity.FillDbParameter(dp, DBTool.GetValue(idicValue[entity.ParameterName.Trim().ToLower()], DBNull.Value), res.ParseType(entity.DBTypeName), res.ParamSign);
                }
                rets.Add(dp);
                if (IsAutoCacheParameters && DBTool.IsEnable(cacheKey) && dp is ICloneable)
                {
                    IDbDataParameter cdp = ((ICloneable)dp).Clone() as IDbDataParameter;
                    cdp.Value = DBNull.Value;
                    cacheParas.Add(cdp);
                }
            }

            if (IsAutoCacheParameters && DBTool.IsEnable(cacheKey) && cacheParas.Count > 0)
                CacheParameters(cacheKey, (IDbDataParameter[])cacheParas.ToArray(typeof(IDbDataParameter)));
            return (IDbDataParameter[])rets.ToArray(typeof(IDbDataParameter));
        }

        private static readonly Type ParameterEntityType = typeof(ParameterEntity);
        public virtual IDbDataParameter[] GetParas(string cacheKey, IDataResource res, object entity)
        {
            ArrayList list = new ArrayList();
            //LinkedList<ParameterEntity> list = new LinkedList<ParameterEntity>();
            IDictionary idic = new Hashtable();
            bool hasParameterAttributeType = false;

            #region 有属性定义
            foreach (PropertyInfo property in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public))
            {
                if (property.GetCustomAttributes(NiDataParameterAttributeType, true).Length > 0)
                {
                    hasParameterAttributeType = true;
                    ParameterEntity ent = (property.GetCustomAttributes(NiDataParameterAttributeType, true)[0] as NiDataParameterAttribute).Entity.Clone() as ParameterEntity;
                    ent.ParameterName = DBTool.GetValue(ent.ParameterName.Trim(), res.ParamSign + property.Name.Trim());
                    ent.ParameterName = (ent.ParameterName.StartsWith(res.ParamSign) ? "" : res.ParamSign) + ent.ParameterName;
                    ent.DBTypeName = DBTool.GetValue(ent.DBTypeName, property.PropertyType.Name);
                    list.Add(ent);
                    idic[ent.ParameterName.Trim().ToLower()] = property.GetValue(entity, null);
                }
            }
            #endregion

            if (!hasParameterAttributeType)
            {
                #region 无属性定义
                foreach (PropertyInfo property in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public))
                {
                    ParameterEntity ent = new ParameterEntity();
                    ent.ParameterName = res.ParamSign + property.Name;
                    ent.DBTypeName = property.PropertyType.Name;
                    list.Add(ent);
                    idic[ent.ParameterName.Trim().ToLower()] = property.GetValue(entity, null);
                }
                #endregion
            }

            return GetParas(cacheKey, res, (ParameterEntity[])list.ToArray(ParameterEntityType), idic);
        }


        private bool isAutoCacheParameters = true;
        /// <summary>
        /// 是否自动缓存Parameters 默认为真
        /// </summary>
        public bool IsAutoCacheParameters
        {
            get
            {
                return isAutoCacheParameters;
            }
            set
            {
                isAutoCacheParameters = value;
                if (!isAutoCacheParameters)
                    parmCache.Clear();
            }
        }

        #endregion

        #region 复制DbHelper内容


        protected readonly static IDictionary parmCache = new Hashtable();
        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of DbParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params IDbDataParameter[] cmdParms)
        {
            //if (cmdParms != null)
            //    ClearParameterValues(cmdParms);
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// 清除parameter的值
        /// </summary>
        /// <param name="cmdParms">IDbDataParameter数组</param>
        public static void ClearParameterValues(params IDbDataParameter[] cmdParms)
        {
            foreach (IDbDataParameter parameter in cmdParms)
            {
                parameter.Value = DBNull.Value;
            }
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached DbParamters array</returns>
        public static IDbDataParameter[] GetCachedParameters(string cacheKey)
        {
            IDbDataParameter[] cachedParms = (IDbDataParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            IDbDataParameter[] clonedParms = new IDbDataParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (IDbDataParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        #endregion
    }
}
