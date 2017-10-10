using System;

namespace GCL.Module.Trigger {
    /// <summary>
    /// ������ģʽ����������ʹ���������װ����Դ�෽��ʵ��Ŀ�ĵķ�����ͨ��������������������Դ��ʵ��Ŀ�ġ�
    /// ��������������������������ڶ�Դ�෽���ļ�¼���޸ĺ��ύ���������������ͳһ�ӿڵ��ύ��
    /// @author baibing
    /// @version 2.0.81209.1
    /// </summary>
    public interface ITriggerVisiter {
        void Action(ATrigger trigger);
    }

    /// <summary>
    /// ��װTrigger.reSet����
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
    /// ��װTrigger.SetEnable����
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
    /// ��װTrigger.Taste����
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
    /// ��װTrigger.Dispose����
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