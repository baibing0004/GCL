namespace GCL.Collections.Pool {

    /// <summary>
    /// KeyValuePool工厂
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public interface IKeyValuePoolFactory<K> {

        /// <summary>
        ///处理建值池的新建操作 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        KeyValuePool CreateKeyValuePool(K key);
    }
}
