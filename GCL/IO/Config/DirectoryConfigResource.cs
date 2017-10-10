using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GCL.Module.Trigger;
namespace GCL.IO.Config {
    /// <summary>
    /// 针对整个文件夹的监控（不包含下级文件夹）
    /// </summary>
    public class DirectoryConfigResource : AConfigResource {
        private CountTrigger counTrigger = new CountTrigger(0);
        private TimeTrigger timeTrigger = new TimeTrigger(new TimeSpan());
        private string filter;
        private DirectoryInfo dinfo;
        public DirectoryConfigResource(string filter, long limit, TimeSpan span) {
            this.filter = IOTool.GetFileName(filter);
            dinfo = new DirectoryInfo((filter.IndexOf(":")<0?AppDomain.CurrentDomain.BaseDirectory:"") + IOTool.GetPath(filter));

            this.LimitNum = limit;
            this.TimeSpanField = span;
            ParallelTriggerProxy ma = new ParallelTriggerProxy();
            ma.AddTrigger(this.counTrigger);
            ma.AddTrigger(this.timeTrigger);
            this.SetTrigger(new SerialTriggerProxy(ma, new DirectoryTrigger((filter.IndexOf(":") < 0 ? AppDomain.CurrentDomain.BaseDirectory : "") + filter)));
        }

        private Encoding encoding = GCL.Common.Tool.DefaultEncoding;

        public Encoding Encoding {
            get { return encoding; }
            set { encoding = value; }
        }

        public long LimitNum {
            get { return counTrigger.LimitNum; }
            set { this.counTrigger.LimitNum = value; }
        }

        public TimeSpan TimeSpanField {
            get { return timeTrigger.TimeSpanField; }
            set { timeTrigger.TimeSpanField = value; }
        }

        public override string Load() {
            StringBuilder sb = new StringBuilder();
            try {
                foreach (FileInfo info in dinfo.GetFiles(this.filter)) {
                    sb.Append(File.ReadAllText(info.FullName, this.encoding).Trim());
                }
                //去掉所有的<config>或者</config> <?……?> <!--……-->标记
                return regWro1.Replace(regWro2.Replace(regWro3.Replace(sb.ToString(), ""), ""), "");
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        static readonly Regex regWro1 = new Regex("</?[cC]onfig[^>]*>");
        static readonly Regex regWro2 = new Regex(@"<\?[^(\?>)]*\?>");
        static readonly Regex regWro3 = new Regex(@"<!--([^(-->)]|,)*-->");
        public override void Save(string value) {
            throw new InvalidOperationException("不允许对一个目录生成文件!");
        }
    }
}
