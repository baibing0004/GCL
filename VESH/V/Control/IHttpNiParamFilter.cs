using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;

namespace GCL.Project.VESH.V.Control {

    /// <summary>
    /// 120225.1.5
    /// 用于提前处理和过滤这个请求的数据
    /// </summary>
    public interface IHttpNiParamFilter {
        void Filt(HttpContext context, IDictionary idic);
    }
}
