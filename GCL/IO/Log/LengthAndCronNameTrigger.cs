using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GCL.Module.CronExpression;
namespace GCL.IO.Log {
    /// <summary>
    /// 长度与克隆文件名触发器，
    /// 其会产生根据时间与文件长度产生的克隆文件名
    /// </summary>
    public class LengthAndCronNameTrigger : INameTrigger {
        private long limitLeng = 0;
        private int num = 0;
        private DateTime passTime = DateTime.Now;
        private DateTime oldTime = DateTime.Now;
        private CronExpression exp;
        private string timeFormat = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitLeng">Byte数目</param>
        /// <param name="cronExpression">克隆表达式</param>
        /// <param name="timeFormat">时间格式化串</param>
        public LengthAndCronNameTrigger(long limitLeng, string cronExpression, string timeFormat) {
            this.limitLeng = limitLeng;
            this.exp = new CronExpression(cronExpression);
            this.passTime = this.exp.Next(oldTime);
            this.timeFormat = timeFormat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="timeFormat"></param>
        public LengthAndCronNameTrigger(string cronExpression, string timeFormat) : this((long)IOTool.MToBytes(200), cronExpression, timeFormat) { }

        #region INameTrigger Members

        public string Taste(string file) {
            //long leng = File.Exists(file)?new FileInfo(file).Length:0;
            if (string.IsNullOrEmpty(file) || (File.Exists(file) && new FileInfo(file).Length > limitLeng) || this.passTime.CompareTo(DateTime.Now) <= 0) {
                //需要重新生成文件名
                if (!string.IsNullOrEmpty(file) && File.Exists(file) && new FileInfo(file).Length > limitLeng) num++;
                string value = string.Format(timeFormat, oldTime) + (num > 0 ? num.ToString() : "");
                if (passTime.CompareTo(DateTime.Now) <= 0) {
                    oldTime = passTime; num = 0;
                    value = string.Format(timeFormat, passTime) + (num > 0 ? num.ToString() : "");
                    passTime = exp.Next(passTime);
                }
                return value;
            }
            return null;
        }

        #endregion
    }
}
