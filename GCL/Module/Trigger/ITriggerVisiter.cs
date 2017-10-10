using System;

namespace GCL.Module.Trigger {
    /// <summary>
    /// 访问者模式，访问者类使用其子类封装调用源类方法实现目的的方法，通过被访问者适配器调用源类实现目的。
    /// 与命令类的区别在于命令类用于对源类方法的纪录，修改和提交；访问者类则侧重统一接口的提交。
    /// @author baibing
    /// @version 2.0.81209.1
    /// </summary>
    public interface ITriggerVisiter {
        void Action(ATrigger trigger);
    }

    /// <summary>
    /// 封装Trigger.reSet方法
    /// 
    /// @author baibing
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class ReSetTriggerVisiter : ITriggerVisiter {

        #region ITriggerVisiter Members

        public void Action(ATrigger trigger) {
            trigger.ReSet();
        }

        #endregion
    }

    /// <summary>
    /// 封装Trigger.SetEnable方法
    /// 
    /// @author baibing
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class SetEnableTriggerVisiter : ITriggerVisiter {

        private bool isEnable = false;

        public bool IsEnable {
            get { return isEnable; }
            set { isEnable = value; }
        }

        public SetEnableTriggerVisiter(bool isEnable) {
            this.isEnable = isEnable;
        }
        public SetEnableTriggerVisiter() {
        }
        #region ITriggerVisiter Members

        public void Action(ATrigger trigger) {
            trigger.SetEnable(isEnable);
        }

        #endregion
    }

    /// <summary>
    /// 封装Trigger.Taste方法
    /// 
    /// @author baibing
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class TasteTriggerVisiter : ITriggerVisiter {

        private Event.EventArg e;

        public Event.EventArg EventArg {
            get { return e; }
            set { e = value; }
        }

        bool isActive = false;

        public bool IsActive {
            get { return isActive; }
            set { isActive = value; }
        }

        #region ITriggerVisiter Members

        public void Action(ATrigger trigger) {
            if (Common.Tool.IsEnable(e))
                isActive |= trigger.Taste(e);
            else
                isActive |= trigger.Taste();
        }

        #endregion
    }

    /// <summary>
    /// 封装Trigger.Dispose方法
    /// 
    /// @author baibing
    /// @version 2.0.81209.1
    /// 
    /// </summary>
    public class DisposeTriggerVisiter : ITriggerVisiter {

        #region ITriggerVisiter Members

        public void Action(ATrigger trigger) {
            trigger.Dispose();
        }

        #endregion
    }
}