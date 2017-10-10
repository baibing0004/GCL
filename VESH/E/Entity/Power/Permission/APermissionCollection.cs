using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.E.Entity.Power.Permission {
    /// <summary>
    /// C121221.1.2
    /// </summary>
    public abstract class APermissionCollection {

        private string ids;
        public APermissionCollection(string ids) {
            this.ids = ids;
        }

        /// <summary>
        /// 权限值
        /// </summary>
        public string PermissionCollections {
            get { return this.ids; }
        }

        /// <summary>
        /// 用于判断是否拥有权限
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool HasRight(string key);
    }
}
