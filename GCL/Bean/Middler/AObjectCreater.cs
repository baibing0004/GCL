using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 主要用于处理以 构造函数/Bean/工厂方式生成对象
    /// </summary>
    public abstract class AObjectCreater {
        private CreaterParameters paras = null;
        private string dll, type;

        static Regex dllRegex = new Regex("((.dll)|(.exe))\\b", RegexOptions.IgnoreCase);
        static Regex dll2Regex = new Regex("(\\|[^,]*)\\b", RegexOptions.IgnoreCase);
        /// <summary>
        /// 装配件名，类名，生成者参数 null或者空装配件名表明已经编译在一起的DLL
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        public AObjectCreater(string dll, string type, CreaterParameters paras) {
            //不处理|这种方式
            this.dll = dll2Regex.Replace(dll, "");
            this.type = type;
            this.paras = paras;

            //进行处理优化
            if (!string.IsNullOrEmpty(dll) && dll.IndexOf(",") > 0) {
                //从“c:/System.Data.dll,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089”中
                //取出这样的部分“System.Data,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089”
                //取得文件目录
                this.dll = null;
                if (this.type.IndexOf(",") < 0) {
                    string[] _name = dll.Split(new char[] { '\\', '/' });
                    this.type = type + "," + dllRegex.Replace(_name[_name.Length - 1], "");
                }
            }
        }

        /// <summary>
        /// null或者空装配件名表明已经编译在一起的DLL
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        public AObjectCreater(string type, CreaterParameters paras)
            : this(null, type, paras) {
        }

        /// <summary>
        /// 获取生成者参数类
        /// </summary>
        /// <returns></returns>
        public CreaterParameters GetCreaterParameters() {
            return this.paras;
        }
        /// <summary>
        /// 获取类的类型
        /// </summary>
        /// <returns></returns>
        public string GetTypeName() {
            return type;
        }

        /// <summary>
        /// 获取装配件名
        /// </summary>
        /// <returns></returns>
        public string GetDll() {
            return dll;
        }
        /// <summary>
        /// 用于生成对象
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public abstract object GetObject();
    }
}
