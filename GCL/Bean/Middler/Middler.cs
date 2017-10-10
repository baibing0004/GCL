using System;
using System.Collections.Generic;
using System.Text;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 中介者，其类方法为默认调用Config
    /// </summary>
    public sealed class Middler : IDisposable {
        private ConfigManager ma = null;
        public Middler(ConfigManager ma) {
            this.ma = ma;
        }

        static Middler() {
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 根据应用域名与对象名获取对象实例
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetObjectByAppName(string app, string name) {
            return ma.GetConfig(MiddlerConfig.NodeName).GetValue(new NameMiddlerGetParameter(app, name));
        }

        /// <summary>
        /// 根据应用域名与对象名获取对象实例
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public E GetObjectByAppName<E>(string app, string name) {
            return (E)GetObjectByAppName(app, name);
        }

        /// <summary>
        /// 根据应用域名与对象组名获取对象实例组
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public E[] GetObjectsByAppName<E>(string app, string name) {
            var objs = GetObjectsByAppName(app,name);
            E[] rets = new E[objs.Length];
            objs.CopyTo(rets, 0);
            return rets;
        }

        /// <summary>
        /// 根据应用域名与对象组名获取对象实例组
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object[] GetObjectsByAppName(string app, string name) {
            return ma.GetConfig(MiddlerConfig.NodeName).GetValue(new NameMiddlerGetParameter(app, name)) as object[];
        }

        /// <summary>
        /// 根据应用域名与父类类型获取对象实例组
        /// 强烈不推荐使用此方法
        /// </summary>
        /// <param name="app"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object[] GetObjectsByAppType(string app, Type type) {
            return ma.GetConfig(MiddlerConfig.NodeName).GetValue(new TypeMiddlerGetParameter(app, type)) as object[];
        }

        /// <summary>
        /// 根据应用域名与父类类型获取对象实例组
        /// 强烈不推荐使用此方法
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="app"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public E[] GetObjectsByAppType<E>(string app, Type type) {
            return (E[])ma.GetConfig(MiddlerConfig.NodeName).GetValue(new TypeMiddlerGetParameter(app, type));
        }

        /// <summary>
        /// 根据应用域名与对象名返回对象实例
        /// 对于Instance方式和Pool方式的设置比较有效！
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public void SetObjectByAppName(string app, string name, object v) {
            ma.GetConfig(MiddlerConfig.NodeName).SetValue(new MiddlerSetParameter(app, name), v);
        }

        #region IDisposable Members

        public void Dispose() {
            this.ma.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// 调用默认Config提供的Middler
    /// </summary>
    public sealed class Middlement {

        static Middlement() {
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
        /// <summary>
        /// 根据应用域名与对象名获取对象实例
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetObjectByAppName(string app, string name) {
            return ConfigManagement.GetConfig(MiddlerConfig.NodeName).GetValue(new NameMiddlerGetParameter(app, name));
        }

        /// <summary>
        /// 根据应用域名与对象名获取对象实例
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static E GetObjectByAppName<E>(string app, string name) {
            return (E)ConfigManagement.GetConfig(MiddlerConfig.NodeName).GetValue(new NameMiddlerGetParameter(app, name));
        }

        /// <summary>
        /// 根据应用域名与对象组名获取对象实例组
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object[] GetObjectsByAppName(string app, string name) {
            return ConfigManagement.GetConfig(MiddlerConfig.NodeName).GetValue(new NameMiddlerGetParameter(app, name)) as object[];
        }

        /// <summary>
        /// 根据应用域名与父类类型获取对象实例组
        /// 强烈不推荐使用此方法
        /// </summary>
        /// <param name="app"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object[] GetObjectsByAppType(string app, Type type) {
            return ConfigManagement.GetConfig(MiddlerConfig.NodeName).GetValue(new TypeMiddlerGetParameter(app, type)) as object[];
        }

        /// <summary>
        /// 根据应用域名与对象名返回对象实例
        /// 对于Instance方式和Pool方式的设置比较有效！
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static void SetObjectByAppName(string app, string name, object v) {
            ConfigManagement.GetConfig(MiddlerConfig.NodeName).SetValue(new MiddlerSetParameter(app, name), v);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public static void Dispose() {
            ConfigManagement.Dispose();
        }
    }
}
