using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 此类不对外公开，纯属内部类优化
    /// </summary>
    class ObjectStaticFactory {

        private string method, typename;
        private string dll, dllPath, dllFullName, dllPath2;
        //G:/baibing/Work/GCL/Test/bin/Debug/System.Messaging.dll, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
        //分成
        //G:/baibing/Work/GCL/Test/bin/Debug/System.Messaging.dll
        //System.Messaging, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
        public ObjectStaticFactory(string dll, string type) {
            this.dll = dll;
            this.typename = type.Substring(0, type.LastIndexOf("."));
            this.method = type.Substring(type.LastIndexOf(".") + 1);
            this.dllPath = dll2Regex.Replace(dll, "").Split(',')[0];
            //用于装载对象中的类
            this.dllPath2 = dll.IndexOf("|") >= 0 ? dll.Split('|')[1] : null;
            //取得文件目录
            string[] _name = dll2Regex.Replace(dll, "").Split(new char[] { '\\', '/' });
            this.dllFullName = _name[_name.Length - 1].IndexOf("|") < 0 ? dllRegex.Replace(_name[_name.Length - 1], "") : "";
        }
        static Regex dllRegex = new Regex("((.dll)|(.exe))\\b", RegexOptions.IgnoreCase);
        static Regex dll2Regex = new Regex("(\\|[^,]*)\\b", RegexOptions.IgnoreCase);
        private Type staticObject = null;
        private object key = DateTime.Now;
        public Type GetStaticObject() {
            if (staticObject == null)
                lock (key) {
                    if (!string.IsNullOrEmpty(this.dllPath2)) {
                        Assembly.LoadFrom(this.dllPath2);
                    }
                    if (staticObject == null)
                        staticObject = Type.GetType(this.typename + (string.IsNullOrEmpty(this.dllFullName) ? "" : ("," + this.dllFullName)));
                    if (staticObject == null) {
                        Assembly.LoadFrom(this.dllPath);
                        staticObject = Type.GetType(this.typename + (string.IsNullOrEmpty(this.dllFullName) ? "" : ("," + this.dllFullName)));
                    }
                    if (staticObject == null)
                        throw new MiddlerException(this.dll, this.typename + (string.IsNullOrEmpty(this.dllFullName) ? "" : ("," + this.dllFullName)), CreateMethod.Factory, null, "不能找到相关类型", null);

                    //if (staticObject == null) { Assembly.LoadFrom(this.GetDll()) }
                    //staticObject = BeanTool.CreateInstance(this.GetDll(), this.GetTypeName()).GetType();
                }
            return staticObject;
        }

        public object GetObject(object[] paras) {
            //object[] a = this.GetCreaterParameters().GetParameters();
            //因为需要提供Type[]被抛弃
            //return  this.GetStaticType().GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, new Type[] { Type.GetType(type) }, null).Invoke(null, new object[] { value });
            //            return BeanTool.Invoke(GetStaticObject(), method, this.GetCreaterParameters().GetParameters());
            //需要参数为数组
            try {
                var val = GetStaticObject().InvokeMember(method, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, paras);
                if (val == null) throw new Exception("创建对象为空！");
                return val;
            } catch (Exception ex) {
                throw new MiddlerException(this.dll, this.typename + (string.IsNullOrEmpty(this.dllFullName) ? "" : ("," + this.dllFullName)), CreateMethod.Factory, paras, ex);
            }
        }
    }
}
