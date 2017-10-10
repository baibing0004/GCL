using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using GCL.IO.Config;
using GCL.Bean.Middler;

namespace GCL.Db.Ni {
    /// <summary>
    /// 用于快速执行管理NiTemplate对象的 取消对middler的管理
    /// </summary>
    public class NiTemplateManager {
        private Middler middler;
        private string app;
        public NiTemplateManager(ConfigManager ma, string app)
            : this(new Middler(ma), app) {
        }

        public NiTemplateManager(Middler middler, string app) {
            this.middler = middler;
            this.app = app;
        }

        #region ExcuteScalar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, CommandType type) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteScalar(string templateName, string commandText) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region ExcuteQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, CommandType type) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteQuery(string templateName, string commandText) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region ExcuteNonQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, CommandType type) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteNonQuery(string templateName, string commandText) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region ExcuteReader

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, CommandType type) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, paras, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, paras, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, idicValue);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText, object entity) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, entity);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public NiDataResult ExcuteReader(string templateName, string commandText) {
            NiTemplate template = this.middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText);
            } finally {
                this.middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region static  ExcuteScalar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, CommandType type) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, type);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteScalar(Middler middler,string app,string templateName, string commandText) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteScalar(commandText);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region static  ExcuteQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, CommandType type) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, type);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteQuery(Middler middler,string app,string templateName, string commandText) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteQuery(commandText);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region static  ExcuteNonQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, CommandType type) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, type);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteNonQuery(Middler middler,string app,string templateName, string commandText) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteNonQuery(commandText);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion

        #region static  ExcuteReader

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, CommandType type, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, CommandType type, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, CommandType type) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, type);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, paras, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paras"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, ParameterEntity[] paras, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, paras, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="idicValue"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, IDictionary idicValue) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, idicValue);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }
        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText, object entity) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText, entity);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static NiDataResult ExcuteReader(Middler middler,string app,string templateName, string commandText) {
            NiTemplate template = middler.GetObjectByAppName(app, templateName) as NiTemplate;
            try {
                return template.ExcuteReader(commandText);
            } finally {
                middler.SetObjectByAppName(app, templateName, template);
            }
        }

        #endregion
    }
}
