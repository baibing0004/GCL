using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Reflection;
using GCL.Common;

namespace GCL.Bean {

    /// <summary>
    /// 转换方式级别
    /// </summary>
    public enum BeanBandingFlags {

        /// <summary>
        /// 公共变量
        /// </summary>
        Field = 1,

        /// <summary>
        /// 类Java的Bean方式方法
        /// </summary>
        BeanMethod = 2,

        /// <summary>
        /// 使用类Map的Get/Set方式的方法
        /// </summary>
        Map = 4,

        /// <summary>
        /// .Net 属性
        /// </summary>        
        Property = 8,

        /// <summary>
        /// .Net this属性方式 
        /// </summary>
        This = 16,

        /// <summary>
        /// 全部可能
        /// </summary>
        All = Field + BeanMethod + Map + Property + This
    }

    public class BeanTool : Tool {

        public static byte[] SerializeToByte(object infos) {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            byte[] data = null;
            try {
                new BinaryFormatter().Serialize(mem, infos);
                data = mem.ToArray();
            } finally {
                try {
                    mem.Close();
                } catch {
                }
            }
            return data;
        }

        public static object DeserializeToObject(byte[] data) {
            MemoryStream mem = new MemoryStream();
            try {
                mem.Write(data, 0, data.Length);
                mem.Flush();
                mem.Position = 0;
                return new BinaryFormatter().Deserialize(mem);
            } finally {
                try {
                    mem.Close();
                } catch {
                }
            }
        }
        /*
         * <P>
         * 用于bean的Get/set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的set方法 全部都匹配
         * 
         * @param source
         *            源bean对象
         * @param aim
         *            目的bean对象
         */
        public static void Transport(object source, object aim) {
            Transport(source, aim, BeanBandingFlags.All);
        }

        /*
        * <P>
        * 用于bean的Get/set方法之间互相赋值
        * <p>
        * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的set方法 全部都匹配
        * 
        * @param source
        *            源bean对象
        * @param aim
        *            目的bean对象
        */
        public static void Transport(object source, object aim, BeanBandingFlags flag) {
            Transport(source, aim, null, null, flag);
        }

        /*
         * <P>
         * 用于bean的Get/Set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的Set方法
         * 
         * @param source
         *            源bean对象
         * @param aim
         *            目的bean对象
         * @param passPro
         *            许可的属性
         * @param forPro
         *            不许可的属性
         */
        public static void Transport(object source, object aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            Transport(source, aim, passPro, forPro, BeanBandingFlags.All);
        }

        /*
         * <P>
         * 用于bean的Get/Set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的Set方法
         * 
         * @param source
         *            源bean对象
         * @param aim
         *            目的bean对象
         * @param passPro
         *            许可的属性
         * @param forPro
         *            不许可的属性
         */
        public static void Transport(object source, object aim, ICollection<object> passPro,
                ICollection<object> forPro, BeanBandingFlags flag) {

            // 判读无效对象直接返回
            if (!IsEnable(source) || !IsEnable(aim))
                return;

            Type bcl = aim.GetType();

            // 获得a的方法
            MemberInfo[] ame = source.GetType().GetMembers();
            // 进行方法遍历
            for (int w = 0; w < ame.Length; w++) {
                if (!(ame[w].MemberType == MemberTypes.Field || ame[w].MemberType == MemberTypes.Method))
                    continue;

                if (ame[w].MemberType == MemberTypes.Field) {
                    //处理公共变量
                    FieldInfo fe = (FieldInfo)ame[w];
                    //!fe.IsAssembly || 
                    if ((IsEnable(passPro) && !passPro.Contains(fe.Name))
                        || (IsEnable(forPro) && forPro.Contains(fe.Name))) continue;

                    try {
                        object data = fe.GetValue(source);
                        SetPropertyValueSP(aim, fe.Name, data.GetType(),
                                data, flag);
                    } catch (Exception e) {
                        // 调用失败 抛出异常 不处理
                    }
                    continue;
                }
                //处理公共方法
                MethodInfo me = (MethodInfo)ame[w];
                string methodName = me.Name.Trim();
                if (methodName.ToLower().StartsWith("get") || methodName.ToLower().StartsWith("is")) {
                    string proName = "";
                    if (methodName.Length > 3)
                        proName = methodName.Substring(methodName.ToLower().StartsWith("get") ? 3 : 2).TrimStart('_');
                    // System.out.println(proName);
                    if ((IsEnable(passPro) && !passPro.Contains(proName))
                            || (IsEnable(forPro) && forPro.Contains(proName)))
                        continue;

                    //&& me.IsAssembly
                    if (!me.ReturnType.Equals(typeof(void))
                            && me.GetParameters().Length == 0) {
                        // 找到无参数Get方法
                        try {
                            object data = me.Invoke(source, null);
                            SetPropertyValueSP(aim, proName, data.GetType(), data, flag);
                        } catch {
                            // 调用失败 抛出异常 不处理
                        }
                    }
                } else {
                    // 没有找到合适的方法不操作
                }
            }
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>        
        /// <param name="aim">目的对象类</param>        
        /// <param name="flag">替换类型</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                Type aim) {
            TransportAll(sourceCol, aim, BeanBandingFlags.All);
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>        
        /// <param name="aim">目的对象类</param>        
        /// <param name="flag">替换类型</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                Type aim, BeanBandingFlags flag) {
            TransportAll(sourceCol, new System.Collections.Generic.List<object>(), aim, flag);
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>
        /// <param name="aimCol">目的对象的存储集合类</param>
        /// <param name="aim">目的对象类</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim) {
            TransportAll(sourceCol, aimCol, aim, BeanBandingFlags.All);
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>
        /// <param name="aimCol">目的对象的存储集合类</param>
        /// <param name="aim">目的对象类</param>
        /// <param name="flag">替换类型</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim, BeanBandingFlags flag) {
            TransportAll(sourceCol, aimCol, aim, null, null, flag);
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>
        /// <param name="aimCol">目的对象的存储集合类</param>
        /// <param name="aim">目的对象类</param>
        /// <param name="passPro">允许赋值的属性集合</param>
        /// <param name="forPro">不允许赋值的属性集合</param>
        /// <param name="flag">替换类型</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            TransportAll(sourceCol, aimCol, aim, passPro, forPro, BeanBandingFlags.All);
        }

        /// <summary>
        /// 用于批量从源集合中生成目的对象的集合 要求目的对象有无参数的初始化方法
        /// </summary>
        /// <param name="sourceCol">源对象集合</param>
        /// <param name="aimCol">目的对象的存储集合类</param>
        /// <param name="aim">目的对象类</param>
        /// <param name="passPro">允许赋值的属性集合</param>
        /// <param name="forPro">不允许赋值的属性集合</param>
        /// <param name="flag">替换类型</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim, ICollection<object> passPro,
                ICollection<object> forPro, BeanBandingFlags flag) {
            aimCol.Clear();
            if (sourceCol.Count == 0)
                return;
            ConstructorInfo cons;
            try {
                cons = aim.GetConstructor(new Type[0]);
            } catch (Exception e) {
                throw new Exception("aim对象缺少无参数初始化函数，或者为空对象 错误说明：" + e.ToString());
            }
            for (IEnumerator<object> ite = sourceCol.GetEnumerator(); ite.MoveNext(); ) {
                object _source = ite.Current;
                object _aim;
                try {
                    _aim = cons.Invoke(new object[0]);
                } catch (Exception e) {
                    throw new Exception("aim对象空参数实例化错误，错误说明：" + e.ToString());
                }
                Transport(_source, _aim, passPro, forPro, flag);
                try {
                    aimCol.Add(_aim);
                } catch (Exception e) {
                    throw new Exception("aimCol对象为空，错误说明：" + e.ToString());
                }
            }
            return;
        }

        /// <summary>
        /// 获得方法
        /// </summary>
        /// <param name="cl">对象类</param>
        /// <param name="name">方法名</param>
        /// <returns></returns>
        public static MethodInfo[] GetAllMethod(Type cl, string name) {
            return GetAllMethod(cl, name, 0);
        }

        /// <summary>
        /// 获得方法
        /// </summary>
        /// <param name="cl">对象类</param>
        /// <param name="name">方法名</param>
        /// <param name="size">参数个数</param>
        /// <returns></returns>
        public static MethodInfo[] GetAllMethod(Type cl, string name, int size) {
            List<MethodInfo> cols = new List<MethodInfo>();
            MethodInfo[] mes = cl.GetMethods();
            for (int w = 0; w < mes.Length; w++)
                if (mes[w].Name.Trim().Equals(name))
                    if (size <= 0
                            || (size > 0 && mes[w].GetParameters().Length == size))
                        cols.Add(mes[w]);
            if (cols.Count == 0)
                return new MethodInfo[0];
            else return cols.ToArray();
        }

        /// <summary>
        /// 获得同名无参方法
        /// </summary>
        /// <param name="type">对象类</param>
        /// <param name="name">方法名</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string name) {
            return type.GetMethod(name, methodFlags | BindingFlags.InvokeMethod | BindingFlags.CreateInstance);
        }

        /// <summary>
        /// 根据已经有的参数集合 获得方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="paraTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string name, params Type[] paraTypes) {
            if (!IsEnable(paraTypes))
                return GetMethod(type, name);
            else
                return type.GetMethod(name, methodFlags | BindingFlags.CreateInstance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, paraTypes, null);
        }
        /// <summary>
        /// 尝试使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                 object para) {
            return SetPropertyValueSP(aim, name, null, para);
        }

        /// <summary>
        /// 使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                 object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(aim, name, null,
                         para, flag);

        }

        /// <summary>
        /// 尝试所有类型方法进行操作设置属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                 object[] paras) {
            return SetPropertyValue(aim, name, null, paras);
        }

        /// <summary>
        /// 进行操作设置属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                 object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(aim, name, null, paras, flag);
        }

        /// <summary>
        /// 尝试使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                Type paraType, object para) {
            return SetPropertyValueSP(aim, name, paraType, para, BeanBandingFlags.All);
        }

        /// <summary>
        /// 使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                Type paraType, object para, BeanBandingFlags flag) {
            if (IsEnable(paraType) && IsEnable(para))
                return SetPropertyValue(aim, name, new Type[] { paraType },
                        new object[] { para }, flag);
            else if (IsEnable(para))
                return SetPropertyValue(aim, name, new Type[] { para.GetType() }, new object[] { para }, flag);
            else
                return SetPropertyValue(aim, name, null, null, flag);
        }

        /// <summary>
        /// 尝试所有类型方法进行操作设置属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras) {
            return SetPropertyValue(aim, name, paraTypes, paras, BeanBandingFlags.All);
        }

        /// <summary>
        /// 进行操作设置属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return OperatePropertyValue(aim, name, paraTypes, paras, flag, "Set");
        }

        /// <summary>
        /// 尝试所有方式获得无参属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name) {
            return GetPropertyValue(aim, name, BeanBandingFlags.All);
        }

        /// <summary>
        /// 获得无参属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name, BeanBandingFlags flag) {
            return GetPropertyValue(aim, name, null, null, flag);
        }

        /// <summary>
        /// 尝试使用单参数值获得属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object GetPropertyValueSP(object aim, string name,
                Type paraType, object para) {
            return GetPropertyValueSP(aim, name, paraType, para, BeanBandingFlags.All);
        }

        /// <summary>
        /// 使用单参数值获得属性值
        /// </summary>
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object GetPropertyValueSP(object aim, string name,
                Type paraType, object para, BeanBandingFlags flag) {
            if (IsEnable(paraType) && IsEnable(para))
                return GetPropertyValue(aim, name, new Type[] { paraType },
                        new object[] { para }, flag);
            else if (IsEnable(para))
                return GetPropertyValue(aim, name, new Type[] { para.GetType() }, new object[] { para }, flag);
            else
                return GetPropertyValue(aim, name, null, null, flag);
        }

        /// <summary>
        /// 尝试所有类型方法进行操作获得属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras) {
            return GetPropertyValue(aim, name, paraTypes, paras, BeanBandingFlags.All);
        }

        /// <summary>
        /// 进行操作获得属性值
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return OperatePropertyValue(aim, name, paraTypes, paras, flag, "Get");
        }

        /// <summary>
        /// 进行操作获得属性值或者赋予属性值操作
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <param name="operate">Get/Set</param>
        /// <returns></returns>
        private static object OperatePropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag, string operate) {
            if (!IsEnable(aim))
                throw new Exception("调用方法对象为空!");
            name = ToStringValue(name).Trim();
            IDictionary<string, object> map = GetFinalAim(aim, name);
            aim = map["aim"];
            name = ToStringValue(map["name"]);

            Exception nsexOut = null;
            int _flagType = (int)Math.Log((int)flag, 2) + 1;
            for (int w = 0; w < _flagType; w++) {
                string _name = name;
                object[] _paras = paras;
                //最多进行4次尝试
                switch (w) {
                    case 0:
                        if ((flag & BeanBandingFlags.Field) > 0) {
                            try {
                                FieldInfo fe = aim.GetType().GetField(_name, methodFlags | BindingFlags.GetField | BindingFlags.SetField);
                                if (fe == null) continue;
                                if (operate.ToLower().StartsWith("get") || operate.ToLower().StartsWith("is"))
                                    return fe.GetValue(aim);
                                else
                                    fe.SetValue(aim, _paras[0]);
                                return null;
                            } catch (Exception ex) {
                                nsexOut = ex;
                            }
                        }
                        continue;
                    case 1:
                        if ((flag & BeanBandingFlags.BeanMethod) > 0) {
                            if (!name.StartsWith(operate)) {
                                if (name.Length > 1)
                                    _name = operate + name.Substring(0, 1).ToUpper()
                                            + name.Substring(1);
                                else if (name.Length == 1)
                                    _name = operate + name.ToUpper();
                                else
                                    _name = operate;
                            }
                        } else
                            continue;
                        break;
                    case 2:
                        if ((flag & BeanBandingFlags.Map) > 0) {
                            _name = operate;
                        } else
                            continue;
                        break;
                    case 3:
                        if ((flag & BeanBandingFlags.Property) > 0 && name.Length > 0) {
                            if (!name.StartsWith(operate.ToLower() + "_"))
                                _name = operate.ToLower() + "_" + name;
                        } else continue;
                        break;
                    case 4:
                        if ((flag & BeanBandingFlags.This) > 0) {
                            _name = operate.ToLower() + "_Item";
                        } else
                            continue;
                        break;
                    default:
                        continue;
                }

                try {
                    return Invoke(aim, _name, paraTypes, _paras);
                } catch (Exception nsex) {
                    nsexOut = nsex;
                    try {
                        // 二次尝试进行模糊设置
                        return Invoke(aim, _name, paras);
                    } catch { }
                    /* 
                       // 二次尝试进行对GetMap方法的模糊设置操作
                       if (!name.Trim().equals(operate))
                           try {
                               if (IsEnable(paras))
                                   return Invoke(aim, operate, new object[] { name,
                                       paras });
                               else
                                   return Invoke(aim, operate, new object[] { name });
                           } catch (Exception e) {
                               e.printStackTrace();
                               throw new Exception(string.Format("没有 {0}//Get 方法存在！",
                                       name));
                           }
                       throw nsex;
                   }*/
                }
            }

            if (IsEnable(nsexOut))
                throw nsexOut;
            else
                throw new MissingMethodException(string.Format("没有 {0}//{1} 方法存在！", name, operate));
        }

        /*
         * 通过对象参数确定方法并调用返回结果 调用无参数方法
         * 
         * @param aim
         *            操作对象
         * @param name
         *            方法名
         * @return 结果
         * @throws Exception
         *             所有可能的错误
         */
        public static object Invoke(object aim, string name) {
            return Invoke(aim, name, null, null);
        }

        /*
         * 通过对象参数确定方法并调用返回结果
         * 
         * @param aim
         *            操作对象
         * @param name
         *            方法名
         * @param paraType
         *            方法参数类型
         * @param para
         *            方法参数
         * @return 结果
         * @throws Exception
         *             所有可能的错误
         */
        public static object InvokeSP(object aim, string name, Type paraType,
                object para) {
            if (IsEnable(paraType) && IsEnable(para))
                return Invoke(aim, name, new Type[] { paraType },
                        new object[] { para });
            else if (IsEnable(para))
                return Invoke(aim, name, para);
            else
                return Invoke(aim, name, null, null);
        }

        private static BindingFlags methodFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
        /*
         * 通过对象参数确定方法并调用返回结果
         * 
         * @param aim
         *            操作对象
         * @param name
         *            方法名
         * @param paraTypes
         *            方法参数类型
         * @param paras
         *            方法参数
         * @return 结果
         * @throws Exception
         *             所有可能的错误
         */
        public static object Invoke(object aim, string name, Type[] paraTypes,
                object[] paras) {
            name = ToStringValue(name).Trim();
            IDictionary<string, object> map = GetFinalAim(aim, name);
            aim = map["aim"];
            name = ToStringValue(map["name"]);
            MethodInfo method = GetMethod(aim.GetType(), name, paraTypes);
            if (!IsEnable(method))
                throw new MissingMethodException(string.Format("{0}.{1}方法没有找到", aim.GetType().FullName, name));
            return method.Invoke(aim, paras);
        }

        /// <summary>
        /// 通过对象参数模糊确定方法并调用返回结果
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性名称 开头字母不大写</param>
        /// <param name="para">属性值</param>
        /// <returns></returns>
        public static object Invoke(object aim, string name, params object[] para) {
            if (!IsEnable(aim) || !IsEnable(name) || !IsEnable(para))
                throw new MissingMethodException(string.Format("没有 对应参数的{0}方法存在！",
                        ToStringValue(name)));

            name = ToStringValue(name).Trim();
            IDictionary<string, object> map = GetFinalAim(aim, name);
            aim = map["aim"];
            name = ToStringValue(map["name"]);

            MethodInfo[] mes = GetAllMethod(aim.GetType(), name, para.Length);
            Array.Sort<MethodInfo>(mes, StringMethodComparator.GetComparator());

            if (IsEnable(para) && IsEnable(mes)) {
                Exception ex = null;
                for (int w = 0; w < mes.Length; w++) {
                    ParameterInfo[] cl = mes[w].GetParameters();
                    try {
                        for (int q = 0; q < cl.Length; q++) {
                            //判断参数类型是不是方法类型的子类 如果是 说明可以进行运算
                            if (!cl[q].ParameterType.IsInstanceOfType(para[q]))
                                throw new Exception();
                        }

                        try {
                            return mes[w].Invoke(aim, methodFlags | BindingFlags.CreateInstance | BindingFlags.InvokeMethod, null, para, null);
                        } catch (Exception e) {
                            ex = e;
                        }
                    } catch { }
                }
                if (IsEnable(ex))
                    throw ex;
                else
                    // 没有匹配成功的方法抛出错误
                    throw new MissingMethodException(string.Format(
                            "没有 对应参数的{0}方法存在！", name));

            } else
                // 没有匹配成功的方法抛出错误
                throw new MissingMethodException(string.Format("没有 对应参数的{0}方法存在！",
                        name));

        }

        /*
         * 用于获得这个对象类的所有Bean类型方法名
         * @param cls
         * @return
         * @throws Exception
         */
        public static string[] GetFieldNames(Type cls) {
            return GetFieldNames(cls, 0);
        }

        /*
        * 用于获得这个对象类的所有GetBean类型方法名
        * @param cls
        * @return
        * @throws Exception
        */
        public static string[] GetGetFieldNames(Type cls) {
            return GetFieldNames(cls, 1);
        }

        /*
         * 用于获得这个对象类的所有SetBean类型方法名
         * @param cls
         * @return
         * @throws Exception
         */
        public static string[] GetSetFieldNames(Type cls) {
            return GetFieldNames(cls, 2);
        }


        /*
         * 用于获得这个对象类的所有SetBean类型方法名
         * @param cls
         * @return
         * @throws Exception
         */
        private static string[] GetFieldNames(Type cls, int flag) {
            System.Collections.ArrayList set = new System.Collections.ArrayList();
            MethodInfo[] mes = cls.GetMethods();
            for (int w = 0; w < mes.Length; w++) {
                switch (flag % 3) {
                    case 0:
                        if (mes[w].Name.Trim().ToLower().StartsWith("set") || mes[w].Name.Trim().ToLower().StartsWith("get"))
                            set.Add(mes[w].Name.Substring(3).TrimStart('_'));
                        break;
                    case 1:
                        if (mes[w].Name.Trim().ToLower().StartsWith("get"))
                            set.Add(mes[w].Name.Substring(3).TrimStart('_'));
                        break;
                    case 2:
                        if (mes[w].Name.Trim().ToLower().StartsWith("set"))
                            set.Add(mes[w].Name.Substring(3).TrimStart('_'));
                        break;
                }
            }
            return (string[])set.ToArray(typeof(string));
        }

        /*
         * 获得最终的操作对象 允许点运算符进行对象的属性的属性的方法的判断
         * 
         * @param aim
         * @param name
         * @return
         * @throws Exception
         */
        private static IDictionary<string, object> GetFinalAim(object aim, string name) {
            IDictionary<string, object> map = new System.Collections.Generic.Dictionary<string, object>();
            map.Add("aim", aim);
            map.Add("name", name);
            if (name.IndexOf(".") > 0) {
                object source = aim;
                string[] props = name.Split('.');
                for (int w = 0; w < props.Length - 1; w++)
                    source = GetPropertyValue(source, props[w]);
                if (!IsEnable(source))
                    throw new MissingMethodException(string.Format(
                            "没有 对应参数的{0}方法存在！", ToStringValue(name)));
                map.Add("aim", source);
                map.Add("name", props[props.Length - 1]);
                return map;
            } else
                return map;
        }

        /// <summary>
        /// 创建无参数对象实例
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyName, string typeName) {
            return CreateInstance(assemblyName, typeName, null);
        }

        /// <summary>
        /// 创建多参数对象实例
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyName, string typeName, object[] paras) {
            if (IsEnable(assemblyName)) {
                string _l = assemblyName.ToLower();
                if ((_l.IndexOf(".dll") >= 0 || _l.IndexOf(".exe") >= 0) && _l.IndexOf(":") < 0)
                    assemblyName = System.AppDomain.CurrentDomain.BaseDirectory + assemblyName;
                object obj = null;

                try {
                    if (IsEnable(paras) && paras.Length > 0)
                        obj = System.Reflection.Assembly.LoadFrom(assemblyName).CreateInstance(typeName, false, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null, paras, null, null);
                    else
                        obj = System.Reflection.Assembly.LoadFrom(assemblyName).CreateInstance(typeName);
                } catch (System.BadImageFormatException) {
                }

                //return Activator.CreateInstanceFrom(assemblyName, typeName).Unwrap();
                if (obj == null && (_l.EndsWith(".dll") || _l.EndsWith(".exe"))) {
                    if (IsEnable(paras) && paras.Length > 0)
                        return System.Reflection.Assembly.LoadFile(assemblyName).CreateInstance(typeName, false, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null, paras, null, null);
                    else
                        return System.Reflection.Assembly.LoadFile(assemblyName).CreateInstance(typeName);
                } else
                    return obj;
            } else
                return Activator.CreateInstance(Type.GetType(typeName));
        }

        /// <summary>
        /// 模拟调用无参数Close方法然后根据是否IDispose对象进行销毁
        /// </summary>
        /// <param name="v"></param>
        public static void Close(object v) {
            try {
                InvokeSP(v, "Close", null, null);
            } catch {
            }
            if (v is IDisposable)
                try {
                    ((IDisposable)v).Dispose();
                } catch {
                }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool[] ToBoolean(object[] array) {
            return Array.ConvertAll<object, bool>(array, Convert.ToBoolean);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] ToDouble(object[] array) {
            return Array.ConvertAll<object, double>(array, Convert.ToDouble);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float[] ToSingle(object[] array) {
            return Array.ConvertAll<object, float>(array, Convert.ToSingle);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal[] ToDecimal(object[] array) {
            return Array.ConvertAll<object, decimal>(array, Convert.ToDecimal);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] ToByte(object[] array) {
            return Array.ConvertAll<object, byte>(array, Convert.ToByte);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static sbyte[] ToSByte(object[] array) {
            return Array.ConvertAll<object, sbyte>(array, Convert.ToSByte);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static short[] ToInt16(object[] array) {
            return Array.ConvertAll<object, short>(array, Convert.ToInt16);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ushort[] ToUInt16(object[] array) {
            return Array.ConvertAll<object, ushort>(array, Convert.ToUInt16);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] ToInt32(object[] array) {
            return Array.ConvertAll<object, int>(array, Convert.ToInt32);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static uint[] ToUInt32(object[] array) {
            return Array.ConvertAll<object, uint>(array, Convert.ToUInt32);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long[] ToInt64(object[] array) {
            return Array.ConvertAll<object, long>(array, Convert.ToInt64);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ulong[] ToUInt64(object[] array) {
            return Array.ConvertAll<object, ulong>(array, Convert.ToUInt64);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static DateTime[] ToDateTime(object[] array) {
            return Array.ConvertAll<object, DateTime>(array, Convert.ToDateTime);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static char[] ToChar(object[] array) {
            return Array.ConvertAll<object, char>(array, Convert.ToChar);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] ToString(object[] array) {
            return Array.ConvertAll<object, string>(array, Convert.ToString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Type[] ToType(object[] array) {
            return Array.ConvertAll<object, Type>(array, ToType);
        }

        static Type ToType(object obj) { return obj as Type; }
    }

    class StringMethodComparator : IComparer<MethodInfo> {

        #region IComparer<MethodInfo> Members

        public int Compare(MethodInfo x, MethodInfo y) {
            ParameterInfo[] am = x.GetParameters();
            ParameterInfo[] bm = y.GetParameters();
            if (am.Length == bm.Length)
                return GetStringTypeCount(x.GetParameters())
                        - GetStringTypeCount(y.GetParameters());
            else
                return am.Length - bm.Length;
        }

        /// <summary>
        /// 获取参数中string类型的数目
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static int GetStringTypeCount(ParameterInfo[] data) {
            int q = 100;
            for (int w = 0; w < data.Length; w++)
                if (data[w].ParameterType.Equals(typeof(string)))
                    q++;
                else if (data[w].ParameterType.Equals(typeof(double)))
                    q -= 1;
                else if (data[w].ParameterType.Equals(typeof(float)))
                    q -= 2;
                else if (data[w].ParameterType.Equals(typeof(long)))
                    q -= 3;
                else if (data[w].ParameterType.Equals(typeof(int)))
                    q -= 4;
                else if (data[w].ParameterType.Equals(typeof(short)))
                    q -= 5;
                else if (data[w].ParameterType.Equals(typeof(byte)))
                    q -= 6;
            return q;
        }

        #endregion

        static StringMethodComparator comp;

        internal static StringMethodComparator GetComparator() {
            if (!BeanTool.IsEnable(comp))
                comp = new StringMethodComparator();
            return comp;
        }
    }
}
