using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;
namespace GCL.Project.VESH.E.Entity.Power.Security {
    /// <summary>
    /// 用于管理多个ISecurity对象实例，其判断原则为与
    /// </summary>
    public class SecurityDecorater : ISecurity {

        public static ISecurity[] ToISecurity(object[] o) {
            return Array.ConvertAll<object, ISecurity>(o, ToISecurity);
        }

        private static ISecurity ToISecurity(object o) {
            return (ISecurity)o;
        }

        private ISecurity[] securities;
        public SecurityDecorater(ISecurity[] securities) {
            this.securities = securities;
            if (securities == null || securities.Length == 0)
                throw new InvalidOperationException("参数不能为空!");
        }

        #region ISecurity Members

        public void Login(SessionDataManager manager) {
            foreach (ISecurity ise in this.securities)
                ise.Login(manager);
        }

        public bool CheckLogin(SessionDataManager manager) {
            foreach (ISecurity ise in this.securities)
                if (!ise.CheckLogin(manager))
                    return false;
            return true;
        }

        public void Logout(SessionDataManager manager) {
            foreach (ISecurity ise in this.securities)
                ise.Logout(manager);
        }

        #endregion

    }
}
