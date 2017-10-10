using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using GCL.Common;

namespace GCL.Bean {
    public class BeanClass : BeanTool {
        private object defaultAim;
        public object GetDefaultAim() {
            return defaultAim;
        }

        public BeanClass(object aimobject) {
            this.defaultAim = aimobject;
        }
        protected BeanClass() {
        }

        protected void SetDefaultAim(Object a) {
            this.defaultAim = a;
        }

        /*
         * <P>
         * 用于bean的Get/set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的set方法 全部都匹配
         * 
         * @param this.defaultAim
         *            源bean对象
         * @param aim
         *            目的bean对象
         */
        public void Transport(object aim) {
            Transport(this.defaultAim, aim);
        }

        /*
        * <P>
        * 用于bean的Get/set方法之间互相赋值
        * <p>
        * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的set方法 全部都匹配
        * 
        * @param this.defaultAim
        *            源bean对象
        * @param aim
        *            目的bean对象
        */
        public void Transport(object aim, BeanBandingFlags flag) {
            Transport(this.defaultAim, aim, flag);
        }

        /*
         * <P>
         * 用于bean的Get/Set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的Set方法
         * 
         * @param this.defaultAim
         *            源bean对象
         * @param aim
         *            目的bean对象
         * @param passPro
         *            许可的属性
         * @param forPro
         *            不许可的属性
         */
        public void Transport(object aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            Transport(this.defaultAim, aim, passPro, forPro);
        }

        /*
         * <P>
         * 用于bean的Get/Set方法之间互相赋值
         * <p>
         * 用于将第一个bean对象的Get方法获得的结果赋值到第二个bean对象的Set方法
         * 
         * @param this.defaultAim
         *            源bean对象
         * @param aim
         *            目的bean对象
         * @param passPro
         *            许可的属性
         * @param forPro
         *            不许可的属性
         */
        public void Transport(object aim, ICollection<object> passPro,
                ICollection<object> forPro, BeanBandingFlags flag) {
            Transport(this.defaultAim, aim, passPro, forPro, flag);
        }



        /// <summary>
        /// 尝试使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                 object para) {
            return SetPropertyValueSP(this.defaultAim, name, para);
        }

        /// <summary>
        /// 使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                 object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(this.defaultAim, name,
                         para, flag);

        }

        /// <summary>
        /// 尝试所有类型方法进行操作设置属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                 object[] paras) {
            return SetPropertyValue(this.defaultAim, name, paras);
        }

        /// <summary>
        /// 进行操作设置属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                 object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(this.defaultAim, name, paras, flag);
        }

        /// <summary>
        /// 尝试使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                Type paraType, object para) {
            return SetPropertyValueSP(this.defaultAim, name, paraType, para);
        }

        /// <summary>
        /// 使用单参数值设置属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                Type paraType, object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(this.defaultAim, name, paraType, para, flag);
        }

        /// <summary>
        /// 尝试所有类型方法进行操作设置属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                Type[] paraTypes, object[] paras) {
            return SetPropertyValue(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// 进行操作设置属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(this.defaultAim, name, paraTypes, paras, flag);
        }

        /// <summary>
        /// 尝试所有方式获得无参属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object GetPropertyValue(string name) {
            return GetPropertyValue(this.defaultAim, name);
        }

        /// <summary>
        /// 获得无参属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object GetPropertyValue(string name, BeanBandingFlags flag) {
            return GetPropertyValue(this.defaultAim, name, flag);
        }

        /// <summary>
        /// 尝试使用单参数值获得属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object GetPropertyValueSP(string name,
                Type paraType, object para) {
            return GetPropertyValueSP(this.defaultAim, name, paraType, para);
        }

        /// <summary>
        /// 使用单参数值获得属性值
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraType">参数类型 属性类型</param>
        /// <param name="para">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object GetPropertyValueSP(string name,
                Type paraType, object para, BeanBandingFlags flag) {
            return GetPropertyValueSP(this.defaultAim, name, paraType, para, flag);
        }

        /// <summary>
        /// 尝试所有类型方法进行操作获得属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <returns></returns>
        public object GetPropertyValue(string name,
                Type[] paraTypes, object[] paras) {
            return GetPropertyValue(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// 进行操作获得属性值
        /// </summary>
        /// <param name="this.defaultAim">调用对象</param>
        /// <param name="name">方法名 属性</param>
        /// <param name="paraTypes">参数类型 属性类型</param>
        /// <param name="paras">属性值</param>
        /// <param name="flag">尝试那些类型</param>
        /// <returns></returns>
        public object GetPropertyValue(string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return GetPropertyValue(this.defaultAim, name, paraTypes, paras, flag);
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
        public object Invoke(string name) {
            return Invoke(this.defaultAim, name);
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
        public object InvokeSP(string name, Type paraType,
                object para) {
            return InvokeSP(this.defaultAim, name, paraType, para);
        }

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
        public object Invoke(string name, Type[] paraTypes,
                object[] paras) {
            return Invoke(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// 通过对象参数模糊确定方法并调用返回结果
        /// </summary>
        /// <param name="aim">调用对象</param>
        /// <param name="name">方法名 属性名称 开头字母不大写</param>
        /// <param name="para">属性值</param>
        /// <returns></returns>
        public object Invoke(string name, params object[] para) {
            return Invoke(this.defaultAim, name, para);
        }

        /// <summary>
        /// 获得多少个Bean方法的属性方法
        /// </summary>
        /// <returns></returns>
        public string[] GetFieldNames() {
            return GetFieldNames(this.defaultAim.GetType());
        }

        /// <summary>
        /// 获得多少个Bean方法的属性方法
        /// </summary>
        /// <returns></returns>
        public string[] GetGetFieldNames() {
            return GetGetFieldNames(this.defaultAim.GetType());
        }

        /// <summary>
        /// 获得多少个Bean方法的属性方法
        /// </summary>
        /// <returns></returns>
        public string[] GetSetFieldNames() {
            return GetSetFieldNames(this.defaultAim.GetType());
        }
    }
}
