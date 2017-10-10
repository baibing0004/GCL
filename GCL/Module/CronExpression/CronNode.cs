using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GCL.Module.CronExpression {
    //各种Cron标记的基类 a,b：a或者b  a/b：a为底b的倍数 -:从A到B

    /*
     * 一个Cron-表达式是一个由六至七个字段组成由空格分隔的字符串，其中6个字段是必须的而一个是可选的，如下：
字段名 		允许的值 		允许的特殊字符
秒 		0-59 		, - * /
分 		0-59 		, - * /
小时 		0-23 		, - * /
日 		1-31 		, - * ? / L W C
月 		1-12 or JAN-DEC 		, - * /
周几 		1-7 or SUN-SAT 		, - * ? / L C #
年 (可选字段) 		empty, 1970-2099 		, - * /

'*' 字符可以用于所有字段，在“分”字段中设为"*"表示"每一分钟"的含义。

'?' 字符可以用在“日”和“周几”字段. 它用来指定 '不明确的值'. 这在你需要指定这两个字段中的某一个值而不是另外一个的时候会被用到。在后面的例子中可以看到其含义。

'-' 字符被用来指定一个值的范围，比如在“小时”字段中设为"10-12"表示"10点到12点".

',' 字符指定数个值。比如在“周几”字段中设为"MON,WED,FRI"表示"the days Monday, Wednesday, and Friday".

'/' 字符用来指定一个值的的增加幅度. 比如在“秒”字段中设置为"0/15"表示"第0, 15, 30, 和 45秒"。而 "5/15"则表示"第5, 20, 35, 和 50". 在'/'前加"*"字符相当于指定从0秒开始. 每个字段都有一系列可以开始或结束的数值。对于“秒”和“分”字段来说，其数值范围为0到59，对于“小时”字段来说其为0到23, 对于“日”字段来说为0到31, 而对于“月”字段来说为1到12。"/"字段仅仅只是帮助你在允许的数值范围内从开始"第n"的值。 因此对于“月”字段来说"7/6"只是表示7月被开启而不是“每六个月”, 请注意其中微妙的差别。

'L'字符可用在“日”和“周几”这两个字段。它是"last"的缩写, 但是在这两个字段中有不同的含义。例如,“日”字段中的"L"表示"一个月中的最后一天" —— 对于一月就是31号对于二月来说就是28号（非闰年）。而在“周几”字段中, 它简单的表示"7" or "SAT"，但是如果在“周几”字段中使用时跟在某个数字之后, 它表示"该月最后一个星期×" —— 比如"6L"表示"该月最后一个周五"。当使用'L'选项时,指定确定的列表或者范围非常重要，否则你会被结果搞糊涂的。

'W' 可用于“日”字段。用来指定历给定日期最近的工作日(周一到周五) 。比如你将“日”字段设为"15W"，意为: "离该月15号最近的工作日"。因此如果15号为周六，触发器会在14号即周五调用。如果15号为周日, 触发器会在16号也就是周一触发。如果15号为周二,那么当天就会触发。然而如果你将“日”字段设为"1W", 而一号又是周六, 触发器会于下周一也就是当月的3号触发,因为它不会越过当月的值的范围边界。'W'字符只能用于“日”字段的值为单独的一天而不是一系列值的时候。

'L'和'W'可以组合用于“日”字段表示为'LW'，意为"该月最后一个工作日"。

'#' 字符可用于“周几”字段。该字符表示“该月第几个周×”，比如"6#3"表示该月第三个周五( 6表示周五而"#3"该月第三个)。再比如: "2#1" = 表示该月第一个周一而 "4#5" = 该月第五个周三。注意如果你指定"#5"该月没有第五个“周×”，该月是不会触发的。

'C' 字符可用于“日”和“周几”字段，它是"calendar"的缩写。 它表示为基于相关的日历所计算出的值（如果有的话）。如果没有关联的日历, 那它等同于包含全部日历。“日”字段值为5C在日期字段中就相当于日历5日以后的第一天。1C在星期字段中相当于星期日后的第一天。

对于“月份”字段和“周几”字段来说合法的字符都不是大小写敏感的。

下面是一些完整的例子:
表达式 		含义
"0 0 12 * * ?" 		每天中午十二点触发
"0 15 10 ? * *" 		每天早上10：15触发
"0 15 10 * * ?" 		每天早上10：15触发
"0 15 10 * * ? *" 		每天早上10：15触发
"0 15 10 * * ? 2005" 		2005年的每天早上10：15触发
"0 * 14 * * ?" 		每天从下午2点开始到2点59分每分钟一次触发
"0 0/5 14 * * ?" 		每天从下午2点开始到2：55分结束每5分钟一次触发
"0 0/5 14,18 * * ?" 		每天的下午2点至2：55和6点至6点55分两个时间段内每5分钟一次触发
"0 0-5 14 * * ?" 		每天14:00至14:05每分钟一次触发
"0 10,44 14 ? 3 WED" 		三月的每周三的14：10和14：44触发
"0 15 10 ? * MON-FRI" 		每个周一、周二、周三、周四、周五的10：15触发
"0 15 10 15 * ?" 		每月15号的10：15触发
"0 15 10 L * ?" 		每月的最后一天的10：15触发
"0 15 10 ? * 6L" 		每月最后一个周五的10：15触发
"0 15 10 ? * 6L" 		每月最后一个周五的10：15触发
"0 15 10 ? * 6L 2002-2005" 		2002年至2005年的每月最后一个周五的10：15触发
"0 15 10 ? * 6#3" 		每月的第三个周五的10：15触发
     */

    /// <summary>
    /// 各种Cron标记的基类
    /// a,b:a或者b
    /// a/b:a为底b的倍数
    /// a-b:从a到b
    /// a#b:允许在星期域中出现。这个字符用于指定本月的某某天。例如：“6#3”表示本月第三周的星期五星期日为1 注意这里DayOfWeek 中星期日为0
    /// aL：在日期和星期意思不同，例如day-of-month域中表示一个月的最后一天。如果在day-of-week域表示周六，如果在day-of-week域中前面加上数字，它表示一个月的最后第几天，例如‘6L’就表示一个月的最后一个星期五
    /// aW：只允许日期域出现。这个字符用于指定日期的最近工作日。例如：如果你在日期域中写 “15W”，表示：这个月15号最近的工作日
    /// aC:
    /// </summary>
    abstract class CronNode {
        private Regex ndeIdentify;

        public Regex NodeIdentify {
            get { return ndeIdentify; }
        }
        protected CronNode parent;
        private IDateTimePart datePart;
        public CronNode(string nodeIdentify, CronNode par, IDateTimePart dateTime) {
            this.ndeIdentify = new Regex(nodeIdentify);
            this.parent = par;
            this.datePart = dateTime;
        }

#if(DEBUG)
        public event Event.EventHandle NextEvent;
        public bool Next(DateTimeContainer container) {
            DateTime t = container.GetDateTime();
            Event.EventArg.CallEventSafely(NextEvent, this, new Event.EventArg(new object[] { t, this.GetType().Name, datePart.GetType().Name }));
#else
        public bool Next(DateTimeContainer container) {
#endif
            int changeValue = this.Regen(container.GetDateTime(), datePart);
            if (changeValue != 0)
                container.SetDateTime(datePart.AddTime(container.GetDateTime(), changeValue));

            //if ((parent != null && parent.Next(container)) || (parent == null && changeValue != 0)) {
            if ((parent != null && parent.Next(container))) {
                //清零
                int changeFirst = this.GetFirst(container.GetDateTime(), datePart);
                container.SetDateTime(datePart.AddTime(container.GetDateTime(), changeFirst));
                return true;
            }
            return changeValue != 0;
        }

        /// <summary>
        /// 用于对于当前操作进行调整（扩大）
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected abstract int Regen(DateTime time, IDateTimePart part);
        /// <summary>
        /// 用于对于当前操作如何设置初始值（缩小）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected abstract int GetFirst(DateTime time, IDateTimePart part);
    }

    abstract class CommonCronNode : CronNode {

        public CommonCronNode(string nodeIdentify, CronNode par, IDateTimePart dateTime) : base(nodeIdentify, par, dateTime) { }

        protected override int Regen(DateTime time, IDateTimePart part) {
            if (part is DayDateTimePart)
                return this.DayRegen(part.GetTime(time), part.GetMaxDate(time), part.GetMaxDate(time.AddMonths(1)));
            else
                return this.Regen(part.GetTime(time), part.GetMaxDate(time));
        }

        protected override int GetFirst(DateTime time, IDateTimePart part) {
            if (part is DayDateTimePart)
                return this.DayGetFirst(part.GetTime(time), part.GetMaxDate(time), part.GetMaxDate(time.AddMonths(1)));
            else
                return this.GetFirst(part.GetTime(time), part.GetMaxDate(time));
        }
        /// <summary>
        /// 29，30，31号必然可以设置在本月或者下下个月 一般的这里的条件只对2月有用
        /// </summary>
        /// <param name="time"></param>
        /// <param name="maxDate"></param>
        /// <param name="nextmaxDate"></param>
        /// <returns></returns>
        protected abstract int DayRegen(int time, int maxDate, int nextmaxDate);
        /// <summary>
        ///29，30，31号必然可以设置在本月或者下下个月 一般的这里的条件只对2月有用
        /// </summary>
        /// <param name="time"></param>
        /// <param name="maxDate"></param>
        /// <param name="nextmaxDate"></param>
        /// <returns></returns>
        protected abstract int DayGetFirst(int time, int maxDate, int nextmaxDate);
        /// <summary>
        /// 用于对于当前操作进行调整（扩大）
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected abstract int Regen(int time, int maxDate);
        /// <summary>
        /// 用于对于当前操作如何设置初始值（缩小）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected abstract int GetFirst(int time, int maxDate);
    }
}
