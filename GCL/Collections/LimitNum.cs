using System;
using System.Collections.Generic;
using System.Text;
using GCL.Common;

namespace GCL.Collections {
    /// <summary>
    ///  ���������ṩ��ĳ����ֵ�����Ʒ��ʡ���������/���ٷ���Ϊͬ���������ṩͬ����ͬʱ��ĳ��������л��Ѳ�����  
    ///  LimitNum.increase(this)�ǿ�������ʹ�õģ�      
    /// </summary>
    public class LimitNum : System.IComparable, System.IComparable<LimitNum>, System.Collections.IComparer {

        private int now = 0;
        private int max = 0;
        private int min = 0;
        private int step = 1;

        /// <summary>
        /// �ֱ��ʼ�� �����С����ʼ������ֵ
        /// </summary>
        /// <param name="max">�������ֵ</param>
        /// <param name="min">������Сֵ</param>
        /// <param name="init">���ó�ʼֵ</param>
        /// <param name="step">���ò���ֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public LimitNum(int max, int min, int init, int step) {
            if (max < min)
                throw new System.IndexOutOfRangeException();
            this.max = max;
            this.min = min;
            this.step = Math.Abs(step) > 0 ? Math.Abs(step) : 1;
            this.SetNow(init);
        }


        /// <summary>
        /// �ֱ��ʼ�� �����С����ʼ������ֵĬ��Ϊ1
        /// </summary>
        /// <param name="max">�������ֵ</param>
        /// <param name="min">������Сֵ</param>
        /// <param name="init">���ó�ʼֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public LimitNum(int max, int min, int init)
            : this(max, min, init, 1) {
        }

        /// <summary>
        /// �ֱ��ʼ�� �����С����ʼĬ��Ϊ��Сֵ������ֵĬ��Ϊ1
        /// </summary>
        /// <param name="max">�������ֵ</param>
        /// <param name="min">������Сֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public LimitNum(int max, int min)
            : this(max, min, min) {
        }


        /// <summary>
        /// �ֱ��ʼ�� �����СĬ��Ϊ0����ʼĬ��Ϊ��Сֵ������ֵĬ��Ϊ1
        /// </summary>
        /// <param name="max">�������ֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public LimitNum(int max)
            : this(max, 0) {
        }

        /// <summary>
        /// �ֱ��ʼ�� ���Ĭ��ΪInt���ֵMAX_VALUE����СĬ��Ϊ0����ʼĬ��Ϊ��Сֵ������ֵĬ��Ϊ1
        /// </summary>
        /// <param name="max">�������ֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public LimitNum()
            : this(int.MaxValue, 0) {
        }


        /// <summary>
        /// ������ֵǰ������Ƿ񳬳����ƣ������׳�����
        /// </summary>
        /// <param name="data">������ֵ</param>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
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
        /// ����һ������
        /// </summary>
        /// <returns>��ֵ</returns>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public int Increase() {
            return Increase(this.step);
        }

        /// <summary>
        /// ����һ����ֵ 
        /// </summary>
        /// <param name="Step">��ֵ</param>
        /// <returns>��ֵ</returns>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public int Increase(int Step) {
            lock (this) {
                //��֤now��ȡʱ��Ψһ��
                return this.SetNow(this.now + Step);
            }
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        /// <param name="Step">��ֵ</param>
        /// <returns>��ֵ</returns>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public int Decrease() {
            return this.Decrease(this.step);
        }

        /// <summary>
        /// ����һ����ֵ 
        /// </summary>
        /// <param name="Step">��ֵ</param>
        /// <returns>��ֵ</returns>
        /// <exception >�����ֵ���������׳�����IndexOutOfRangeException</exception>
        public int Decrease(int Step) {
            return this.Increase(-1 * Step);
        }

        /// <summary>
        /// ���Nowֵ
        /// </summary>
        public int Now {
            get {
                return now;
            }
        }



        /// <summary>
        /// ���Maxֵ
        /// </summary>
        public int Max {
            get {
                return max;
            }
        }



        /// <summary>
        /// ���Minֵ
        /// </summary>
        public int Min {
            get {
                return min;
            }
        }


        /// <summary>
        /// ���Stepֵ
        /// </summary>
        public int Step {
            get {
                return this.step;
            }
        }

        /// <summary>
        /// ����now
        /// </summary>
        /// <returns></returns>
        public int GetNow() {
            return this.now;
        }

        /// <summary>
        /// ����Maxֵ
        /// </summary>
        /// <returns></returns>
        public int GetMax() {
            return max;
        }

        /// <summary>
        /// ����minֵ
        /// </summary>
        /// <returns></returns>
        public int GetMin() {
            return min;
        }

        /// <summary>
        /// ����Stepֵ
        /// </summary>
        /// <returns></returns>
        public int GetStep() {
            return step;
        }

        /// <summary>
        /// ����Equals���� 
        /// </summary>
        /// <param name="arg0">���һ���object����</param>
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
        /// ����Equals���� 
        /// </summary>
        /// <param name="e">���LimitNum����</param>
        /// <returns></returns>
        public bool Equals(LimitNum e) {
            return this.now == e.now;
        }

        /// <summary>
        /// ����Equals����
        /// </summary>
        /// <param name="e">���int����</param>
        /// <returns></returns>
        public bool Equals(int e) {
            return this.now == e;
        }

        /// <summary>
        /// ���ǻ�ȡHashCode����
        /// </summary>
        /// <returns></returns>
        public new int GetHashCode() {
            return this.now;
        }

        /// <summary>
        /// �����ַ���
        /// </summary>
        /// <returns></returns>
        public new string ToString() {
            return "" + this.now;
        }

        /// <summary>
        /// �Ƿ��Ѿ�������
        /// </summary>
        /// <returns></returns>
        public bool IsFull() {
            return this.now == this.max;
        }

        /// <summary>
        /// �Ƿ�Ϊ0
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            return this.now == 0;
        }

        /// <summary>
        /// �Ƿ��ѽ����ײ�
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

