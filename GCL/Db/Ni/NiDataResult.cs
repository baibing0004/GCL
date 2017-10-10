using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
namespace GCL.Db.Ni {

    /// <summary>
    /// 数据集
    /// </summary>
    public class NiDataResult : IDisposable {
        private DataSet ds = new DataSet();
        /// <summary>
        /// 数据结果
        /// </summary>
        public DataSet DataSet {
            get {
                return ds;
            }
        }


        private IDictionary<string, IDataParameter> idic;
        /// <summary>
        /// Key应该是对象全路径名.属性（参数属性前面有@的）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDictionary<string, IDataParameter> OutParameter {
            get {
                if (idic == null) idic = new Dictionary<string, IDataParameter>();
                return idic;
            }
        }



        /// <summary>
        /// Reader事件 
        /// </summary>
        public event Event.EventHandle ReaderEvent;

        /// <summary>
        /// 调用Reader方法 触发处理事件！
        /// </summary>
        /// <param name="reader"></param>
        public void DoReader(IDataReader reader) {
            try {
                while (!reader.IsClosed && reader.Read()) {
                    Event.EventArg.CallEventSafely(ReaderEvent, this, new Event.EventArg(reader));
                }
            } finally {
                try {
                    reader.Close();
                } catch {
                }
            }
        }



        /// <summary>
        /// 获得制定表/列的数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public object GetCell(int table, int col) {
            try {
                return ds.Tables[table].Rows[0][col];
            } catch (IndexOutOfRangeException) {
                return null;
            }
        }

        /// <summary>
        /// 获得制定表/列的数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public object GetCell(string table, string col) {
            try {
                return ds.Tables[table].Rows[0][col];
            } catch (IndexOutOfRangeException) {
                return null;
            }
        }

        /// <summary>
        /// 获得第一张表/列的数据
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public object GetCell(int col) {
            return GetCell(0, col);
        }

        /// <summary>
        /// 获得第一张表/列的数据
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public object GetCell(string col) {
            try {
                return ds.Tables[0].Rows[0][col];
            } catch (IndexOutOfRangeException) {
                return null;
            }
        }

        /// <summary>
        /// 获得制定表第一列的数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public object GetFirstCell(int table) {
            return GetCell(table, 0);
        }

        /// <summary>
        /// 获得制定表第一列的数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public object GetFirstCell(string table) {
            try {
                return ds.Tables[table].Rows[0][0];
            } catch (IndexOutOfRangeException) {
                return null;
            }
        }

        /// <summary>
        /// 获得第一张表第一列的数据
        /// </summary>
        /// <returns></returns>
        public object GetFirstCell() {
            return GetCell(0, 0);
        }

        /// <summary>
        /// 获取第一张表的第一列的数据默认为0
        /// </summary>
        /// <returns></returns>
        public int GetFirstInt32() { return Convert.ToInt32(GetFirstCell()); }

        /// <summary>
        /// 调用其自身方法填充对象 可以使用BeanTool.Transport方法实现这个功能
        /// </summary>
        /// <param name="o"></param>
        /// <param name="table"></param>
        /// <param name="row"></param>
        public void Fill(IRowToObject o, int table, int row) {
            o.Fill(ds.Tables[table].Rows[row]);
        }

        /// <summary>
        /// 调用其自身方法填充对象
        /// </summary>
        /// <param name="o"></param>
        /// <param name="table"></param>
        /// <param name="row"></param>
        public void Fill(IRowToObject o, string table, int row) {
            o.Fill(ds.Tables[table].Rows[row]);
        }

        private static readonly Type IRowToObjectType = typeof(IRowToObject);

        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public object[] Fill(Type t, DataTable dt) {
            try {
                if (t.GetInterface(IRowToObjectType.FullName) == null) {
                    IDictionary<string, ParameterEntity> idic = new Dictionary<string, ParameterEntity>();
                    IDictionary<string, PropertyInfo> idic2 = new Dictionary<string, PropertyInfo>();
                    bool hasParameterAttributeType = false;
                    #region 有属性定义
                    foreach (PropertyInfo property in t.GetProperties(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public)) {
                        if (property.GetCustomAttributes(typeof(NiDataParameterAttribute), true).Length > 0) {
                            hasParameterAttributeType = true;
                            ParameterEntity ent = (property.GetCustomAttributes(typeof(NiDataParameterAttribute), true)[0] as NiDataParameterAttribute).Entity.Clone() as ParameterEntity;
                            ent.ParameterName = DBTool.GetValue(ent.ParameterName.Trim(), property.Name.Trim()).TrimStart('$', '&', '@');
                            ent.DBTypeName = property.PropertyType.IsEnum ? "Int32" : DBTool.GetValue(ent.DBTypeName, property.PropertyType.Name);

                            switch (ent.DBTypeName.ToLower()) {
                                case "int":
                                    ent.DBTypeName = "Int32";
                                    break;
                                case "long":
                                    ent.DBTypeName = "Int64";
                                    break;
                                case "short":
                                    ent.DBTypeName = "Int16";
                                    break;
                                case "bool":
                                    ent.DBTypeName = "Boolean";
                                    break;
                            }
                            idic[ent.ParameterName.Trim()] = ent;
                            idic2[ent.ParameterName.Trim()] = property;
                            //将属性定义转换成对应的类型
                        }
                    }
                    #endregion
                    if (!hasParameterAttributeType) {
                        #region 无属性定义
                        foreach (PropertyInfo property in t.GetProperties(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public)) {
                            ParameterEntity ent = new ParameterEntity();
                            ent.ParameterName = property.Name;
                            ent.DBTypeName = property.PropertyType.IsEnum ? "Int32" : property.PropertyType.Name;
                            switch (ent.DBTypeName.ToLower()) {
                                case "int":
                                    ent.DBTypeName = "Int32";
                                    break;
                                case "long":
                                    ent.DBTypeName = "Int64";
                                    break;
                                case "short":
                                    ent.DBTypeName = "Int16";
                                    break;
                                case "bool":
                                    ent.DBTypeName = "Boolean";
                                    break;
                            }
                            idic[ent.ParameterName.Trim().Trim('@', '&', '$')] = ent;
                            idic2[ent.ParameterName.Trim()] = property;
                        }
                        #endregion
                    }

                    if (idic.Count > 0) {
                        object[] values = new object[dt.Rows.Count];
                        int w = 0;
                        foreach (DataRow row in dt.Rows) {
                            object _t = Activator.CreateInstance(t);
                            foreach (KeyValuePair<string, ParameterEntity> pair in idic) {
                                MethodInfo me = convertType.GetMethod("To" + pair.Value.DBTypeName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, new Type[] { stringType }, null);
                                if (me == null)
                                    throw new InvalidOperationException(string.Format("Conver.To{0} 方法没有找到！", pair.Value.DBTypeName));
                                else if (row.Table.Columns.Contains(pair.Key))
                                    if (idic2[pair.Key].PropertyType.IsEnum) {
                                        Type typEnum = idic2[pair.Key].PropertyType;
                                        idic2[pair.Key].SetValue(_t, Enum.Parse(typEnum, Enum.GetName(typEnum, me.Invoke(null, new object[] { DBNull.Value.Equals(row[pair.Key]) ? null : row[pair.Key] }))), null);
                                    } else
                                        idic2[pair.Key].SetValue(_t, me.Invoke(null, new object[] { DBNull.Value.Equals(row[pair.Key]) ? null : row[pair.Key] }), null);
                            }
                            values[w] = _t;
                            w++;
                        }
                        return values;
                    }
                } else {
                    object[] values2 = new object[dt.Rows.Count];
                    int q = 0;
                    for (System.Collections.IEnumerator ie = dt.Rows.GetEnumerator(); ie.MoveNext(); q++) {
                        IRowToObject i = Activator.CreateInstance(t) as IRowToObject;
                        i.Fill(ie.Current as DataRow);
                        values2[q] = i;
                    }
                    return values2;
                }
            } catch (System.ArgumentNullException) {
            } catch (System.ArgumentException) {
            } catch (System.NotSupportedException) {
            } catch (System.Reflection.TargetInvocationException) {
                throw;
            } catch (System.MethodAccessException) {
            } catch (System.MissingMethodException) {
            } catch (System.MemberAccessException) {
            } catch (System.TypeLoadException) {
                throw;
            } catch (System.Runtime.InteropServices.InvalidComObjectException) {
            } catch (System.Runtime.InteropServices.COMException) {
            } catch (Exception) {
                throw new InvalidOperationException(string.Format("{0}类型不是IRowToObject接口的子类!", t.FullName));
            }
            return null;
        }

        /// <summary>
        /// 使用DataRow进行赋值（大小写名称敏感）
        /// </summary>
        /// <param name="t"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public object Fill(Type t, DataRow row) {
            try {
                if (t.GetInterface(IRowToObjectType.FullName) == null) {
                    IDictionary<string, ParameterEntity> idic = new Dictionary<string, ParameterEntity>();
                    IDictionary<string, PropertyInfo> idic2 = new Dictionary<string, PropertyInfo>();
                    bool hasParameterAttributeType = false;
                    #region 有属性定义
                    foreach (PropertyInfo property in t.GetProperties(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public)) {
                        if (property.GetCustomAttributes(typeof(NiDataParameterAttribute), true).Length > 0) {
                            hasParameterAttributeType = true;
                            ParameterEntity ent = (property.GetCustomAttributes(typeof(NiDataParameterAttribute), true)[0] as NiDataParameterAttribute).Entity.Clone() as ParameterEntity;
                            ent.ParameterName = DBTool.GetValue(ent.ParameterName.Trim(), property.Name.Trim()).TrimStart('$', '&', '@');
                            ent.DBTypeName = property.PropertyType.Name;
                            switch (ent.DBTypeName.ToLower()) {
                                case "int":
                                    ent.DBTypeName = "Int32";
                                    break;
                                case "long":
                                    ent.DBTypeName = "Int64";
                                    break;
                                case "short":
                                    ent.DBTypeName = "Int16";
                                    break;
                                case "bool":
                                    ent.DBTypeName = "Boolean";
                                    break;
                            }
                            idic[ent.ParameterName.Trim()] = ent;
                            idic2[ent.ParameterName.Trim()] = property;
                            //将属性定义转换成对应的类型
                        }
                    }
                    #endregion
                    if (!hasParameterAttributeType) {
                        #region 无属性定义
                        foreach (PropertyInfo property in t.GetProperties(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.Public)) {
                            ParameterEntity ent = new ParameterEntity();
                            ent.ParameterName = property.Name;
                            ent.DBTypeName = property.PropertyType.Name;
                            switch (ent.DBTypeName.ToLower()) {
                                case "int":
                                    ent.DBTypeName = "Int32";
                                    break;
                                case "long":
                                    ent.DBTypeName = "Int64";
                                    break;
                                case "short":
                                    ent.DBTypeName = "Int16";
                                    break;
                                case "bool":
                                    ent.DBTypeName = "Boolean";
                                    break;
                            }
                            idic[ent.ParameterName.Trim().Trim('@', '&', '$')] = ent;
                        }
                        #endregion
                    }

                    if (idic.Count > 0) {
                        object _t = Activator.CreateInstance(t);
                        foreach (KeyValuePair<string, ParameterEntity> pair in idic) {
                            MethodInfo me = convertType.GetMethod("To" + pair.Value.DBTypeName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, new Type[] { stringType }, null);
                            if (me == null)
                                throw new InvalidOperationException(string.Format("Conver.To{0} 方法没有找到！", pair.Value.DBTypeName));
                            else if (row.Table.Columns.Contains(pair.Key))
                                idic2[pair.Key].SetValue(_t, me.Invoke(null, new object[] { row[pair.Key] }), null);
                        }
                        return _t;
                    }
                } else {
                    IRowToObject i = Activator.CreateInstance(t) as IRowToObject;
                    i.Fill(row);
                    return i;
                }
            } catch (System.ArgumentNullException) {
            } catch (System.ArgumentException) {
            } catch (System.NotSupportedException) {
            } catch (System.Reflection.TargetInvocationException) {
                throw;
            } catch (System.MethodAccessException) {
            } catch (System.MissingMethodException) {
            } catch (System.MemberAccessException) {
            } catch (System.TypeLoadException) {
                throw;
            } catch (System.Runtime.InteropServices.InvalidComObjectException) {
            } catch (System.Runtime.InteropServices.COMException) {
            } catch (Exception) {
                throw new InvalidOperationException(string.Format("{0}类型不是IRowToObject接口的子类!", t.FullName));
            }
            return null;
        }
        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <param name="t"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public object[] Fill(Type t, string table) {
            return Fill(t, ds.Tables[table]);
        }

        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <param name="t"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public object[] Fill(Type t, int table) {
            return Fill(t, ds.Tables[table]);
        }


        static readonly Type convertType = typeof(Convert);
        static Type stringType = typeof(object);
        public T Fill<T>(DataRow row) where T : class {
            Type t = typeof(T);
            return Fill(t, row) as T;
        }
        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public T[] Fill<T>(DataTable dt) where T : class {
            Type t = typeof(T);
            var ret = Fill(t, dt);
            if (ret == null)
                return new T[] { Activator.CreateInstance<T>() };
            else
                return Array.ConvertAll<object, T>(ret, new Converter<object, T>(p => p as T));
        }

        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <param name="T"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public T[] Fill<T>(string table) where T : class {
            return Fill<T>(ds.Tables[table]);
        }

        /// <summary>
        /// 批量获取无参构造的对象,类型必须自己实现IRowToObject接口
        /// </summary>
        /// <param name="T"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public T[] Fill<T>(int table) where T : class {
            return Fill<T>(ds.Tables[table]);
        }

        public bool HasData() {
            if (this.ds.Tables.Count > 0) {
                foreach (DataTable dt in this.ds.Tables) {
                    if (dt.Rows.Count > 0) return true;
                }
            }
            return false;
        }

        public bool HasData(int id) {
            if (this.ds.Tables.Count >= (id + 1)) {
                if (this.ds.Tables[id].Rows.Count > 0) return true;
            }
            return false;
        }

        public bool HasData(string id) {
            if (this.ds.Tables.Count > 0 && this.ds.Tables.Contains(id)) {
                if (this.ds.Tables[id].Rows.Count > 0) return true;
            }
            return false;
        }
        #region IDisposable Members
        public void Dispose() {
            this.Clear();
            this.ds = null;
            this.idic = null;
        }

        private object clearKey = DateTime.Now;
        public void Clear() {
            lock (clearKey) {
                if (ds != null) {
                    this.DataSet.Tables.Clear();
                    this.DataSet.AcceptChanges();
                }
                if (idic != null)
                    this.OutParameter.Clear();
            }
        }

        #endregion
    }

    public interface IRowToObject {
        void Fill(DataRow row);
    }
    //public class TestGeneric<TKey,TValue>
    //    where TKey:IRowToObject
    //    where TValue:IRowToObject {

    //}
}