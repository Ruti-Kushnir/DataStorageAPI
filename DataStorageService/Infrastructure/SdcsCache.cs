namespace DataStorageService.Infrastructure
{
    /// <summary>
    /// Self-Designed Caching System (SDCS) with LRU eviction policy.
    /// Capacity: 3-100 items. Thread-safe implementation.
    /// </summary>
    public class SdcsCache<T>
    {
        private readonly int _capacity;
        private readonly Dictionary<string, CacheNode<T>> _map;
        private CacheNode<T>? _head;
        private CacheNode<T>? _tail;
        private readonly object _lock = new();

        public SdcsCache(int capacity)
        {
            // דרישה: קיבולת בין 3 ל-100
            if (capacity < 3 || capacity > 100)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be between 3 and 100.");

            _capacity = capacity;
            _map = new Dictionary<string, CacheNode<T>>(capacity);
        }

        /// <summary>
        /// Retrieves a value associated with the specified key from the cache.
        /// Accessing an item moves it to the front of the LRU (Least Recently Used) list, 
        /// marking it as the most recently used.
        /// </summary>
        /// <param name="key">The unique identifier for the cached item.</param>
        /// <returns>The cached value of type T if found; otherwise, the default value for T.</returns>
        public T? Get(string key)
        {
            lock (_lock)
            {
                if (!_map.TryGetValue(key, out var node))
                    return default;

                MoveToHead(node);
                return node.Value;
            }
        }

        /// <summary>
        /// Adds an item to the cache using LRU (Least Recently Used) policy.
        /// If capacity is reached, the least recently used item is evicted.
        /// </summary>
        public void Put(string key, T value)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(key, out var node))
                {
                    node.Value = value;
                    MoveToHead(node);
                }
                else
                {
                    if (_map.Count >= _capacity)
                    {
                        RemoveLeastRecentlyUsed();
                    }

                    var newNode = new CacheNode<T>(key, value);
                    AddNodeAtHead(newNode);
                    _map[key] = newNode;
                }
            }
        }

        private void MoveToHead(CacheNode<T> node)
        {
            RemoveNode(node);
            AddNodeAtHead(node);
        }

        private void AddNodeAtHead(CacheNode<T> node)
        {
            node.Next = _head;
            node.Previous = null;

            if (_head != null) _head.Previous = node;
            _head = node;

            if (_tail == null) _tail = node;
        }

        private void RemoveNode(CacheNode<T> node)
        {
            if (node.Previous != null) node.Previous.Next = node.Next;
            else _head = node.Next;

            if (node.Next != null) node.Next.Previous = node.Previous;
            else _tail = node.Previous;
        }

        private void RemoveLeastRecentlyUsed()
        {
            if (_tail == null) return;
            _map.Remove(_tail.Key);
            RemoveNode(_tail);
        }
    }
}
