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
         * ����bean��Get/set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����set���� ȫ����ƥ��
         * 
         * @param this.defaultAim
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         */
        public void Transport(object aim) {
            Transport(this.defaultAim, aim);
        }

        /*
        * <P>
        * ����bean��Get/set����֮�以�ำֵ
        * <p>
        * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����set���� ȫ����ƥ��
        * 
        * @param this.defaultAim
        *            Դbean����
        * @param aim
        *            Ŀ��bean����
        */
        public void Transport(object aim, BeanBandingFlags flag) {
            Transport(this.defaultAim, aim, flag);
        }

        /*
         * <P>
         * ����bean��Get/Set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����Set����
         * 
         * @param this.defaultAim
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         * @param passPro
         *            ��ɵ�����
         * @param forPro
         *            ����ɵ�����
         */
        public void Transport(object aim, ICollection<object> passPro,
                ICollection<object> forPro) {
            Transport(this.defaultAim, aim, passPro, forPro);
        }

        /*
         * <P>
         * ����bean��Get/Set����֮�以�ำֵ
         * <p>
         * ���ڽ���һ��bean�����Get������õĽ����ֵ���ڶ���bean�����Set����
         * 
         * @param this.defaultAim
         *            Դbean����
         * @param aim
         *            Ŀ��bean����
         * @param passPro
         *            ��ɵ�����
         * @param forPro
         *            ����ɵ�����
         */
        public void Transport(object aim, ICollection<object> passPro,
                ICollection<object> forPro, BeanBandingFlags flag) {
            Transport(this.defaultAim, aim, passPro, forPro, flag);
        }



        /// <summary>
        /// ����ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                 object para) {
            return SetPropertyValueSP(this.defaultAim, name, para);
        }

        /// <summary>
        /// ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                 object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(this.defaultAim, name,
                         para, flag);

        }

        /// <summary>
        /// �����������ͷ������в�����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                 object[] paras) {
            return SetPropertyValue(this.defaultAim, name, paras);
        }

        /// <summary>
        /// ���в�����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                 object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(this.defaultAim, name, paras, flag);
        }

        /// <summary>
        /// ����ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                Type paraType, object para) {
            return SetPropertyValueSP(this.defaultAim, name, paraType, para);
        }

        /// <summary>
        /// ʹ�õ�����ֵ��������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValueSP(string name,
                Type paraType, object para, BeanBandingFlags flag) {
            return SetPropertyValueSP(this.defaultAim, name, paraType, para, flag);
        }

        /// <summary>
        /// �����������ͷ������в�����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                Type[] paraTypes, object[] paras) {
            return SetPropertyValue(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// ���в�����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object SetPropertyValue(string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return SetPropertyValue(this.defaultAim, name, paraTypes, paras, flag);
        }

        /// <summary>
        /// �������з�ʽ����޲�����ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object GetPropertyValue(string name) {
            return GetPropertyValue(this.defaultAim, name);
        }

        /// <summary>
        /// ����޲�����ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object GetPropertyValue(string name, BeanBandingFlags flag) {
            return GetPropertyValue(this.defaultAim, name, flag);
        }

        /// <summary>
        /// ����ʹ�õ�����ֵ�������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object GetPropertyValueSP(string name,
                Type paraType, object para) {
            return GetPropertyValueSP(this.defaultAim, name, paraType, para);
        }

        /// <summary>
        /// ʹ�õ�����ֵ�������ֵ
        /// </summary>
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraType">�������� ��������</param>
        /// <param name="para">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object GetPropertyValueSP(string name,
                Type paraType, object para, BeanBandingFlags flag) {
            return GetPropertyValueSP(this.defaultAim, name, paraType, para, flag);
        }

        /// <summary>
        /// �����������ͷ������в����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <returns></returns>
        public object GetPropertyValue(string name,
                Type[] paraTypes, object[] paras) {
            return GetPropertyValue(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// ���в����������ֵ
        /// </summary>
        /// <param name="this.defaultAim">���ö���</param>
        /// <param name="name">������ ����</param>
        /// <param name="paraTypes">�������� ��������</param>
        /// <param name="paras">����ֵ</param>
        /// <param name="flag">������Щ����</param>
        /// <returns></returns>
        public object GetPropertyValue(string name,
                Type[] paraTypes, object[] paras, BeanBandingFlags flag) {
            return GetPropertyValue(this.defaultAim, name, paraTypes, paras, flag);
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
        public object Invoke(string name) {
            return Invoke(this.defaultAim, name);
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
        public object InvokeSP(string name, Type paraType,
                object para) {
            return InvokeSP(this.defaultAim, name, paraType, para);
        }

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
        public object Invoke(string name, Type[] paraTypes,
                object[] paras) {
            return Invoke(this.defaultAim, name, paraTypes, paras);
        }

        /// <summary>
        /// ͨ���������ģ��ȷ�����������÷��ؽ��
        /// </summary>
        /// <param name="aim">���ö���</param>
        /// <param name="name">������ �������� ��ͷ��ĸ����д</param>
        /// <param name="para">����ֵ</param>
        /// <returns></returns>
        public object Invoke(string name, params object[] para) {
            return Invoke(this.defaultAim, name, para);
        }

        /// <summary>
        /// ��ö��ٸ�Bean���������Է���
        /// </summary>
        /// <returns></returns>
        public string[] GetFieldNames() {
            return GetFieldNames(this.defaultAim.GetType());
        }

        /// <summary>
        /// ��ö��ٸ�Bean���������Է���
        /// </summary>
        /// <returns></returns>
        public string[] GetGetFieldNames() {
            return GetGetFieldNames(this.defaultAim.GetType());
        }

        /// <summary>
        /// ��ö��ٸ�Bean���������Է���
        /// </summary>
        /// <returns></returns>
        public string[] GetSetFieldNames() {
            return GetSetFieldNames(this.defaultAim.GetType());
        }
    }
}
