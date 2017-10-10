using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Module.CronExpression {
    /// <summary>
    /// 用于分别获取年/月/星期/日/时/分/秒实现
    /// </summary>
    interface IDateTimePart {
        int GetTime(DateTime time);
        int GetMaxDate(DateTime time);
        DateTime AddTime(DateTime time, int data);
    }

    /// <summary>
    /// 年
    /// </summary>
    public class YearDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Year;
        }

        public int GetMaxDate(DateTime time) {
            return 10000;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddYears(data);
        }

        #endregion
    }

    /// <summary>
    /// 星期
    /// </summary>
    public class WeekDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return (int)time.DayOfWeek;
        }

        public int GetMaxDate(DateTime time) {
            //int limit=System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetDaysInMonth(time.Year, time.Month);
            //return (limit - time.Day < 7) ? (limit - time.Day + GetTime(time)) : 6;
            return 7;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddDays(data);
        }

        #endregion
    }

    /// <summary>
    /// 月
    /// </summary>
    public class MonthDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Month;
        }

        public int GetMaxDate(DateTime time) {
            return 12;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddMonths(data);
        }

        #endregion
    }



    /// <summary>
    /// 日
    /// </summary>
    public class DayDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Day;
        }

        public int GetMaxDate(DateTime time) {
            return System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetDaysInMonth(time.Year, time.Month);
        }


        public DateTime AddTime(DateTime time, int data) {
            return time.AddDays(data);
        }

        #endregion
    }

    /// <summary>
    /// 时
    /// </summary>
    public class HourDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Hour;
        }

        public int GetMaxDate(DateTime time) {
            return 24;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddHours(data);
        }

        #endregion
    }

    /// <summary>
    /// 分
    /// </summary>
    public class MinuteDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Minute;
        }

        public int GetMaxDate(DateTime time) {
            return 60;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddMinutes(data);
        }

        #endregion
    }

    /// <summary>
    /// 秒
    /// </summary>
    public class SecondDateTimePart : IDateTimePart {
        #region IDateTimePart Members

        public int GetTime(DateTime time) {
            return time.Second;
        }

        public int GetMaxDate(DateTime time) {
            return 60;
        }

        public DateTime AddTime(DateTime time, int data) {
            return time.AddSeconds(data);
        }

        #endregion
    }

   
}
