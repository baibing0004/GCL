using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using GCL.Module.Trigger;
namespace GCL.IO.Config {
    public class FileConfigResource : AConfigResource {


        private CountTrigger counTrigger = new CountTrigger(0);
        private TimeTrigger timeTrigger = new TimeTrigger(new TimeSpan());
        private FileInfo file;

        public FileConfigResource(FileInfo file, long limit, TimeSpan span) {
            this.file = file;
            this.LimitNum = limit;
            this.TimeSpanField = span;
            ParallelTriggerProxy ma = new ParallelTriggerProxy();
            ma.AddTrigger(this.counTrigger);
            ma.AddTrigger(this.timeTrigger);
            this.SetTrigger(new SerialTriggerProxy(ma, new FileTrigger(file)));
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
            if (file.Exists)
                return regWro2.Replace(regWro3.Replace(File.ReadAllText(file.FullName, this.encoding), ""), "");
            else
                return "";
        }
        static readonly Regex regWro2 = new Regex(@"<\?[^(\?>)]*\?>");
        static readonly Regex regWro3 = new Regex(@"<!--([^(-->)]|,)*-->");
        public override void Save(string value) {
            File.WriteAllText(file.FullName, value, this.encoding);
        }
    }
}
