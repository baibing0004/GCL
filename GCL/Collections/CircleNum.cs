using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Collections {
    public class CircleNum : LimitNum {
        /// <summary>
        /// 分别初始化 最大，最小，初始，步长值
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <param name="init">设置初始值</param>
        /// <param name="step">设置步长值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public CircleNum(int max, int min, int init, int step)
            : base(max, min, init, step) {
        }


        /// <summary>
        /// 分别初始化 最大，最小，初始，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <param name="init">设置初始值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public CircleNum(int max, int min, int init)
            : base(max, min, init, 1) {
        }

        /// <summary>
        /// 分别初始化 最大，最小，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <param name="min">设置最小值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public CircleNum(int max, int min)
            : base(max, min, min) {
        }


        /// <summary>
        /// 分别初始化 最大，最小默认为0，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public CircleNum(int max)
            : base(max, 0) {
        }

        /// <summary>
        /// 分别初始化 最大默认为Int最大值MAX_VALUE，最小默认为0，初始默认为最小值，步长值默认为1
        /// </summary>
        /// <param name="max">设置最大值</param>
        /// <exception >如果现值超出限制抛出错误IndexOutOfRangeException</exception>
        public CircleNum()
            : base(int.MaxValue, 0) {
        }

        public override int SetNow(int data) {
            try {
                return base.SetNow(data);
            } catch (IndexOutOfRangeException) {
                return base.SetNow(base.Min);
            }
        }
    }
}
