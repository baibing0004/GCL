namespace GCL.Collections.Pool {

    /// <summary>
    /// KeyValuePool����
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public interface IKeyValuePoolFactory<K> {

        /// <summary>
        ///����ֵ�ص��½����� 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        KeyValuePool CreateKeyValuePool(K key);
    }
}
