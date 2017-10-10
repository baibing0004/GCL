using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.Event;
using GCL.Module.Trigger;
namespace GCL.Project.MyProcessController {

    /// <summary>
    /// 克隆表达式Job的基类
    /// </summary>
    public class CronExpressionJob : AJobProxy {

        private CronExpressionTrigger cron;
        /// <summary>
        /// 克隆表达式基类
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="job"></param>
        public CronExpressionJob(string cronExpression, IJob job)
            : base(job) {
            cron = new CronExpressionTrigger(cronExpression);
        }

        /// <summary>
        /// 克隆表达式基类
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="job"></param>
        public CronExpressionJob(string cronExpression, int randSec, IJob job)
            : base(job) {
            cron = new CronExpressionTrigger(cronExpression, randSec);
        }

        public CronExpressionTrigger Trigger {
            get {
                return cron;
            }
        }
    }

    /// <summary>
    /// 用于管理多个克隆表达式定义的触发器，任务
    /// </summary>
    public class CronJobThreadServer : ABusinessThreadServer {

        /// <summary>
        /// 记录间隔时间，最小30秒,单位毫秒
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="formatter"></param>
        public CronJobThreadServer(int waitTime, string formatter) : base(formatter, new GCL.Threading.TimerThread(waitTime)) { }

        /// <summary>
        /// 记录间隔时间，最小30秒，单位毫秒
        /// </summary>
        /// <param name="waitTime"></param>
        public CronJobThreadServer(int waitTime) : base(new GCL.Threading.TimerThread(waitTime)) { }

        private IList<CronExpressionJob> resList = new List<CronExpressionJob>();
        private LinkedList<CronExpressionJob> queryList = new LinkedList<CronExpressionJob>();
        public CronExpressionJob Job {
            set {
                try {
                    if (!resList.Contains(value)) {
                        value.Init(this, null);
                        resList.Add(value);
                        SetQueryOrder(value);
                        this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 204, string.Format("{0}Init成功!", value.GetType().FullName));
                    }
                } catch (Exception ex) {
                    this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 210, new object[] { string.Format("{0}Init方法错误\r\n{1}!", value.GetType().FullName, ex.ToString()), ex });
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 设置其检索顺序
        /// </summary>
        /// <param name="job"></param>
        protected virtual void SetQueryOrder(CronExpressionJob job) {
            if (queryList.Contains(job)) queryList.Remove(job);
            LinkedListNode<CronExpressionJob> j = queryList.First;
            //如果新Job的调用时间小于其它调用时间则插入
            while (queryList.Count > 0 && j != null && job.Trigger.PassTime.CompareTo(j.Value.Trigger.PassTime) > 0) j = j.Next;
            if (j != null && j.Value != null)
                queryList.AddBefore(j, job);
            else
                queryList.AddLast(job);
        }

        protected override void OnClose() {
            foreach (CronExpressionJob j in resList)
                try {
                    j.Close(this, null);
                    this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 221, string.Format("{0}Close成功!", j.GetType().FullName));
                } catch (Exception ex) {
                    this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 222, new object[] { string.Format("{0}Close方法错误\r\n{1}!", j.GetType().FullName, ex.ToString()), ex });
                }
        }


        protected override void FirstRun() {
            this.CallProcessEventSimple(GCL.IO.Log.LogType.RELEASE, 201, "CronThreadServer启动成功!");
        }

        protected override void Action(object sender, EventArg e) {
            while (queryList.Count > 0 && queryList.First.Value.Trigger.Taste()) {
                CronExpressionJob j = queryList.First.Value;
                try {
                    j.Action(this, null);
                    this.CallProcessEventSimple(GCL.IO.Log.LogType.DEBUG, 202, string.Format("{0}执行成功!", j.GetType().Name));
                } catch (Exception ex) {
                    this.CallProcessEventSimple(GCL.IO.Log.LogType.ERROR, 203, new object[] { string.Format("{0}执行失败\r\n{1}!", j.GetType().Name, ex.ToString()), ex });
                } finally {
                    SetQueryOrder(j);
                }
            }
        }

        #region 允许IJob对象可以在Action方法中 将sender转成CronJobThreadServer类方式进行事件调用 注意可能引起线程内对象交叉引用错误!


        public void CJSCallProcessEventSimple(Exception ex, object obj) {
            base.CallProcessEventSimple(ex, obj);
        }

        public void CJSCallProcessEventSimple(Exception ex, object[] obj) {
            base.CallProcessEventSimple(ex, obj);
        }

        public void CJSCallProcessEventSimple(GCL.IO.Log.LogType type, int eventNum, object obj) {
            base.CallProcessEventSimple(type, eventNum, obj);
        }

        public void CJSCallProcessEventSimple(GCL.IO.Log.LogType type, int eventNum, object[] objs) {
            base.CallProcessEventSimple(type, eventNum, objs);
        }

        public void CJSCallProcessEventSimple(GCL.Threading.Process.ProcessEventArg e) {
            base.CallProcessEventSimple(e);
        }

        public void CJSCallProcessEventSimple(GCL.Threading.Process.ProcessState state) {
            base.CallProcessEventSimple(state);
        }

        public void CJSCallProcessEventSimple(int eventNum, object obj) {
            base.CallProcessEventSimple(eventNum, obj);
        }

        public void CJSCallProcessEventSimple(int eventNum, object[] objs) {
            base.CallProcessEventSimple(eventNum, objs);
        }

        #endregion

    }
}
