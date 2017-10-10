using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Reflection;
using GCL.Common;

namespace GCL.Bean {

    /// <summary>
    /// ת����ʽ����
    /// </summary>
    public enum BeanBandingFlags {

        /// <summary>
        /// ��������
        /// </summary>
        Field = 1,

        /// <summary>
        /// ��Java��Bean��ʽ����
        /// </summary>
        BeanMethod = 2,

        /// <summary>
        /// ʹ����Map��Get/Set��ʽ�ķ���
        /// </summary>
        Map = 4,

        /// <summary>
        /// .Net ����
        /// </summary>        
        Property = 8,

        /// <summary>
        /// .Net this���Է�ʽ 
        /// </summary>
        This = 16,

        /// <summary>
        /// ȫ������
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
         * ����bean��Get/set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����set���� ȫ����ƥ��
         * 
         * @param source
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         */
        public static void Transport(object source, object aim) {
            Transport(source, aim, BeanBandingFlags.All);
        }

        /*
        * <P>
        * ����bean��Get/set����֮�以�ำֵ
        * <p>
        * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����set���� ȫ����ƥ��
        * 
        * @param source
        *            Դbean����
        * @param aim
        *            Ŀ��bean����
        */
        public static void Transport(object source, object aim, BeanBandingFlags flag) {
            Transport(source, aim, null, null, flag);
        }

        /*
         * <P>
         * ����bean��Get/Set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����Set����
         * 
         * @param source
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         * @param passPro
         *            ��ɵ�����
         * @param forPro
         *            ����ɵ�����
         */
        public static void Transport(object source, object aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            Transport(source, aim, passPro, forPro, BeanBandingFlags.All);
        }

        /*
         * <P>
         * ����bean��Get/Set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����Set����
         * 
         * @param source
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         * @param passPro
         *            ��ɵ�����
         * @param forPro
         *            ����ɵ�����
         */
        public static void Transport(object source, object aim, ICollection<object> passPro,
                ICollection<object> forPro, BeanBandingFlags flag) {

            // �ж���Ч����ֱ�ӷ���
            if (!IsEnable(source) || !IsEnable(aim))
                return;

            Type bcl = aim.GetType();

            // ���a�ķ���
            MemberInfo[] ame = source.GetType().GetMembers();
            // ���з�������
            for (int w = 0; w < ame.Length; w++) {
                if (!(ame[w].MemberType == MemberTypes.Field || ame[w].MemberType == MemberTypes.Method))
                    continue;

                if (ame[w].MemberType == MemberTypes.Field) {
                    //����������
                    FieldInfo fe = (FieldInfo)ame[w];
                    //!fe.IsAssembly || 
                    if ((IsEnable(passPro) && !passPro.Contains(fe.Name))
                        || (IsEnable(forPro) && forPro.Contains(fe.Name))) continue;

                    try {
                        object data = fe.GetValue(source);
                        SetPropertyValueSP(aim, fe.Name, data.GetType(),
                                data, flag);
                    } catch (Exception e) {
                        // ����ʧ�� �׳��쳣 ������
                    }
                    continue;
                }
                //����������
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
                        // �ҵ��޲���Get����
                        try {
                            object data = me.Invoke(source, null);
                            SetPropertyValueSP(aim, proName, data.GetType(), data, flag);
                        } catch {
                            // ����ʧ�� �׳��쳣 ������
                        }
                    }
                } else {
                    // û���ҵ����ʵķ���������
                }
            }
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>        
        /// <param name="aim">Ŀ�Ķ�����</param>        
        /// <param name="flag">�滻����</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                Type aim) {
            TransportAll(sourceCol, aim, BeanBandingFlags.All);
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>        
        /// <param name="aim">Ŀ�Ķ�����</param>        
        /// <param name="flag">�滻����</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                Type aim, BeanBandingFlags flag) {
            TransportAll(sourceCol, new System.Collections.Generic.List<object>(), aim, flag);
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>
        /// <param name="aimCol">Ŀ�Ķ���Ĵ洢������</param>
        /// <param name="aim">Ŀ�Ķ�����</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim) {
            TransportAll(sourceCol, aimCol, aim, BeanBandingFlags.All);
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>
        /// <param name="aimCol">Ŀ�Ķ���Ĵ洢������</param>
        /// <param name="aim">Ŀ�Ķ�����</param>
        /// <param name="flag">�滻����</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim, BeanBandingFlags flag) {
            TransportAll(sourceCol, aimCol, aim, null, null, flag);
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>
        /// <param name="aimCol">Ŀ�Ķ���Ĵ洢������</param>
        /// <param name="aim">Ŀ�Ķ�����</param>
        /// <param name="passPro">����ֵ�����Լ���</param>
        /// <param name="forPro">������ֵ�����Լ���</param>
        /// <param name="flag">�滻����</param>
        /// <returns></returns>
        public static void TransportAll(ICollection<object> sourceCol,
                ICollection<object> aimCol, Type aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            TransportAll(sourceCol, aimCol, aim, passPro, forPro, BeanBandingFlags.All);
        }

        /// <summary>
        /// ����������Դ����������Ŀ�Ķ���ļ��� Ҫ��Ŀ�Ķ������޲����ĳ�ʼ������
        /// </summary>
        /// <param name="sourceCol">Դ���󼯺�</param>
        /// <param name="aimCol">Ŀ�Ķ���Ĵ洢������</param>
        /// <param name="aim">Ŀ�Ķ�����</param>
        /// <param name="passPro">����ֵ�����Լ���</param>
        /// <param name="forPro">������ֵ�����Լ���</param>
        /// <param name="flag">�滻����</param>
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
                throw new Exception("aim����ȱ���޲�����ʼ������������Ϊ�ն��� ����˵����" + e.ToString());
            }
            for (IEnumerator<object> ite = sourceCol.GetEnumerator(); ite.MoveNext(); ) {
                object _source = ite.Current;
                object _aim;
                try {
                    _aim = cons.Invoke(new object[0]);
                } catch (Exception e) {
                    throw new Exception("aim����ղ���ʵ�������󣬴���˵����" + e.ToString());
                }
                Transport(_source, _aim, passPro, forPro, flag);
                try {
                    aimCol.Add(_aim);
                } catch (Exception e) {
                    throw new Exception("aimCol����Ϊ�գ�����˵����" + e.ToString());
                }
            }
            return;
        }

        /// <summary>
        /// ��÷���
        /// </summary>
        /// <param name="cl">������</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public static MethodInfo[] GetAllMethod(Type cl, string name) {
            return GetAllMethod(cl, name, 0);
        }

        /// <summary>
        /// ��÷���
        /// </summary>
        /// <param name="cl">������</param>
        /// <param name="name">������</param>
        /// <param name="size">��������</param>
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
        /// ���ͬ���޲η���
        /// </summary>
        /// <param name="type">������</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string name) {
            return type.GetMethod(name, methodFlags | BindingFlags.InvokeMethod | BindingFlags.CreateInstance);
        }

        /// <summary>
        /// �����Ѿ��еĲ������� ��÷���
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
        /// ����ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                 object para) {
            return SetPropertyValueSP(aim, name, null, para);
        }

        /// <summary>
        /// ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                 object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(aim, name, null,
                         para, flag);

        }

        /// <summary>
        /// �����������ͷ������в�����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                 object[] paras) {
            return SetPropertyValue(aim, name, null, paras);
        }

        /// <summary>
        /// ���в�����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                 object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(aim, name, null, paras, flag);
        }

        /// <summary>
        /// ����ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object SetPropertyValueSP(object aim, string name,
                Type paraType, object para) {
            return SetPropertyValueSP(aim, name, paraType, para, BeanBandingFlags.All);
        }

        /// <summary>
        /// ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
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
        /// �����������ͷ������в�����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras) {
            return SetPropertyValue(aim, name, paraTypes, paras, BeanBandingFlags.All);
        }

        /// <summary>
        /// ���в�����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object SetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return OperatePropertyValue(aim, name, paraTypes, paras, flag, "Set");
        }

        /// <summary>
        /// �������з�ʽ����޲�����ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name) {
            return GetPropertyValue(aim, name, BeanBandingFlags.All);
        }

        /// <summary>
        /// ����޲�����ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name, BeanBandingFlags flag) {
            return GetPropertyValue(aim, name, null, null, flag);
        }

        /// <summary>
        /// ����ʹ�õ�����ֵ�������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object GetPropertyValueSP(object aim, string name,
                Type paraType, object para) {
            return GetPropertyValueSP(aim, name, paraType, para, BeanBandingFlags.All);
        }

        /// <summary>
        /// ʹ�õ�����ֵ�������ֵ
        /// </summary>
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
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
        /// �����������ͷ������в����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras) {
            return GetPropertyValue(aim, name, paraTypes, paras, BeanBandingFlags.All);
        }

        /// <summary>
        /// ���в����������ֵ
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public static object GetPropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return OperatePropertyValue(aim, name, paraTypes, paras, flag, "Get");
        }

        /// <summary>
        /// ���в����������ֵ���߸�������ֵ����
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <param name="operate">Get/Set</param>
        /// <returns></returns>
        private static object OperatePropertyValue(object aim, string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag, string operate) {
            if (!IsEnable(aim))
                throw new Exception("���÷�������Ϊ��!");
            name = ToStringValue(name).Trim();
            IDictionary<string, object> map = GetFinalAim(aim, name);
            aim = map["aim"];
            name = ToStringValue(map["name"]);

            Exception nsexOut = null;
            int _flagType = (int)Math.Log((int)flag, 2) + 1;
            for (int w = 0; w < _flagType; w++) {
                string _name = name;
                object[] _paras = paras;
                //������4�γ���
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
                        // ���γ��Խ���ģ������
                        return Invoke(aim, _name, paras);
                    } catch { }
                    /* 
                       // ���γ��Խ��ж�GetMap������ģ�����ò���
                       if (!name.Trim().equals(operate))
                           try {
                               if (IsEnable(paras))
                                   return Invoke(aim, operate, new object[] { name,
                                       paras });
                               else
                                   return Invoke(aim, operate, new object[] { name });
                           } catch (Exception e) {
                               e.printStackTrace();
                               throw new Exception(string.Format("û�� {0}//Get �������ڣ�",
                                       name));
                           }
                       throw nsex;
                   }*/
                }
            }

            if (IsEnable(nsexOut))
                throw nsexOut;
            else
                throw new MissingMethodException(string.Format("û�� {0}//{1} �������ڣ�", name, operate));
        }

        /*
         * ͨ���������ȷ�����������÷��ؽ�� �����޲�������
         * 
         * @param aim
         *            ��������
         * @param name
         *            ������
         * @return ���
         * @throws Exception
         *             ���п��ܵĴ���
         */
        public static object Invoke(object aim, string name) {
            return Invoke(aim, name, null, null);
        }

        /*
         * ͨ���������ȷ�����������÷��ؽ��
         * 
         * @param aim
         *            ��������
         * @param name
         *            ������
         * @param paraType
         *            ������������
         * @param para
         *            ��������
         * @return ���
         * @throws Exception
         *             ���п��ܵĴ���
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
         * ͨ���������ȷ�����������÷��ؽ��
         * 
         * @param aim
         *            ��������
         * @param name
         *            ������
         * @param paraTypes
         *            ������������
         * @param paras
         *            ��������
         * @return ���
         * @throws Exception
         *             ���п��ܵĴ���
         */
        public static object Invoke(object aim, string name, Type[] paraTypes,
                object[] paras) {
            name = ToStringValue(name).Trim();
            IDictionary<string, object> map = GetFinalAim(aim, name);
            aim = map["aim"];
            name = ToStringValue(map["name"]);
            MethodInfo method = GetMethod(aim.GetType(), name, paraTypes);
            if (!IsEnable(method))
                throw new MissingMethodException(string.Format("{0}.{1}����û���ҵ�", aim.GetType().FullName, name));
            return method.Invoke(aim, paras);
        }

        /// <summary>
        /// ͨ���������ģ��ȷ�����������÷��ؽ��
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ �������� ��ͷ��ĸ����д</param>
        /// <param name="para">����ֵ</param>
        /// <returns></returns>
        public static object Invoke(object aim, string name, params object[] para) {
            if (!IsEnable(aim) || !IsEnable(name) || !IsEnable(para))
                throw new MissingMethodException(string.Format("û�� ��Ӧ������{0}�������ڣ�",
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
                            //�жϲ��������ǲ��Ƿ������͵����� ����� ˵�����Խ�������
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
                    // û��ƥ��ɹ��ķ����׳�����
                    throw new MissingMethodException(string.Format(
                            "û�� ��Ӧ������{0}�������ڣ�", name));

            } else
                // û��ƥ��ɹ��ķ����׳�����
                throw new MissingMethodException(string.Format("û�� ��Ӧ������{0}�������ڣ�",
                        name));

        }

        /*
         * ���ڻ����������������Bean���ͷ�����
         * @param cls
         * @return
         * @throws Exception
         */
        public static string[] GetFieldNames(Type cls) {
            return GetFieldNames(cls, 0);
        }

        /*
        * ���ڻ����������������GetBean���ͷ�����
        * @param cls
        * @return
        * @throws Exception
        */
        public static string[] GetGetFieldNames(Type cls) {
            return GetFieldNames(cls, 1);
        }

        /*
         * ���ڻ����������������SetBean���ͷ�����
         * @param cls
         * @return
         * @throws Exception
         */
        public static string[] GetSetFieldNames(Type cls) {
            return GetFieldNames(cls, 2);
        }


        /*
         * ���ڻ����������������SetBean���ͷ�����
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
         * ������յĲ������� �������������ж�������Ե����Եķ������ж�
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
                            "û�� ��Ӧ������{0}�������ڣ�", ToStringValue(name)));
                map.Add("aim", source);
                map.Add("name", props[props.Length - 1]);
                return map;
            } else
                return map;
        }

        /// <summary>
        /// �����޲�������ʵ��
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyName, string typeName) {
            return CreateInstance(assemblyName, typeName, null);
        }

        /// <summary>
        /// �������������ʵ��
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
        /// ģ������޲���Close����Ȼ������Ƿ�IDispose�����������
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
        /// ��ȡ������string���͵���Ŀ
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
