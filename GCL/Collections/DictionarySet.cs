using System;
using System.Collections;
using System.Text;

namespace GCL.Collections {

    /// <summary>
    ///  默认使用Dictionary<object,object>作为键值对应匹配
    /// </summary>
    public class DictionarySet : ICloneable, ICollection, IList, IEnumerable {
        private IDictionary data;
        public DictionarySet(int capacity)
            : this(new System.Collections.Generic.Dictionary<object, object>()) {
        }

        public DictionarySet()
            : this(new System.Collections.Generic.Dictionary<object, object>()) {
        }

        public DictionarySet(IDictionary dic) {
            this.data = dic;
        }

        #region ICloneable Members

        public object Clone() {
            return new DictionarySet(((ICloneable)this.data).Clone() as IDictionary);
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index) {

            this.data.CopyTo(array, index);
        }

        public int Count {
            get { return this.data.Count; }
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public object SyncRoot {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator() {
            return this.data.Keys.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value) {
            lock (this) {
                this.data.Add(value, value);
                return this.data.Count;
            }
        }

        public void Clear() {
            lock (this) {
                this.data.Clear();
            }
        }

        public bool Contains(object value) {
            lock (this) {
                return this.data.Contains(value);
            }
        }

        public int IndexOf(object value) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, object value) {
            this.Add(value);
        }

        public bool IsFixedSize {
            get { return this.data.IsFixedSize; }
        }

        public bool IsReadOnly {
            get { return this.data.IsReadOnly; }
        }

        public void Remove(object value) {
            lock (this) {
                this.data.Remove(value);
            }
        }

        public void RemoveAt(int index) {
            throw new Exception("The method or operation is not implemented.");
        }

        public object this[int index] {
            get {
                throw new Exception("The method or operation is not implemented.");
            }
            set {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }

    /// <summary>
    /// 默认使用Hashtable作为键值对应匹配
    /// </summary>
    public class HashSet : DictionarySet {
        public HashSet(int capacity)
            : base(new Hashtable(capacity)) {
        }

        public HashSet()
            : base(new Hashtable()) {
        }

        public HashSet(IDictionary dic)
            : base(dic) {
        }
    }
}
