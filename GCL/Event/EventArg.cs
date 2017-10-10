using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;
using GCL.Bean;

namespace GCL.Event {

    public enum EventLevel {
        /// <summary>
        /// 注释级别 运行中产生的日志信息或调试信息级别 不会对程序运行产生方向性影响
        /// </summary>
        Comment = 0,
        /// <summary>
        /// 重要级别 事件对逻辑或者程序的运行有方向性影响
        /// </summary>
        Importent = 1,
    }

    /// <summary>
    /// 事件参数类，使用数组存储数据。 只有子类可以进行直接操作，否则必须通过GetXXX方法进行读取，是否可以SetXXX视子类需要。
    /// 建议不开发Set操作。而可以通过加入对象而不是数据来实现相应的操作！ 重写 equals 方法 Comparator接口 和 Comparable接口
    /// 支持作为 Tree/Hash 的操作 值相同的EventArg相等
    /// 主要用于继承而作为参数类使用 如果作为参数类直接使用 建议重载并视需要开放GetPara 与 SetPara方法。
    /// 白冰 2.0.51212.1
    /// </summary>
    public class EventArg : BeanClass, IComparable, IComparer<EventArg>, IDisposable, ICloneable {

        /// <summary>
        /// System.EventHandle的默认实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        public static void _EventHandleDefault(object sender, EventArgs e) {
        }

        /// <summary>
        /// EventHandle的默认实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void _EventHandleDefault(object sender, EventArg e) {
        }

        /// <summary>
        /// CommonEventHandle的默认实现
        /// </summary>
        /// <param name="e"></param>
        public static void _CommonEventHandleDefault(EventArg e) {
        }

        /// <summary>
        /// CommandEventHandle的默认实现
        /// </summary>
        public static void _CommandEventHandleDefault() {
        }

        /// <summary>
        /// EventHandle的安全调用方法
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
        /// EventHandle的安全调用方法
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
        ///  CommonEventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="e"></param>
        public static void CallEventSafely(CommonEventHandle handle, EventArg e) {
            CallCommonEventSafely(handle, e);
        }

        /// <summary>
        /// CommandEventHandle的安全调用方法
        /// </summary>
        /// <param name="handle"></param>
        public static void CallEventSafely(CommandEventHandle handle) {
            CallCommandEventSafely(handle);
        }


        /// <summary>
        /// DynamicEventFunc的安全调用个方法
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
        ///  CommonEventHandle的安全调用方法
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
        /// CommandEventHandle的安全调用方法
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
        /// 获得事件级别
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
        /// 事件编号 为-1时说明未定义
        /// </summary>
        protected int eventNum = -1;

        /// <summary>
        /// 获得事件编号 为-1时说明未定义
        /// </summary>
        /// <returns></returns>
        public int GetEventNumber() {
            return this.eventNum;
        }

        public void SetEventNumber(int num) {
            if (eventNum >= 0)
                throw new InvalidOperationException("事件号已经定义:" + eventNum);
            else
                this.eventNum = num;
        }

        /// <summary>
        /// 获得事件编号 为-1时说明未定义
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
        ///设置事件是否可以取消
        /// </summary>
        /// <param name="cancle"></param>
        public void SetCancle(bool cancle) {
            lock (changeCancleKey) {
                if (this.changeCancle == false) {
                    this.changeCancle = true;
                    this.isCancle = cancle;
                } else
                    throw new InvalidOperationException("已经不能取消!");
            }
        }

        /// <summary>
        /// 获取事件是否可以取消
        /// </summary>
        /// <returns></returns>
        public bool GetCancle() {
            return this.isCancle;
        }

        /// <summary>
        /// 是否可以取消
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
        /// 返回真实的值 如果曾经被人设置过值 那么获得的就是被设置过的值 如果不是那么获得是这次设置的值 相当于设置默认值
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
        /// 无参数 且不允许改变
        /// </summary>
        public EventArg()
            : this(EventLevel.Comment, -1, null, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置
        /// </summary>
        /// <param name="size">事件参数数目</param>
        public EventArg(int size)
            : this(new object[size], true) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置
        /// </summary>
        /// <param name="level">事件级别</param>
        public EventArg(EventLevel level)
            : this(level, -1, null, false) {
        }


        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置
        /// </summary>
        /// <param name="obj">事件参数</param>
        public EventArg(object obj)
            : this(new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置
        /// </summary>
        /// <param name="objs">事件参数</param>
        public EventArg(object[] objs)
            : this(objs, false) {
        }



        /// <summary>
        /// 实现相关的数组参数设置 默认无事件号 事件级别为Comment
        /// </summary>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置</param>
        public EventArg(object obj, bool allowSet)
            : this(EventLevel.Comment, -1, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认无事件号 事件级别为Comment
        /// </summary>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置</param>
        public EventArg(object[] objs, bool allowSet)
            : this(EventLevel.Comment, -1, objs, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="size">事件参数大小</param>
        public EventArg(int eventNum, int size)
            : this(EventLevel.Comment, eventNum, size) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        public EventArg(int eventNum, object obj)
            : this(eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        public EventArg(int eventNum, object[] objs)
            : this(eventNum, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public EventArg(int eventNum, object obj, bool allowSet)
            : this(eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public EventArg(int eventNum, object[] objs, bool allowSet)
            : this(EventLevel.Comment, eventNum, objs, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="size">事件数目</param>
        public EventArg(EventLevel level, int size)
            : this(level, -1, size) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="objs">事件参数</param>
        public EventArg(EventLevel level, object obj)
            : this(level, new object[] { obj }, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="obj">事件参数</param>
        public EventArg(EventLevel level, object[] objs)
            : this(level, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public EventArg(EventLevel level, object obj, bool allowSet)
            : this(level, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public EventArg(EventLevel level, object[] objs, bool allowSet)
            : this(level, -1, objs, allowSet) {
        }


        /// <summary>
        /// 实现相关的数组参数设置 默认不允许设置参数
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        public EventArg(EventLevel level, int eventNum, object obj)
            : this(level, eventNum, new object[] { obj }) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="obj">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
        public EventArg(EventLevel level, int eventNum, object obj, bool allowSet)
            : this(level, eventNum, new object[] { obj }, allowSet) {
        }

        /// <summary>
        /// 实现相关的数组参数设置 默认可以设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="size">事件参数数目</param>
        public EventArg(EventLevel level, int eventNum, int size)
            : this(level, eventNum, new object[size], true) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        public EventArg(EventLevel level, int eventNum, object[] objs)
            : this(level, eventNum, objs, false) {
        }

        /// <summary>
        /// 实现相关的数组参数设置
        /// </summary>
        /// <param name="level">事件级别</param>
        /// <param name="eventNum">事件号</param>
        /// <param name="objs">事件参数</param>
        /// <param name="allowSet">是否允许设置参数</param>
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
        /// 是否允许赋值操作
        /// </summary>
        private bool overRide = false;

        /// <summary>
        /// 是否允许覆盖
        /// </summary>
        public bool AllowOverride() {
            return this.overRide;
        }

        /// <summary>
        /// 是否允许覆盖
        /// </summary>
        public bool Override {
            get {
                return AllowOverride();
            }
        }

        /// <summary>
        /// 这个属性满足C#习惯 但不支持使用 与GetPara同意
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
        /// 建议覆盖
        /// </summary>
        /// <returns>返回最大值</returns>
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
	 * @return 返回Bean属性方法的数目，而且一个属性不管是否有Get/Set，都只出现一次
	 */
        public int GetPropertyLength() {
            return this.propertySet.Length;
        }

        /*
         * @return 返回Bean属性方法的数目，而且一个属性不管是否有Get/Set，都只出现一次
         */
        public int GetGetPropertyLength() {
            return this.gpSet.Length;
        }

        /*
         * @return 返回Bean属性方法的数目，而且一个属性不管是否有Get/Set，都只出现一次
         */
        public int GetSetPropertyLength() {
            return this.spSet.Length;
        }

        /*
         * 返回制定ID的属性名称
         * 
         * @param id
         *            制定ID
         * @return
         */
        public string GetPropertyName(int id) {
            return this.propertySet[id];
        }

        /*
         * 返回制定ID的属性名称
         * 
         * @param id
         *            制定ID
         * @return
         */
        public string GetGetPropertyName(int id) {
            return this.gpSet[id];
        }

        /*
         * 返回制定ID的属性名称
         * 
         * @param id
         *            制定ID
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
         * 返回指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param paraTypes
         *            参数类型
         * @param paras
         *            参数
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object GetPropertyValue(int id, Type[] paraTypes, object[] paras) {
            return base.GetPropertyValue(this.gpSet[id], paraTypes, paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 返回指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param paraType
         *            参数类型
         * @param para
         *            参数
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object GetPropertyValueSP(int id, Type paraType, object para) {
            return base.GetPropertyValueSP(this.gpSet[id], paraType, para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 返回指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object GetPropertyValue(int id) {
            return base.GetPropertyValue(this.gpSet[id],
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 设置指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param paraTypes
         *            参数类型
         * @param paras
         *            参数值
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object SetPropertyValue(int id, Type[] paraTypes, object[] paras) {
            return base.SetPropertyValue(this.spSet[id], paraTypes, paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 设置指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param paraType
         *            参数类型
         * @param para
         *            参数值
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object SetPropertyValueSP(int id, Type paraType, object para) {
            return base.SetPropertyValueSP(this.spSet[id], paraType, para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 设置指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param paras
         *            参数值
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object SetPropertyValue(int id, object[] paras) {
            return base.SetPropertyValue(this.spSet[id], paras,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /*
         * 设置指定ID的属性值
         * 
         * @param id
         *            制定的ID参数
         * @param para
         *            参数值
         * @return 属性值
         * @throws Exception
         *             所有可能发生的错误
         */
        public object SetPropertyValueSP(int id, object para) {
            return base.SetPropertyValueSP(this.spSet[id], para,
                    BeanBandingFlags.BeanMethod | BeanBandingFlags.Property);
        }

        /// <summary>
        /// 用于继承子类重新初始化参数类
        /// </summary>
        /// <param name="paras"></param>
        protected void SetPara(object[] paras) {
            this.para = paras;
        }

        /// <summary>
        /// 根据制定位置获取参数
        /// </summary>
        /// <param name="id">从0开始</param>
        /// <returns>根据ID 返回制定的参数值</returns>
        public virtual object GetPara(int id) {
            if (this.GetLength() == -1 || id >= this.GetLength() || id < 0)
                throw new IndexOutOfRangeException("错误！超出边界值");
            return this.para[id];
        }

        /// <summary>
        /// 设置参数数组相应位置的值
        /// </summary>
        /// <param name="id">数组位置参数（从0开始）</param>
        /// <param name="data">具体的数据值</param>
        /// <returns>这个位置的原有数据</returns>
        /// <exception>任何可能的错误（超出边界）</exception>
        public virtual object SetPara(int id, object data) {
            lock (this) {
                if (this.GetLength() == -1 || id >= this.GetLength() || id < 0)
                    throw new IndexOutOfRangeException("错误！超出边界值");

                if (!this.overRide)
                    throw new InvalidOperationException("错误！不允许设置值");
                else {
                    object _object = this.para[id];
                    this.para[id] = data;
                    return _object;
                }
            }
        }

        /// <summary>
        /// 数组相对Super的MAXLENGTH位置参数（从0开始）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToStringOfPara(int id) {
            return ToStringValue(GetPara(id));
        }

        /// <summary>
        /// 返回参数数组长度 如果为空则返回-1
        /// </summary>
        public int Length {
            get {
                return GetLength();
            }
        }
        /// <summary>
        /// 返回参数数组长度 如果为空则返回-1
        /// </summary>
        /// <returns></returns>
        public int GetLength() {
            if (this.para == null)
                return -1;
            return this.para.Length;
        }

        /// <summary>
        /// 返回参数数组中的值是否都为null而不是根据数组的大小来判断的！
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty() {
            for (int w = 0; w < this.GetLength(); w++)
                if (this.para[w] != null)
                    return false;
            return true;
        }

        /// <summary>
        /// 重写值是否相等的方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回值是否等于输入的参数</returns>
        public override bool Equals(object obj) {
            if (!(obj is EventArg))
                return false;
            return this.Equals((EventArg)obj);
        }

        /// <summary>
        /// 重写值是否相等的方法
        /// </summary>
        /// <param name="obj">对比参数</param>
        /// <returns>返回值是否等于输入的参数</returns>
        public bool Equals(EventArg obj) {
            return Array.Equals(this.para, obj.para);
        }

        /// <summary>
        /// 重载Hash算法作各成员Hash算法的加法余数运算
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

        #region IComparable 方法

        /// <summary>
        /// 实现IComparable中的方法实现对另一个对象的比较
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
        ///  先判断是否相等然后 根据hashCode值判断大小
        /// </summary>
        /// <param name="o"></param>
        /// <returns>是否相等如果不等使用hashCode判断</returns>
        public int CompareTo(EventArg obj) {
            if (Equals(obj))
                return 0;
            return this.GetHashCode() - obj.GetHashCode();
        }


        #region IComparer<EventArg> 方法

        /// <summary>
        /// 实现IComparer EventArg 接口的方法 比较2个EventArg类型的大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(EventArg x, EventArg y) {
            return x.CompareTo(y);
        }

        #endregion

        /// <summary>
        /// 重载IComparer接口的方法 比较2个object类型的大小
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
        /// 调用对象的无参数构造函数产生新的对象
        /// </summary>
        /// <returns></returns>
        protected virtual EventArg CreateNewInstance() {
            return (EventArg)Activator.CreateInstance(this.GetType());
        }

        /// <summary>
        /// 如果想使用Clone方法必须声明一个空参数构造函数或覆盖CreateNewInstance方法或者覆盖本方法
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
