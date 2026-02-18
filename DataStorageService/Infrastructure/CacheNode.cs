namespace DataStorageService.Infrastructure
{
    /// <summary>
    /// Represents a node in the custom doubly linked list for the SDCS.
    /// This manual implementation avoids using built-in LinkedList collections.
    /// </summary>
    /// <typeparam name="T">The type of value stored.</typeparam>
    public class CacheNode<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }

        public CacheNode<T>? Next { get; set; }
        public CacheNode<T>? Previous { get; set; }

        public CacheNode(string key, T value)
        {
            Key = key;
            Value = value;
        }
    }
}
