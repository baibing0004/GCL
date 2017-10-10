using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;

namespace GCL.Collections {
    /// <summary>
    ///  本类用于提供对某个数值的限制访问。所有增加/减少方法为同步方法。提供同步的同时对某个对象进行唤醒操作！  
    ///  LimitNum.increase(this)是可以正常使用的！      
    /// </summary>
    public class LimitNum : System.IComparable, System.IComparable<LimitNum>, System.Collections.IComparer {

        private int now = 0;
        private int max = 0;
        private int min = 0;
        private int step = 1;

        /// <summary>
        /// 分别初始化 最大，最小，初始，步长值
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <param name="init">设置初始值</param>
        /// <param name="step">设置步长值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public LimitNum(int max, int min, int init, int step) {
            if (max < min)
                throw new System.IndexOutOfRangeException();
            this.max = max;
            this.min = min;
            this.step = Math.Abs(step) > 0 ? Math.Abs(step) : 1;
            this.SetNow(init);
        }


        /// <summary>
        /// 分别初始化 最大，最小，初始，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <param name="init">设置初始值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public LimitNum(int max, int min, int init)
            : this(max, min, init, 1) {
        }

        /// <summary>
        /// 分别初始化 最大，最小，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public LimitNum(int max, int min)
            : this(max, min, min) {
        }


        /// <summary>
        /// 分别初始化 最大，最小默认为0，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public LimitNum(int max)
            : this(max, 0) {
        }

        /// <summary>
        /// 分别初始化 最大默认为Int最大值MAX_VALUE，最小默认为0，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public LimitNum()
            : this(int.MaxValue, 0) {
        }


        /// <summary>
        /// 设置现值前，检查是否超出限制，否则抛出错误。
        /// </summary>
        /// <param name="data">设置现值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public virtual int SetNow(int data) {
            lock (this) {
                if (min == max)
                    return now;
                if (data < min || data > max)
                    throw new IndexOutOfRangeException();
                this.now = data;
                Tool.ObjectPulseAll(this);
                return this.now;
            }
        }

        /// <summary>
        /// 增加一个步长
        /// </summary>
        /// <returns>现值</returns>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public int Increase() {
            return Increase(this.step);
        }

        /// <summary>
        /// 增加一个数值 
        /// </summary>
        /// <param name="Step">数值</param>
        /// <returns>现值</returns>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public int Increase(int Step) {
            lock (this) {
                //保证now获取时的唯一性
                return this.SetNow(this.now + Step);
            }
        }

        /// <summary>
        /// 减少一个步长
        /// </summary>
        /// <param name="Step">数值</param>
        /// <returns>现值</returns>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public int Decrease() {
            return this.Decrease(this.step);
        }

        /// <summary>
        /// 减少一个数值 
        /// </summary>
        /// <param name="Step">数值</param>
        /// <returns>现值</returns>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public int Decrease(int Step) {
            return this.Increase(-1 * Step);
        }

        /// <summary>
        /// 获得Now值
        /// </summary>
        public int Now {
            get {
                return now;
            }
        }



        /// <summary>
        /// 获得Max值
        /// </summary>
        public int Max {
            get {
                return max;
            }
        }



        /// <summary>
        /// 获得Min值
        /// </summary>
        public int Min {
            get {
                return min;
            }
        }


        /// <summary>
        /// 获得Step值
        /// </summary>
        public int Step {
            get {
                return this.step;
            }
        }

        /// <summary>
        /// 返回now
        /// </summary>
        /// <returns></returns>
        public int GetNow() {
            return this.now;
        }

        /// <summary>
        /// 返回Max值
        /// </summary>
        /// <returns></returns>
        public int GetMax() {
            return max;
        }

        /// <summary>
        /// 返回min值
        /// </summary>
        /// <returns></returns>
        public int GetMin() {
            return min;
        }

        /// <summary>
        /// 返回Step值
        /// </summary>
        /// <returns></returns>
        public int GetStep() {
            return step;
        }

        /// <summary>
        /// 覆盖Equals方法 
        /// </summary>
        /// <param name="arg0">针对一般的object对象</param>
        /// <returns></returns>
        public new bool Equals(object arg0) {
            if (arg0 is int)
                return Equals((int)arg0);
            else if (arg0 is LimitNum)
                return Equals((LimitNum)arg0);
            else
                return false;
        }

        /// <summary>
        /// 覆盖Equals方法 
        /// </summary>
        /// <param name="e">针对LimitNum对象</param>
        /// <returns></returns>
        public bool Equals(LimitNum e) {
            return this.now == e.now;
        }

        /// <summary>
        /// 覆盖Equals方法
        /// </summary>
        /// <param name="e">针对int对象</param>
        /// <returns></returns>
        public bool Equals(int e) {
            return this.now == e;
        }

        /// <summary>
        /// 覆盖获取HashCode方法
        /// </summary>
        /// <returns></returns>
        public new int GetHashCode() {
            return this.now;
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public new string ToString() {
            return "" + this.now;
        }

        /// <summary>
        /// 是否已经到顶部
        /// </summary>
        /// <returns></returns>
        public bool IsFull() {
            return this.now == this.max;
        }

        /// <summary>
        /// 是否为0
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            return this.now == 0;
        }

        /// <summary>
        /// 是否已近到底部
        /// </summary>
        /// <returns></returns>
        public bool IsButtom() {
            return this.now == this.min;
        }

        #region IComparer Members

        public int Compare(object x, object y) {
            bool o1IsSon = x is LimitNum;
            bool o2IsSon = y is LimitNum;
            if (o1IsSon && o2IsSon)
                return this.Compare((LimitNum)x, (LimitNum)y);
            else if (o1IsSon)
                return 1;
            else if (o2IsSon)
                return -1;
            else
                return x.GetHashCode() - y.GetHashCode();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj) {
            if (Equals(obj))
                return 0;
            return this.GetHashCode() - obj.GetHashCode();
        }

        #endregion

        #region IComparable<LimitNum> Members

        public int CompareTo(LimitNum other) {
            return this.Now - other.Now;
        }

        #endregion
    }
}

