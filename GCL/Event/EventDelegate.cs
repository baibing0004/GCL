using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Event {

    /// <summary>
    /// ��System.EventHandleͬ��ʵ�ֵķ���Ҳһ���������ڶ����������ͱ����EventArg����
    /// ע����winForm�ж����ⲿ����¼�ʱ �䴦���¼�����һ��Ҫ�� 
    /// this.invoke(delegete �¼���ί���� �����ӿ���  ,new object[]{} �¼�������˳������)
    /// �����������¼������� ��ֹ���ֽ����߳����� ��Ϊ�����ӿ�������ʱ�൱�����̵߳�һ������ ���Բ���ֱ�ӵ����ⲿ�̵߳���Դ
    /// Java�л�û�г���������� ������Ϊʹ���¼����� ���� �䷽������ͨ���ӿڵ��õ�û��ָ�� ���Բ��ᷢ�����ϵĴ���
    /// </summary>
    /// <param name="sender">������</param>
    /// <param name="e">����ֵ</param>
    public delegate void EventHandle(object sender, EventArg e);
    public delegate void EEventHandle<E>(object sender, E e);


    /// <summary>
    /// ��̬�����������
    /// </summary>
    /// <param name="param"></param>
    public delegate void DynamicEventFunc(params object[] param);

    /// <summary>
    /// ��̬����������
    /// </summary>
    /// <param name="param"></param>
    public delegate T DynamicFunc<T>(params object[] param);


    /// <summary>
    /// ��̬��ʽ����������� ֻ�з���trueʱ �ſ��Լ�������
    /// </summary>
    /// <param name="param"></param>
    public delegate bool PopDynamicEventFunc(params object[] param);

    /// <summary>
    /// ˵��������һ���¼����ǲ�����Է�����������
    /// </summary>
    /// <param name="e">����ֵ</param>
    public delegate void CommonEventHandle(EventArg e);

    /// <summary>
    /// ���������ֻΪ˵��һ��������� �������κβ��� Ҳ�Ͳ���������ȡ�����κβ���
    /// </summary>
    public delegate void CommandEventHandle();

    /// <summary>
    /// ���ڻ�ȡһ����KeyValue�����ṩ���Զ���������ʹ��
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public delegate TSource KeyValueFunction<TSource>(TSource key);

    /// <summary>
    /// ���ڻ�ȡһ����KeyValue�����ṩ���Զ���������ʹ��
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
