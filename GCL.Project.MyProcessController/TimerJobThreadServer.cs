using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.IO.Log;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 定时Job服务
    /// </summary>
    public class TimerJobThreadServer : ABusinessThreadServer {

        private IJob job;
        public TimerJobThreadServer(int waitTime, IJob job)
            : base(new GCL.Threading.TimerThread(waitTime)) {
            this.job = job;
        }
        protected override void OnClose() {
            try {
                this.job.Close(this, null);
                this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 221, string.Format("{0}Close成功!", job.GetType().FullName));
            } catch (Exception ex) {
                this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 222, new object[] { string.Format("{0}Close方法错误\r\n{1}!", job.GetType().FullName, ex.ToString()), ex });
            }
        }

        protected override void FirstRun() {
            try {
                this.job.Init(this, null);
                this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 204, string.Format("{0}Init成功!", job.GetType().FullName));
            } catch (Exception ex) {
                this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 210, new object[] { string.Format("{0}Init方法错误\r\n{1}!", job.GetType().FullName, ex.ToString()), ex });
                throw ex;
            }
            this.CallProcessEventSimple(GCL.IO.Log.LogType.RELEASE, 201, "TimerJobThreadServer启动成功!");
        }

        protected override void Action(object sender, GCL.Event.EventArg e) {
            try {
                this.job.Action(this, null);
                this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 202, string.Format("{0}执行成功!", job.GetType().Name));
            } catch (Exception ex) {
                this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 203, new object[] { string.Format("{0}执行失败\r\n{1}!", job.GetType().Name, ex.ToString()), ex });
                throw ex;
            }
        }

        public new void CallProcessEventSimple(LogType logtype, int num, params object[] objs) {
            base.CallProcessEventSimple(logtype, num, objs);
        }
    }
}
