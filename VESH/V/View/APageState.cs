using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace GCL.Project.VESH.V.View {

    /// <summary>
    /// C121221.1.3
    /// 用于处理复杂页面的状态，其使用方法如下，某页面在继承Page的同时实现接口IPageStateUser<E> 其中E为其具体页面类名，从而保持一个APageState状态的实例。调用此实例的OnLoad方法用于调用具体页面空间实现页面在该状态下的初始化显示。
    /// 然后在类内部嵌套定义 APageState的子类，其中E为页面类名。用于定义和处理页面的不同状态！这样页面内部通过调用自己状态的OnLoad方法和Excute方法（比如页面提交或者各种刷新）实现页面在不同状态下的互相作用，当然各个状态需要管理页面状态的转变。并已经提供了新的方法如（User.SetPageState(新状态)）
    /// </summary>
    public abstract class APageState<E> {
        private E page;
        /// <summary>
        /// 获得页面对象
        /// </summary>
        protected E Source {
            get { return page; }
        }
        /// <summary>
        /// 获得页面状态用户
        /// </summary>
        protected IPageStateUser<E> User {
            get { return page as IPageStateUser<E>; }
        }
        public APageState(E page) {
            if (!(page is IPageStateUser<E>))
                throw new InvalidOperationException("此页面不是IPageStateUser的子类!");
            this.page = page;

        }

        /// <summary>
        /// 一般的用来处理装载事件
        /// </summary>
        public abstract void OnLoad(object sender, EventArgs e);

        /// <summary>
        /// 一般的用来处理执行事件
        /// </summary>
        public abstract void Excute(object sender, EventArgs e, params object[] paras);
    }

    /// <summary>
    /// 状态设置者 允许状态设置状态
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IPageStateUser<E> {
        void SetPageState(APageState<E> state);
        APageState<E> GetPageState();
    }
}
