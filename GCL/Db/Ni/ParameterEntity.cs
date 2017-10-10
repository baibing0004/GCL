using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace GCL.Db.Ni {
    /// <summary>
    /// 从文件中获取的参数对象
    /// </summary>
    public class ParameterEntity : ICloneable, IDisposable {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// DB类型名
        /// </summary>
        public string DBTypeName { get; set; }

        /// <summary>
        /// DB类型名
        /// </summary>
        public DbType DBType { get; set; }

        /// <summary>
        /// 参数方向
        /// </summary>
        public ParameterDirection ParameterDirection { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value {
            set {
                if (!Nullable && (value == null && value == DBNull.Value) && (DefaultValue == null && DefaultValue == DBNull.Value))
                    throw new InvalidOperationException("不允许填写空对象");
            }
        }

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        private object defaultValue = DBNull.Value;
        /// <summary>
        /// 默认参数值
        /// </summary>
        public object DefaultValue {
            get {
                return defaultValue;
            }
            set {
                defaultValue = (value == null) ? DBNull.Value : value;
            }
        }

        /// <summary>
        /// 参数限制值
        /// </summary>
        public int Size { get; set; }
        public ParameterEntity() {
            DBType = DbType.String;
            ParameterDirection = ParameterDirection.Input;
            Nullable = false;
            Value = DBNull.Value;
            Size = 0;
        }

        /// <summary>
        /// 使用自身属性填充DbParameter对象
        /// </summary>
        /// <param name="para"></param>
        public void FillDbParameter(IDbDataParameter para, object value, DbType dbtype, string paramSign) {
            para.ParameterName = this.ParameterName.StartsWith(paramSign) ? this.ParameterName : (paramSign + this.ParameterName);
            para.Direction = this.ParameterDirection;
            para.DbType = dbtype;
            //这里只进行校验
            this.Value = value;
            para.Value = (value == null || value == DBNull.Value) ? this.DefaultValue : value;
            if (Size > 0)
                para.Size = Size;
        }

        #region ICloneable Members

        public object Clone() {
            return MemberwiseClone();
            //ParameterEntity entity = new ParameterEntity();
            //Bean.BeanTool.Transport(this, entity);
            //return entity;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Bean.BeanTool.Close(this.DefaultValue);
        }

        #endregion
    }
}
