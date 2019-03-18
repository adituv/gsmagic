////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace gsmagic {
////    class VirtList {
////    }
////}


//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace VirtualListWithPagingTechnique {
//    namespace VirtualLists {
//        public interface IObjectGenerator<T> {
//            /// <summary>
//            /// Returns the number of items in the collection.
//            /// </summary>
//            int Count { get; }

//            /// <summary>
//            /// Generate the item that is located on the specified index.
//            /// </summary>
//            /// <remarks>
//            /// This method is only be called once per index.
//            /// </remarks>
//            /// <param name="index">Index of the items that must be generated.</param>
//            /// <returns>Fresh new instance.</returns>
//            T CreateObject(int index);
//        }

//        /// <summary>
//        /// Virtual lists are lists from which the content is loaded on demand.
//        /// </summary>
//        /// <remarks>
//        /// Use visual lists if it is expensive to populate the list and only
//        /// a subset of the list's elements are used. The virtual list uses an
//        /// object generator to populate the list on demand.
//        /// </remarks>
//        /// <typeparam name="T">Objects that are stored inside the list.</typeparam>
//        public class VirtualList<T> : IList<T>, IList where T : class {
//            #region Internal attributes
//            /// <summary>
//            /// Object that is used to generate the requested objects.
//            /// </summary>
//            /// <remarks>
//            /// This object can also hold a IMultipleObjectGenerator reference.
//            /// </remarks>
//            private readonly IObjectGenerator<T> _generator;

//            /// <summary>
//            /// Internal array that holds the cached items.
//            /// </summary>
//            private readonly T[] _cachedItems;
//            #endregion

//            #region Constructor
//            /// <summary>
//            /// Create the virtual list.
//            /// </summary>
//            /// <param name="generator"></param>
//            public VirtualList(IObjectGenerator<T> generator) {
//                int maxItems = generator.Count; // Determine the number of items
//                _generator = generator; // Save generator and items
//                _cachedItems = new T[maxItems];
//            }
//            #endregion

//            #region IList<T> Members
//            public int IndexOf(T item) {
//                return IndexOf(item);
//            }
//            public void Insert(int index, T item) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public void RemoveAt(int index) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public T this[int index] {
//                get {
//                    if (!IsItemCached(index)) // Cache item if it isn't cached already
//                        CacheItem(index);
//                    return _cachedItems[index]; // Return the cached object
//                }
//                set { throw new NotSupportedException("VirtualList is a read-only collection."); }
//            }
//            #endregion

//            #region ICollection<T> Members
//            public void Add(T item) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public void Clear() {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public bool Contains(T item) {
//                return (IndexOf(item) != -1);
//            }
//            public void CopyTo(T[] array, int arrayIndex) {
//                _cachedItems.CopyTo(array, arrayIndex);
//            }
//            public int Count {
//                get { return _cachedItems.Length; }
//            }
//            public bool IsReadOnly {
//                get { return true; }
//            }
//            public bool Remove(T item) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            #endregion

//            #region IEnumerable<T> Members
//            public IEnumerator<T> GetEnumerator() {
//                return new VirtualEnumerator(this);
//            }
//            #endregion

//            #region IList Members
//            public int Add(object value) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public bool Contains(object value) {
//                throw new NotImplementedException();
//            }
//            public int IndexOf(object value) {
//                int items = _cachedItems.Length;
//                for (int index = 0; index < items; ++index) {
//                    if (_cachedItems[index].Equals(value)) // Check if item is found
//                        return index;
//                }
//                return -1; // Item not found
//            }
//            public void Insert(int index, object value) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            public bool IsFixedSize {
//                get { return true; }
//            }
//            public void Remove(object value) {
//                throw new NotSupportedException("VirtualList is a read-only collection.");
//            }
//            object IList.this[int index] {
//                get { return this[index]; }
//                set { throw new NotSupportedException("VirtualList is a read-only collection."); }
//            }
//            #endregion

//            #region ICollection Members
//            public void CopyTo(Array array, int index) {
//                _cachedItems.CopyTo(array, index);
//            }
//            public bool IsSynchronized {
//                get { return false; }
//            }
//            public object SyncRoot {
//                get { return this; }
//            }
//            #endregion

//            #region IEnumerable Members
//            IEnumerator IEnumerable.GetEnumerator() {
//                return new VirtualEnumerator(this);
//            }
//            #endregion

//            #region Internal helper methods required for caching
//            private bool IsItemCached(int index) {
//                return (_cachedItems[index] != null); // If the object is NULL, then it is empty
//            }
//            #endregion

//            public void CacheItem(int index) {
//                _cachedItems[index] = _generator.CreateObject(index); // Obtain only a single object
//            }

//            #region Internal IEnumerator implementation
//            private class VirtualEnumerator : IEnumerator<T> {
//                private readonly VirtualList<T> _collection;
//                private int _cursor;
//                public VirtualEnumerator(VirtualList<T> collection) {
//                    _collection = collection;
//                    _cursor = 0;
//                }
//                public T Current {
//                    get { return _collection[_cursor]; }
//                }
//                object IEnumerator.Current {
//                    get { return Current; }
//                }
//                public bool MoveNext() {
//                    if (_cursor == _collection.Count) // Check if we are behind
//                        return false;
//                    ++_cursor; // Increment cursor
//                    return true;
//                }
//                public void Reset() {
//                    _cursor = 0; // Reset cursor
//                }
//                public void Dispose() { // NOP
//                }
//            }
//            #endregion
//        }
//    }
//}