using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Bean;

namespace GCL.Event {

    public enum EventLevel {
        /// <summary>
        /// ע�ͼ��� �����в�������־��Ϣ�������Ϣ���� ����Գ������в���������Ӱ��
        /// </summary>
        Comment = 0,
        /// <summary>
        /// ��Ҫ���� �¼����߼����߳���������з�����Ӱ��
        /// </summary>
        Importent = 1,
    }

    /// <summary>
    /// �¼������࣬ʹ������洢���ݡ� ֻ��������Խ���ֱ�Ӳ������������ͨ��GetXXX�������ж�ȡ���Ƿ����SetXXX��������Ҫ��
    /// ���鲻����Set������������ͨ��������������������ʵ����Ӧ�Ĳ����� ��д equals ���� Comparator�ӿ� �� Comparable�ӿ�
    /// ֧����Ϊ Tree/Hash �Ĳ��� ֵ��ͬ��EventArg���
    /// ��Ҫ���ڼ̳ж���Ϊ������ʹ�� �����Ϊ������ֱ��ʹ�� �������ز�����Ҫ����GetPara �� SetPara������
    /// �ױ� 2.0.51212.1
    /// </summary>
    public class EventArg : BeanClass, IComparable, IComparer<EventArg>, IDisposable, ICloneable {

        /// <summary>
        /// System.EventHandle��Ĭ��ʵ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        public static void _EventHandleDefault(object sender, EventArgs e) {
        }

        /// <summary>
        /// EventHandle��Ĭ��ʵ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _EventHandleDefault(object sender, EventArg e) {
        }

        /// <summary>
        /// CommonEventHandle��Ĭ��ʵ��
        /// </summary>
        /// <param name="e"></param>
        public static void _CommonEventHandleDefault(EventArg e) {
        }

        /// <summary>
        /// CommandEventHandle��Ĭ��ʵ��
        /// </summary>
        public static void _CommandEventHandleDefault() {
        }

        /// <summary>
        /// EventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(EventHandler handle, object sender, EventArgs e) {
            try {
                handle(sender, e);
            } catch {
            }
        }

        /// <summary>
        /// EventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(EventHandle handle, object sender, EventArg e) {
            try {
                handle(sender, e);
            } catch {
            }
        }

        /// <summary>
        ///  CommonEventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(CommonEventHandle handle, EventArg e) {
            CallCommonEventSafely(handle, e);
        }

        /// <summary>
        /// CommandEventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        public static void CallEventSafely(CommandEventHandle handle) {
            CallCommandEventSafely(handle);
        }


        /// <summary>
        /// DynamicEventFunc�İ�ȫ���ø�����
        /// </summary>
        /// <param name="method"></param>
        /// <param name="p"></param>
        public static void CallEventSafely(DynamicEventFunc method, params object[] p) {
            try {
                method(p);
            } catch {
            }
        }

        /// <summary>
        ///  CommonEventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="e"></param>
        public static void CallCommonEventSafely(CommonEventHandle handle, EventArg e) {
            try {
                handle(e);
            } catch {
            }
        }

        /// <summary>
        /// CommandEventHandle�İ�ȫ���÷���
        /// </summary>
        /// <param name="handle"></param>
        public static void CallCommandEventSafely(CommandEventHandle handle) {
            try {
                handle();
            } catch {
            }
        }

        protected object[] para;

        protected EventLevel level = EventLevel.Comment;

        /// <summary>
        /// ����¼�����
        /// </summary>
        /// <returns></returns>
        public EventLevel GetLevel() {
            return this.level;
        }

        public EventLevel Level {
            get {
                return GetLevel();
            }
        }

        /// <summary>
        /// �¼���� Ϊ-1ʱ˵��δ����
        /// </summary>
        protected int eventNum = -1;

        /// <summary>
        /// ����¼���� Ϊ-1ʱ˵��δ����
        /// </summary>
        /// <returns></returns>
        public int GetEventNumber() {
            return this.eventNum;
        }

        public void SetEventNumber(int num) {
            if (eventNum >= 0)
                throw new InvalidOperationException("�¼����Ѿ�����:" + eventNum);
            else
                this.eventNum = num;
        }

        /// <summary>
        /// ����¼���� Ϊ-1ʱ˵��δ����
        /// </summary>
        public int EventNumber {
            get {
                return GetEventNumber();
            }
            set {
                this.SetEventNumber(value);
            }
        }

        private bool isCancle = false;

        private object changeCancleKey = DateTime.Now;

        private bool changeCancle = false;

        /// <summary>
        ///�����¼��Ƿ����ȡ��
        /// </summary>
        /// <param name="cancle"></param>
        public void SetCancle(bool cancle) {
            lock (changeCancleKey) {
                if (this.changeCancle == false) {
                    this.changeCancle = true;
                    this.isCancle = cancle;
                } else
                    throw new InvalidOperationException("�Ѿ�����ȡ��!");
            }
        }

        /// <summary>
        /// ��ȡ�¼��Ƿ����ȡ��
        /// </summary>
        /// <returns></returns>
        public bool GetCancle() {
            return this.isCancle;
        }

        /// <summary>
        /// �Ƿ����ȡ��
        /// </summary>
        public bool Cancle {
            get {
                return this.GetCancle();
            }
            set {
                this.SetCancle(value);
            }
        }

        /// <summary>
        /// ������ʵ��ֵ ��������������ù�ֵ ��ô��õľ��Ǳ����ù���ֵ ���������ô�����������õ�ֵ �൱������Ĭ��ֵ
        /// </summary>
        /// <param name="rCancle"></param>
        /// <returns></returns>
        public bool GetCancle(bool rCancle) {
            try {
                this.SetCancle(rCancle);
                return rCancle;
            } catch {
                return this.GetCancle();
            }
        }

        protected EventArg(EventArg e)
            : this() {
            this.para = e.para;
            this.level = e.level;
            this.overRide = e.overRide;
            this.eventNum = e.eventNum;
            this.isCancle = e.isCancle;
            this.changeCancle = e.changeCancle;
        }
        /// <summary>
        /// �޲��� �Ҳ�����ı�
        /// </summary>
        public EventArg()
            : this(EventLevel.Comment, -1, null, false) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ����������
        /// </summary>
        /// <param name="size">�¼�������Ŀ</param>
        public EventArg(int size)
            : this(new object[size], true) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ����������
        /// </summary>
        /// <param name="level">�¼�����</param>
        public EventArg(EventLevel level)
            : this(level, -1, null, false) {
        }


        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ���������
        /// </summary>
        /// <param name="obj">�¼�����</param>
        public EventArg(object obj)
            : this(new object[] { obj }) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ���������
        /// </summary>
        /// <param name="objs">�¼�����</param>
        public EventArg(object[] objs)
            : this(objs, false) {
        }



        /// <summary>
        /// ʵ����ص������������ Ĭ�����¼��� �¼�����ΪComment
        /// </summary>
        /// <param name="obj">�¼�����</param>
        /// <param name="allowSet">�Ƿ���������</param>
        public EventArg(object obj, bool allowSet)
            : this(EventLevel.Comment, -1, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�����¼��� �¼�����ΪComment
        /// </summary>
        /// <param name="objs">�¼�����</param>
        /// <param name="allowSet">�Ƿ���������</param>
        public EventArg(object[] objs, bool allowSet)
            : this(EventLevel.Comment, -1, objs, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ���������ò���
        /// </summary>
        /// <param name="eventNum">�¼���</param>
        /// <param name="size">�¼�������С</param>
        public EventArg(int eventNum, int size)
            : this(EventLevel.Comment, eventNum, size) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ��������ò���
        /// </summary>
        /// <param name="eventNum">�¼���</param>
        /// <param name="obj">�¼�����</param>
        public EventArg(int eventNum, object obj)
            : this(eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ��������ò���
        /// </summary>
        /// <param name="eventNum">�¼���</param>
        /// <param name="objs">�¼�����</param>
        public EventArg(int eventNum, object[] objs)
            : this(eventNum, objs, false) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="eventNum">�¼���</param>
        /// <param name="obj">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(int eventNum, object obj, bool allowSet)
            : this(eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="eventNum">�¼���</param>
        /// <param name="objs">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(int eventNum, object[] objs, bool allowSet)
            : this(EventLevel.Comment, eventNum, objs, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ���������ò���
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="size">�¼���Ŀ</param>
        public EventArg(EventLevel level, int size)
            : this(level, -1, size) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ��������ò���
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="objs">�¼�����</param>
        public EventArg(EventLevel level, object obj)
            : this(level, new object[] { obj }, false) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ��������ò���
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="obj">�¼�����</param>
        public EventArg(EventLevel level, object[] objs)
            : this(level, objs, false) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="obj">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(EventLevel level, object obj, bool allowSet)
            : this(level, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="objs">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(EventLevel level, object[] objs, bool allowSet)
            : this(level, -1, objs, allowSet) {
        }


        /// <summary>
        /// ʵ����ص������������ Ĭ�ϲ��������ò���
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="eventNum">�¼���</param>
        /// <param name="obj">�¼�����</param>
        public EventArg(EventLevel level, int eventNum, object obj)
            : this(level, eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="eventNum">�¼���</param>
        /// <param name="obj">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(EventLevel level, int eventNum, object obj, bool allowSet)
            : this(level, eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// ʵ����ص������������ Ĭ�Ͽ�������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="eventNum">�¼���</param>
        /// <param name="size">�¼�������Ŀ</param>
        public EventArg(EventLevel level, int eventNum, int size)
            : this(level, eventNum, new object[size], true) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="eventNum">�¼���</param>
        /// <param name="objs">�¼�����</param>
        public EventArg(EventLevel level, int eventNum, object[] objs)
            : this(level, eventNum, objs, false) {
        }

        /// <summary>
        /// ʵ����ص������������
        /// </summary>
        /// <param name="level">�¼�����</param>
        /// <param name="eventNum">�¼���</param>
        /// <param name="objs">�¼�����</param>
        /// <param name="allowSet">�Ƿ��������ò���</param>
        public EventArg(EventLevel level, int eventNum, object[] objs, bool allowSet) {
            this.level = level;
            this.eventNum = eventNum;
            this.para = objs;
            this.overRide = allowSet;
            this.Init();
        }

        private string[] gpSet;

        private string[] spSet;

        private string[] propertySet;


        /// <summary>
        /// �Ƿ�����ֵ����
        /// </summary>
        private bool overRide = false;

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool AllowOverride() {
            return this.overRide;
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool Override {
            get {
                return AllowOverride();
            }
        }

        /// <summary>
        /// �����������C#ϰ�� ����֧��ʹ�� ��GetParaͬ��
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual object this[int index] {
            get {
                return this.GetPara(index);
            }
            set {
                this.SetPara(index, value);
            }
        }

        /// <summary>
        /// ���鸲��
        /// </summary>
        /// <returns>�������ֵ</returns>
        protected virtual int GetMaxLength() {
            return 0;
        }

        protected virtual void Init() {
            if (!IsEnable(this.para))
                this.SetPara(new object[this.GetMaxLength()]);
            try {
                this.SetDefaultAim(this);
                this.gpSet = this.GetGetFieldNames();
                this.spSet = this.GetSetFieldNames();
                this.propertySet = this.GetFieldNames();
            } catch (Exception e) {
                System.Console.WriteLine(e.StackTrace);
            }
        }

        /*
	 * @return ����Bean���Է�������Ŀ������һ�����Բ����Ƿ���Get/Set����ֻ����һ��
	 */
        public int GetPropertyLength() {
            return this.propertySet.Length;
        }

        /*
         * @return ����Bean���Է�������Ŀ������һ�����Բ����Ƿ���Get/Set����ֻ����һ��
         */
        public int GetGetPropertyLength() {
            return this.gpSet.Length;
        }

        /*
         * @return ����Bean���Է�������Ŀ������һ�����Բ����Ƿ���Get/Set����ֻ����һ��
         */
        public int GetSetPropertyLength() {
            return this.spSet.Length;
        }

        /*
         * �����ƶ�ID����������
         * 
         * @param id
         *            �ƶ�ID
         * @return
         */
        public string GetPropertyName(int id) {
            return this.propertySet[id];
        }

        /*
         * �����ƶ�ID����������
         * 
         * @param id
         *            �ƶ�ID
         * @return
         */
        public string GetGetPropertyName(int id) {
            return this.gpSet[id];
        }

        /*
         * �����ƶ�ID����������
         * 
         * @param id
         *            �ƶ�ID
         * @return
         */
        public string GetSetPropertyName(int id) {
            return this.spSet[id];
        }

        private bool ContainProperty(string[] source, string name) {
            for (int w = 0; w < source.Length; w++) {
                if (source[w].Trim().Equals(name))
                    return true;
            }
            return false;
        }

        public bool ContainProperty(string name) {
            return this.ContainProperty(this.propertySet, name);
        }

        public bool ContainSetProperty(string name) {
            return this.ContainProperty(this.spSet, name);
        }

        public bool ContainGetProperty(string name) {
            return this.ContainProperty(this.gpSet, name);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param paraTypes
         *            ��������
         * @param paras
         *            ����
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object GetPropertyValue(int id, Type[] paraTypes, object[] paras) {
            return base.GetPropertyValue(this.gpSet[id], paraTypes, paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param paraType
         *            ��������
         * @param para
         *            ����
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object GetPropertyValueSP(int id, Type paraType, object para) {
            return base.GetPropertyValueSP(this.gpSet[id], paraType, para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object GetPropertyValue(int id) {
            return base.GetPropertyValue(this.gpSet[id],
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param paraTypes
         *            ��������
         * @param paras
         *            ����ֵ
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object SetPropertyValue(int id, Type[] paraTypes, object[] paras) {
            return base.SetPropertyValue(this.spSet[id], paraTypes, paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param paraType
         *            ��������
         * @param para
         *            ����ֵ
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object SetPropertyValueSP(int id, Type paraType, object para) {
            return base.SetPropertyValueSP(this.spSet[id], paraType, para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param paras
         *            ����ֵ
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object SetPropertyValue(int id, object[] paras) {
            return base.SetPropertyValue(this.spSet[id], paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * ����ָ��ID������ֵ
         * 
         * @param id
         *            �ƶ���ID����
         * @param para
         *            ����ֵ
         * @return ����ֵ
         * @throws Exception
         *             ���п��ܷ����Ĵ���
         */
        public object SetPropertyValueSP(int id, object para) {
            return base.SetPropertyValueSP(this.spSet[id], para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /// <summary>
        /// ���ڼ̳��������³�ʼ��������
        /// </summary>
        /// <param name="paras"></param>
        protected void SetPara(object[] paras) {
            this.para = paras;
        }

        /// <summary>
        /// �����ƶ�λ�û�ȡ����
        /// </summary>
        /// <param name="id">��0��ʼ</param>
        /// <returns>����ID �����ƶ��Ĳ���ֵ</returns>
        public virtual object GetPara(int id) {
            if (this.GetLength() == -1 || id >= this.GetLength() || id < 0)
                throw new IndexOutOfRangeException("���󣡳����߽�ֵ");
            return this.para[id];
        }

        /// <summary>
        /// ���ò���������Ӧλ�õ�ֵ
        /// </summary>
        /// <param name="id">����λ�ò�������0��ʼ��</param>
        /// <param name="data">���������ֵ</param>
        /// <returns>���λ�õ�ԭ������</returns>
        /// <exception>�κο��ܵĴ��󣨳����߽磩</exception>
        public virtual object SetPara(int id, object data) {
            lock (this) {
                if (this.GetLength() == -1 || id >= this.GetLength() || id < 0)
                    throw new IndexOutOfRangeException("���󣡳����߽�ֵ");

                if (!this.overRide)
                    throw new InvalidOperationException("���󣡲���������ֵ");
                else {
                    object _object = this.para[id];
                    this.para[id] = data;
                    return _object;
                }
            }
        }

        /// <summary>
        /// �������Super��MAXLENGTHλ�ò�������0��ʼ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToStringOfPara(int id) {
            return ToStringValue(GetPara(id));
        }

        /// <summary>
        /// ���ز������鳤�� ���Ϊ���򷵻�-1
        /// </summary>
        public int Length {
            get {
                return GetLength();
            }
        }
        /// <summary>
        /// ���ز������鳤�� ���Ϊ���򷵻�-1
        /// </summary>
        /// <returns></returns>
        public int GetLength() {
            if (this.para == null)
                return -1;
            return this.para.Length;
        }

        /// <summary>
        /// ���ز��������е�ֵ�Ƿ�Ϊnull�����Ǹ�������Ĵ�С���жϵģ�
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty() {
            for (int w = 0; w < this.GetLength(); w++)
                if (this.para[w] != null)
                    return false;
            return true;
        }

        /// <summary>
        /// ��дֵ�Ƿ���ȵķ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>����ֵ�Ƿ��������Ĳ���</returns>
        public override bool Equals(object obj) {
            if (!(obj is EventArg))
                return false;
            return this.Equals((EventArg)obj);
        }

        /// <summary>
        /// ��дֵ�Ƿ���ȵķ���
        /// </summary>
        /// <param name="obj">�ԱȲ���</param>
        /// <returns>����ֵ�Ƿ��������Ĳ���</returns>
        public bool Equals(EventArg obj) {
            return Array.Equals(this.para, obj.para);
        }

        /// <summary>
        /// ����Hash�㷨������ԱHash�㷨�ļӷ���������
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            lock (para) {
                int _result = this.para.Length;
                for (int w = 0; w < this.para.Length; w++)
                    _result = (_result + this.para[w].GetHashCode())
                            % int.MaxValue;
                return _result;
            }
        }


        /*
         * @see java.lang.object#hashCode()
         */
        public virtual int SuperGetHashCode() {
            return base.GetHashCode();
        }

        #region IComparable ����

        /// <summary>
        /// ʵ��IComparable�еķ���ʵ�ֶ���һ������ıȽ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj) {
            if (!(obj is EventArg))
                return 1;
            return CompareTo((EventArg)obj);


        }

        #endregion

        /// <summary>
        ///  ���ж��Ƿ����Ȼ�� ����hashCodeֵ�жϴ�С
        /// </summary>
        /// <param name="o"></param>
        /// <returns>�Ƿ�����������ʹ��hashCode�ж�</returns>
        public int CompareTo(EventArg obj) {
            if (Equals(obj))
                return 0;
            return this.GetHashCode() - obj.GetHashCode();
        }


        #region IComparer<EventArg> ����

        /// <summary>
        /// ʵ��IComparer EventArg �ӿڵķ��� �Ƚ�2��EventArg���͵Ĵ�С
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(EventArg x, EventArg y) {
            return x.CompareTo(y);
        }

        #endregion

        /// <summary>
        /// ����IComparer�ӿڵķ��� �Ƚ�2��object���͵Ĵ�С
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y) {
            bool o1IsSon = x is EventArg;
            bool o2IsSon = y is EventArg;
            if (o1IsSon && o2IsSon)
                return this.Compare((EventArg)x, (EventArg)y);
            else if (o1IsSon)
                return 1;
            else if (o2IsSon)
                return -1;
            else
                return x.GetHashCode() - y.GetHashCode();
        }

        #region IDisposable Members
        public virtual void Dispose() {            
            this.para = null;
        }
        #endregion

        ~EventArg() {
            this.Dispose();
        }

        #region ICloneable Members

        /// <summary>
        /// ���ö�����޲������캯�������µĶ���
        /// </summary>
        /// <returns></returns>
        protected virtual EventArg CreateNewInstance() {
            return (EventArg)Activator.CreateInstance(this.GetType());
        }

        /// <summary>
        /// �����ʹ��Clone������������һ���ղ������캯���򸲸�CreateNewInstance�������߸��Ǳ�����
        /// </summary>
        /// <returns></returns>
        public virtual object Clone() {

            EventArg _result = this.CreateNewInstance();

            _result.level = this.GetLevel();
            _result.SetEventNumber(this.GetEventNumber());
            _result.overRide = this.AllowOverride();

            if (IsEnable(this.para)) {
                if (IsEnable(_result.para) || _result.para.Length != this.para.Length)
                    _result.para = new object[para.Length];
                Array.Copy(this.para, _result.para, this.para.Length);
            } else
                _result.para = null;

            this.Transport(_result);
            return _result;
        }

        #endregion
    }
}
