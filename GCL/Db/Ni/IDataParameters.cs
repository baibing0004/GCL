using System;
using System.Collections;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    /// <summary>
    /// 对象格式化接口 用于处理对象属性转化为SQLParameter
    /// </summary>
    public interface IDataParameters {
        /// <summary>
        /// 根据对象生成DBParameters
        /// </summary>
        /// <param name="res"></param>
        /// <param name="paras"></param>
        /// <param name="entity">如果属性附有NiDataParameterAttribute属性，那么可以认为是参数之一，其它无属性的不是参数，但是如果都无NiDataParameterAttribute属性定义那么认为该对象所有的可读属性都是参数</param>
        /// <returns></returns>
        IDbDataParameter[] GetParas(string cacheKey, IDataResource res, ParameterEntity[] paras, object entity);

        /// <summary>
        /// 根据对象生成DBParameters
        /// </summary>
        /// <param name="res"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue">Key为ParaEntity.ParameterName,值为对象属性值</param>
        /// <returns></returns>
        IDbDataParameter[] GetParas(string cacheKey, IDataResource res, ParameterEntity[] paras, IDictionary idicValue);

        /// <summary>
        /// 根据对象生成DBParameters
        /// </summary>
        /// <param name="res"></param>
        /// <param name="entity">如果属性附有NiDataParameterAttribute属性，那么可以认为是参数之一，其它无属性的不是参数，但是如果都无NiDataParameterAttribute属性定义那么认为该对象所有的可读属性都是参数，且属性名为参数名</param>
        /// <returns></returns>
        IDbDataParameter[] GetParas(string cacheKey, IDataResource res, object entity);

        /// <summary>
        /// 是否自动缓存Parameters
        /// </summary>
        bool IsAutoCacheParameters { get; set; }
    }
}
