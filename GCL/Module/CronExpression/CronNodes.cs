using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Module.CronExpression {

    /// <summary>
    /// 处理*号
    /// </summary>
    class XCronNode : CommonCronNode {

        public XCronNode(string text, CronNode par, IDateTimePart part) : base("^\\*$", par, part) { }

        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            return 0;
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            return 1 - time;
        }

        protected override int Regen(int time, int maxDate) {
            return 0;
        }

        protected override int GetFirst(int time, int maxDate) {
            return -1 * time;
        }
    }

    /// <summary>
    /// 处理？号
    /// </summary>
    class QCronNode : CronNode {

        public QCronNode(string text, CronNode par, IDateTimePart part) : base("^\\?$", par, part) { }

        protected override int Regen(DateTime time, IDateTimePart part) {
            return 0;
        }

        protected override int GetFirst(DateTime time, IDateTimePart part) {
            return 0;
        }
    }

    /// <summary>
    /// 处理具体数字 
    /// //TODO请注意星期中1-7需要改为0-6
    /// </summary>
    class NCronNode : CommonCronNode {
        private int num = 0;
        public NCronNode(string num, CronNode par, IDateTimePart datePart)
            : base("^\\d+$", par, datePart) {
            this.num = Convert.ToInt32(num);
            if (datePart is WeekDateTimePart)
                this.num--;
        }

        protected override int Regen(int time, int maxDate) {
            return (time > num) ? (maxDate - time + num) : num - time;
        }

        protected override int GetFirst(int time, int maxDate) {
            return num - time;
        }

        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            //29，30，31号必然可以设置在本月或者下下个月 一般的这里的条件只对2月有用
            return this.Regen(time, maxDate) + ((time > num && num > nextmaxDate) ? nextmaxDate : 0);
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            if (num > maxDate)
                //说明本月无法设置 设置为下个月
                return maxDate - time + num;

            return this.GetFirst(time, maxDate);
        }
    }

    /// <summary>
    /// 处理逗号
    /// </summary>
    class DCronNode : CommonCronNode {
        private int[] nums;
        public DCronNode(string num, CronNode par, IDateTimePart datePart)
            : base("^\\d+[,\\d+]+$", par, datePart) {
            string[] _n = num.Split(',');
            this.nums = new int[_n.Length];
            for (int w = 0; w < _n.Length; w++)
                if (datePart is WeekDateTimePart)
                    nums[w] = Convert.ToInt32(_n[w]) - 1;
                else
                    nums[w] = Convert.ToInt32(_n[w]);
            Array.Sort(nums);
        }

        protected int GetSmallNum(int time) {
            if (time <= nums[0])
                return 0;
            if (time >= nums[nums.Length - 1])
                return nums.Length - 1;

            int l = 0, r, m;
            r = nums.Length;
            while (r - l > 1) {
                m = (l + r) / 2;
                if (time >= nums[m])
                    l = m;
                else
                    r = m;
            }
            return l;
        }

        protected override int Regen(int time, int maxDate) {
            //当前数字已经大于最大的数
            if (time > nums[nums.Length - 1])
                return maxDate - time + nums[0];
            int order = GetSmallNum(time);
            return (time > nums[order]) ? (nums[order + 1] - time) : nums[order] - time;
        }

        protected override int GetFirst(int time, int maxDate) {
            return nums[0] - time;
        }

        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            if (time > nums[nums.Length - 1]) {
                //说明下个月也无法设置 设置为下下个月
                //29，30，31号必然可以设置在本月或者下下个月 一般的这里的条件只对2月有用
                return maxDate - time + nums[0] + (nums[0] > nextmaxDate ? nextmaxDate : 0);
            }
            int order = GetSmallNum(time);
            return (time > nums[order]) ? (nums[order + 1] - time) : nums[order] - time;
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            if (nums[0] > maxDate)
                //说明本月无法设置 设置为下个月
                return maxDate - time + nums[0];

            return this.GetFirst(time, maxDate);
        }
    }

    /// <summary>
    /// -号
    /// </summary>
    class SCronNode : CommonCronNode {
        private int[] nums;
        public SCronNode(string num, CronNode par, IDateTimePart datePart)
            : base("^\\d+-\\d+$", par, datePart) {
            string[] _n = num.Split('-');
            int l = Convert.ToInt32(_n[0]);
            int r = Convert.ToInt32(_n[1]);
            this.nums = new int[r - l + 1];
            for (int w = 0; w < nums.Length; w++)
                if (datePart is WeekDateTimePart)
                    nums[w] = w + l - 1;
                else
                    nums[w] = w + l;
        }

        protected int GetSmallNum(int time) {
            if (time <= nums[0])
                return 0;
            if (time >= nums[nums.Length - 1])
                return nums.Length - 1;

            int l = 0, r, m;
            r = nums.Length;
            while (r - l > 1) {
                m = (l + r) / 2;
                if (time >= nums[m])
                    l = m;
                else
                    r = m;
            }
            return l;
        }

        protected override int Regen(int time, int maxDate) {
            //当前数字已经大于最大的数
            if (time > nums[nums.Length - 1])
                return maxDate - time + nums[0];
            int order = GetSmallNum(time);
            return (time > nums[order]) ? (nums[order + 1] - time) : nums[order] - time;
        }

        protected override int GetFirst(int time, int maxDate) {
            return nums[0] - time;
        }

        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            if (time > nums[nums.Length - 1]) {
                //说明下个月也无法设置 设置为下下个月
                //29，30，31号必然可以设置在本月或者下下个月 一般的这里的条件只对2月有用
                return maxDate - time + nums[0] + (nums[0] > nextmaxDate ? nextmaxDate : 0);
            }
            int order = GetSmallNum(time);
            //当前数字已经大于最大的数
            return (time > nums[order]) ? (nums[order + 1] - time) : nums[order] - time;
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            if (nums[0] > maxDate)
                //说明本月无法设置 设置为下个月
                return maxDate - time + nums[0];

            return this.GetFirst(time, maxDate);
        }
    }

    /// <summary>
    /// 除号
    /// </summary>
    class PCronNode : CommonCronNode {

        int l = 0, r = 0;
        public PCronNode(string num, CronNode par, IDateTimePart part)
            : base("^\\d+/\\d+$", par, part) {
            string[] _n = num.Split('/');
            l = Convert.ToInt32(_n[0]);
            r = Convert.ToInt32(_n[1]);
            if (part is WeekDateTimePart) { l--; }
        }

        protected int GetBigNum(int time) {
            return ((time - l) / r + 1) * r + l;
        }

        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            if (time > l && (time - l) % r == 0)
                return 0;
            int value = GetBigNum(time);
            if (value > maxDate)
                //如果下个月不能设置那么就设置到下下个月
                return maxDate - time + l + (l > nextmaxDate ? nextmaxDate : 0);
            else
                return value - time;
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            if (l > maxDate)
                //如果本月设置不了那么下个月肯定可以设置
                return maxDate - time + l;
            return l - time;
        }

        protected override int Regen(int time, int maxDate) {
            if (time >= l && (time - l) % r == 0)
                return 0;
            int value = GetBigNum(time);
            if (value > maxDate)
                return maxDate - time + l;
            else
                return value - time;
        }

        protected override int GetFirst(int time, int maxDate) {
            return l - time;
        }
    }

    /// <summary>
    /// #号 注意将1-7转为0-6
    /// </summary>
    class ACronNode : CronNode {
        int l = 0, r = 0;
        public ACronNode(string num, CronNode par, IDateTimePart part)
            : base("^[1-7]#[1-5]$", par, part) {
            string[] _n = num.Split('#');
            l = Convert.ToInt32(_n[0]) - 1;
            r = Convert.ToInt32(_n[1]);
        }

        private int GetMaxDay(DateTime time, IDateTimePart part) {
            if (part is WeekDateTimePart)
                return System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetDaysInMonth(time.Year, time.Month);
            else
                return part.GetMaxDate(time);
        }
        /// <summary>
        /// 得到第r个星期l是几号
        /// </summary>
        /// <param name="time"></param>
        /// <param name="weekDay"></param>
        /// <returns></returns>
        protected int GetRightDay(int time, int weekDay) {
            //int firstDay=7- (time - weekDay-1) % 7;
            //if (firstDay == 7)
            //    firstDay = 0;
            //int firstDay = Math.Abs((time - weekDay - 1) % 7);
            int firstDay = ((1 - time + weekDay) % 7 + 7) % 7;
            return 7 * (this.r - 1) + 1 + l - firstDay + (this.l < firstDay ? 7 : 0);
        }

        protected override int Regen(DateTime time, IDateTimePart part) {
            int num = GetRightDay(time.Day, (int)time.DayOfWeek);
            int maxDate = GetMaxDay(time, part);
            if (time.Day <= num && num <= maxDate)
                return num - time.Day;
            else {
                int value = 0;
                while (time.Day>num || maxDate < num) {
                    //下个月1号
                    int _t = maxDate - time.Day + 1;
                    value += _t;
                    time = time.AddDays(_t);
                    num = GetRightDay(time.Day, (int)time.DayOfWeek);
                    maxDate = GetMaxDay(time, part);
                }
                return value + num - time.Day;
            }
        }

        protected override int GetFirst(DateTime time, IDateTimePart part) {
            int num = GetRightDay(time.Day, (int)time.DayOfWeek);
            int maxDate = GetMaxDay(time, part);
            if (time.Day <= num && num <= maxDate)
                return time.Day - num;
            else {
                int value = 0;
                while (time.Day > num || maxDate < num) {
                    //下个月1号
                    int _t = maxDate - time.Day + 1;
                    value += _t;
                    time = time.AddDays(_t);
                    num = GetRightDay(time.Day, (int)time.DayOfWeek);
                    maxDate = GetMaxDay(time, part);
                }
                return value + num - time.Day;
            }
        }
    }

    /// <summary>
    /// W号
    /// </summary>
    class WCronNode : CronNode {
        int l;
        public WCronNode(string num, CronNode par, IDateTimePart part)
            : base("^(\\d+|L)W$", par, part) {
            if (num.ToUpper().Equals("LW"))
                l = 31;
            else
                l = Convert.ToInt32(num.Substring(0, num.Length - 1));
        }

        /// <summary>
        /// 得到l日附近的工作日是几号
        /// </summary>
        /// <param name="time"></param>
        /// <param name="weekDay"></param>
        /// <returns></returns>
        protected int GetRightDay(int time, int maxDate, int weekDay) {
            //int firstDay = 7 - (time - weekDay - 1) % 7;
            //if (firstDay == 7)
            //    firstDay = 0;
            //return ((time-1)%7+firstDay)%7;
            int _l = Math.Min(l, maxDate);
            int r = ((_l - time + weekDay) % 7 + 7) % 7;
            if (r > 5)
                return _l - 1;
            if (r < 1)
                return _l + 1;
            return _l;
        }

        protected override int Regen(DateTime time, IDateTimePart part) {
            int maxDate = part.GetMaxDate(time);
            int num = GetRightDay(time.Day, maxDate, (int)time.DayOfWeek);
            if (time.Day <= num && maxDate >= num)
                return num - time.Day;
            else {
                return maxDate - time.Day + GetRightDay(1, part.GetMaxDate(time.AddMonths(1)), ((1 + maxDate - time.Day + (int)time.DayOfWeek) % 7 + 7) % 7);
            }
        }

        protected override int GetFirst(DateTime time, IDateTimePart part) {
            return GetRightDay(time.Day, part.GetMaxDate(time), (int)time.DayOfWeek) - time.Day;
        }
    }

    /// <summary>
    /// 处理最后几天，或者最后一个星期几的问题
    /// </summary>
    class LCronNode : CronNode {
        int l = 0;
        public LCronNode(string num, CronNode par, IDateTimePart part)
            : base("^\\d*L$", par, part) {
            if (num.Length > 1)
                l = Convert.ToInt32(num.Substring(0, num.Length - 1));
            if (part is WeekDateTimePart && l > 0) { l--; }
        }

        private int GetMaxDay(DateTime time, IDateTimePart part) {
            if (part is WeekDateTimePart)
                return System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetDaysInMonth(time.Year, time.Month);
            else
                return part.GetMaxDate(time);
        }
        /// <summary>
        /// 得到l日附近的工作日是几号
        /// </summary>
        /// <param name="time"></param>
        /// <param name="weekDay"></param>
        /// <returns></returns>
        protected int GetRightDay(DateTime time, IDateTimePart part) {
            if (part is WeekDateTimePart) {
                int last = GetMaxDay(time,part);
                //得到最后一天是星期几
                int lastDay = ((last - time.Day + (int)time.DayOfWeek) % 7 + 7) % 7;
                return last + l - lastDay - (l < lastDay ? 0 : 7);
            } else
                return part.GetMaxDate(time) - l;
        }

        protected override int Regen(DateTime time, IDateTimePart part) {
            int maxDate = GetMaxDay(time, part);

            int num = GetRightDay(time, part);
            if (time.Day <= num && maxDate >= num)
                return num - time.Day;
            else
                return maxDate - time.Day + GetRightDay(time.AddMonths(1), part);
        }

        protected override int GetFirst(DateTime time, IDateTimePart part) {
            int maxDate = GetMaxDay(time, part);
            
            int num = GetRightDay(time, part);
            if (time.Day <= num && maxDate >= num)
                return time.Day - num;
            else
                return maxDate - time.Day + GetRightDay(time.AddMonths(1), part);
        }
    }
    /// <summary>
    /// 处理几号以后 或者星期几以后
    /// </summary>
    class CCronNode : CommonCronNode {
        int l = 0;
        public CCronNode(string num, CronNode par, IDateTimePart part)
            : base("^\\d+C$", par, part) {
            if (num.Length > 1)
                l = Convert.ToInt32(num.Substring(0, num.Length - 1));
            if (part is WeekDateTimePart) { l--; }
        }


        protected override int DayRegen(int time, int maxDate, int nextmaxDate) {
            if (time >= l)
                return 0;
            else
                return l - time + ((l > maxDate) ? maxDate : 0);
        }

        protected override int DayGetFirst(int time, int maxDate, int nextmaxDate) {
            return l - time;
        }

        protected override int Regen(int time, int maxDate) {
            if (time >= l)
                return 0;
            else
                return l - time;
        }

        protected override int GetFirst(int time, int maxDate) {
            return l - time;
        }
    }
}
