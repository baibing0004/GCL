using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Event {

    /// <summary>
    /// 与System.EventHandle同名实现的方法也一样，不过第二个参数类型变成了EventArg对象
    /// 注意在winForm中订阅外部类的事件时 其处理事件函数一定要用 
    /// this.invoke(delegete 事件的委托类 方法接口类  ,new object[]{} 事件参数按顺序排列)
    /// 调用真正的事件处理方法 防止出现交叉线程引用 因为方法接口在运行时相当于子线程的一个方法 所以不能直接调用外部线程的资源
    /// Java中还没有出现这个错误 可能因为使用事件较少 或者 其方法都是通过接口调用的没有指针 所以不会发生以上的错误
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">参数值</param>
    public delegate void EventHandle(object sender, EventArg e);
    public delegate void EEventHandle<E>(object sender, E e);


    /// <summary>
    /// 动态事务命令处理方法
    /// </summary>
    /// <param name="param"></param>
    public delegate void DynamicEventFunc(params object[] param);

    /// <summary>
    /// 动态方法处理方法
    /// </summary>
    /// <param name="param"></param>
    public delegate T DynamicFunc<T>(params object[] param);


    /// <summary>
    /// 动态链式事务命令处理方法 只有返回true时 才可以继续进行
    /// </summary>
    /// <param name="param"></param>
    public delegate bool PopDynamicEventFunc(params object[] param);

    /// <summary>
    /// 说明发生了一个事件但是不允许对发生者作操作
    /// </summary>
    /// <param name="e">参数值</param>
    public delegate void CommonEventHandle(EventArg e);

    /// <summary>
    /// 纯命令参数只为说明一个命令发生了 不传递任何参数 也就不允许程序采取方法任何操作
    /// </summary>
    public delegate void CommandEventHandle();

    /// <summary>
    /// 用于获取一个简单KeyValue方法提供给自定义类属性使用
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public delegate TSource KeyValueFunction<TSource>(TSource key);

    /// <summary>
    /// 用于获取一个简单KeyValue方法提供给自定义类属性使用
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class KeyValueClass<TSource> {
        private KeyValueFunction<TSource> kvf;
        public KeyValueClass(KeyValueFunction<TSource> func) {
            this.kvf = func;
        }
        public TSource this[TSource key] {
            get { return this.kvf(key); }
        }
    };
}
