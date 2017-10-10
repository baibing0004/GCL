using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    public class RegularTextTrigger : ATrigger {
        Regex regex = null;
        public RegularTextTrigger(string regexExpression) {
            regex = new Regex(regexExpression);
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            if (regex.Matches((e.GetPara(0) as LogRecord).GetContent()).Count > 0)
                return e;
            else
                return null;
        }

        public override void ReSet() {
        }
    }
}
