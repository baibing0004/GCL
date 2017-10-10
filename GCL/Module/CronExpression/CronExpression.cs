using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
namespace GCL.Module.CronExpression {
    public sealed class CronExpression {
        private CronNode nodes;
        static IDateTimePart[] PARTS = new IDateTimePart[] { new SecondDateTimePart(), new MinuteDateTimePart(), new HourDateTimePart(), new DayDateTimePart(), new WeekDateTimePart(), new MonthDateTimePart(), new YearDateTimePart() };
        static Regex[] REGEXS = new Regex[] { 
             new Regex(string.Format(@"(^\*$)|(^{0}([-/]{0})?$)|(^{0}(,{0})+$)","[1-5]?[0-9]"))
            ,new Regex(string.Format(@"(^\*$)|(^{0}([-/]{0})?$)|(^{0}(,{0})+$)","[1-5]?[0-9]"))
            ,new Regex(string.Format(@"(^\*$)|(^{0}([-/]{0})?$)|(^{0}(,{0})+$)","[1-2]?[0-9]"))
            ,new Regex(string.Format(@"(^[\*\?]$)|(^{0}([-/]{0})?$)|(^{0}?[LWC]$)|(^{0}(,{0})+$)|(^LW$)","[1-3]?[0-9]"))
            ,new Regex(string.Format(@"(^\*$)|(^{0}([-/]{0})?$)|(^{0}(,{0})+$)","1?[0-9]"))
            ,new Regex(string.Format(@"(^[\*\?]$)|(^{0}([-/#]{0})?$)|(^{0}?[LC]$)|(^{0}(,{0})+$)","[1-7]"))
            ,new Regex(string.Format(@"(^\*$)|(^{0}([-/]{0})?$)|(^{0}(,{0})+$)","[0-9]{4}"))
        };
        static int[][] FIELDLIMIT = new int[][]{new int[]{ 0, 59 }, new int[]{ 0, 59 }, new int[]{ 0, 23 }, new int[]{ 1, 31 },
				new int[]{ 1, 12 }, new int[]{ 1, 7 }, new int[]{ 1, 9999 } };

        public CronExpression(string expression) {
            expression = expression.Trim();
            if (expression.Split(' ').Length < 7)
                expression += " *";
            string[] exp = expression.ToUpper().Split(' ');

            MatchCronExpression(exp);

            #region 替换月与星期的位置 理顺嵌套关系！
            string _t = exp[5];
            exp[5] = exp[4];
            exp[4] = _t;
            #endregion
            for (int w = 6; w >= 0; w--) {
                nodes = CreateCronNode(exp[w], nodes, PARTS[w]);
#if(DEBUG)
                nodes.NextEvent += new GCL.Event.EventHandle(nodes_NextEvent);
            }
            this.NextEvent += new GCL.Event.EventHandle(GCL.Event.EventArg._EventHandleDefault);

        }
        public event Event.EventHandle NextEvent;
        void nodes_NextEvent(object sender, GCL.Event.EventArg e) {
            Event.EventArg.CallEventSafely(NextEvent, sender, e);
        }
#else
            }
        }
#endif
        static Type[] TYPES = new Type[] { typeof(XCronNode), typeof(QCronNode), typeof(NCronNode), typeof(DCronNode), typeof(SCronNode), typeof(PCronNode), typeof(ACronNode), typeof(WCronNode), typeof(LCronNode), typeof(CCronNode) };
        static Regex[] CRONREGEX = new Regex[] { new Regex("^\\*$"), new Regex("^\\?$"), new Regex("^\\d+$"), new Regex("^\\d+[,\\d+]+$"), new Regex("^\\d+-\\d+$"), new Regex("^\\d+/\\d+$"), new Regex("^[1-7]#[1-5]$"), new Regex("^(\\d+|L)W$"), new Regex("^\\d*L$"), new Regex("^\\d+C$") };
        static CronNode CreateCronNode(string text, CronNode node, IDateTimePart part) {
            for (int w = 0; w < CRONREGEX.Length; w++)
                if (CRONREGEX[w].Match(text).Success) return Activator.CreateInstance(TYPES[w], new object[] { text, node, part }) as CronNode;

            throw new Exception(text + "没有找到合适的表示式元素!");
        }

        static Regex NUMREGEX = new Regex("\\d+");
        static bool MatchCronExpression(string[] text) {
            for (int w = 0; w < 7; w++) {
                if (!REGEXS[w].Match(text[w]).Success)
                    throw new Exception(string.Format("第{0}项不符合要求:{1}", w + 1, text[w]));
                for (IEnumerator ienum = NUMREGEX.Matches(text[w]).GetEnumerator(); ienum.MoveNext(); ) {
                    int num = Convert.ToInt32(((Match)ienum.Current).Value);
                    if (!(FIELDLIMIT[w][0] <= num & FIELDLIMIT[w][1] >= num))
                        throw new Exception(string.Format("第{0}项超出范围:{1}", w + 1, text[w]));
                }

            }
            return true;
        }
        /*
         *(^\*$)|(^[1-5]?[0-9]([-/][1-5]?[0-9])?$)|(^[1-5]?[0-9](,[1-5]?[0-9])+$)
        (^\*$)|(^[1-5]?[0-9]([-/][1-5]?[0-9])?$)|(^[1-5]?[0-9](,[1-5]?[0-9])+$)
        (^\*$)|(^[1-2]?[0-9]([-/][1-2]?[0-9])?$)|(^[1-2]?[0-9](,[1-2]?[0-9])+$)
        (^[\*\?]$)|(^[1-3]?[0-9]([-/][1-3]?[0-9])?$)|(^[1-3]?[0-9]?[LWC]$)|(^[1-3]?[0-9](,[1-3]?[0-9])+$)
        (^\*$)|(^1?[0-9]([-/]1?[0-9])?$)|(^1?[0-9](,1?[0-9])+$)
        (^[\*\?]$)|(^[1-7]([-/#][1-7])?$)|(^[1-7]?[LC]$)|(^[1-7](,[1-7])+$)
        (^\*$)|(^[0-9]{4}([-/][0-9]{4})?$)|(^[0-9]{4}(,[0-9]{4})+$)
                FieldName = new String[] { "秒", "分", "小时", "日期", "月", "星期", "年" };
         */
        public DateTime Next(DateTime time) {
            DateTimeContainer container = new DateTimeContainer();
            container.SetDateTime(time.AddSeconds(1));
            while (this.nodes.Next(container)) ;
            return container.GetDateTime();
        }


    }
}
