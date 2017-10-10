using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GCL.Project.VESH.V.Control.Moduler {
    public interface IModuler : IDisposable {

        /// <summary>
        /// 是否是鉴权使用的
        /// </summary>
        bool IsSecurity { get; }

        /// <summary>
        /// 初始化 一般不用Session
        /// </summary>
        /// <param name="context"></param>
        void Init(HttpApplication context);

        /// <summary>
        /// 每次请求时执行
        /// </summary>
        /// <param name="context"></param>
        void BeginRequest(HttpContext context);

        /// <summary>
        /// 每次请求时执行
        /// </summary>
        /// <param name="context"></param>
        void EndRequest(HttpContext context);
    }
}
