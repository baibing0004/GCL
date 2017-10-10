using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.Event;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// Job职务
    /// </summary>
    public interface IJob {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Init(object sender, EventArg e);
        /// <summary>
        /// 定点执行的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Action(object sender, EventArg e);
        /// <summary>
        /// 任务关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Close(object sender, EventArg e);
    }

    public class AJobProxy : IJob {
        private IJob job;
        public AJobProxy(IJob job) {
            this.job = job;
        }
        #region IJob Members

        public void Init(object sender, EventArg e) {
            this.job.Init(sender, e);
        }

        public void Action(object sender, EventArg e) {
            this.job.Action(sender, e);
        }

        public void Close(object sender, EventArg e) {
            this.job.Close(sender, e);
        }

        #endregion
    }
}
