using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Db.Ni {
    /// <summary>
    /// Ni框架属性 可在对象上自定义通用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class NiDataParameterAttribute : Attribute {
        public ParameterEntity Entity { get; set; }
        public NiDataParameterAttribute() {
            Entity = new ParameterEntity();
            Entity.ParameterName = "";
        }

        /// <summary>
        /// 参数名属性
        /// </summary>
        /// <param name="name"></param>
        public NiDataParameterAttribute(string name)
            : this() {
            //if (name.StartsWith(@"@"))
                Entity.ParameterName = name;
            //else
            //    throw new InvalidOperationException(@"参数名需要以@开头！");
        }

        /// <summary>
        /// 参数名，DbType
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public NiDataParameterAttribute(string name, string dbType)
            : this(name) {
            Entity.DBTypeName = dbType;
        }

        /// <summary>
        /// 参数名，nullAble
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nullAble"></param>
        public NiDataParameterAttribute(string name, bool nullAble)
            : this(name) {
            Entity.Nullable = nullAble;
        }

        /// <summary>
        /// 参数名，direction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public NiDataParameterAttribute(string name, System.Data.ParameterDirection direction)
            : this(name) {
            Entity.ParameterDirection = direction;
        }

        /// <summary>
        /// 参数名，DbType，是否允许为空
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="nullAble"></param>
        public NiDataParameterAttribute(string name, string dbType, bool nullAble)
            : this(name, dbType) {
            Entity.Nullable = nullAble;
        }

        /// <summary>
        /// 参数名，DbType，默认值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="defaultValue"></param>
        public NiDataParameterAttribute(string name, string dbType, object defaultValue)
            : this(name, dbType) {
            Entity.DefaultValue = defaultValue;
        }




        /// <summary>
        /// 参数名，DbType，参数方向
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        public NiDataParameterAttribute(string name, string dbType, System.Data.ParameterDirection direction)
            : this(name, dbType) {
            Entity.ParameterDirection = direction;
        }

        /// <summary>
        /// 参数名，DbType，限制大小
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        public NiDataParameterAttribute(string name, string dbType, int size)
            : this(name, dbType) {
            Entity.Size = size;
        }
        /// <summary>
        /// 参数名，DbType，限制大小，参数方向
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public NiDataParameterAttribute(string name, string dbType, int size, System.Data.ParameterDirection direction)
            : this(name, dbType, size) {
            Entity.ParameterDirection = direction;
        }

        /// <summary>
        /// 参数名，DbType，限制大小，是否允许为空
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="nullAble"></param>
        public NiDataParameterAttribute(string name, string dbType, int size, bool nullAble)
            : this(name, dbType, size) {
            Entity.Nullable = nullAble;
        }

        /// <summary>
        /// 参数名，DbType，限制大小，默认值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="defaultValue"></param>
        public NiDataParameterAttribute(string name, string dbType, int size, object defaultValue)
            : this(name, dbType, size) {
            Entity.DefaultValue = defaultValue;
        }

        /// <summary>
        /// 参数名，DbType，限制大小，默认值,是否允许为空,参数方向
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="defaultValue"></param>
        public NiDataParameterAttribute(string name, string dbType, int size, object defaultValue, bool nullAble, System.Data.ParameterDirection direction)
            : this(name, dbType, size, defaultValue) {
            Entity.Nullable = nullAble;
            Entity.ParameterDirection = direction;
        }
    }
}
