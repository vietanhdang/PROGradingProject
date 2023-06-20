namespace BusinessLogic.Base
{
    public class CacheItem<T>
    {
        public T Value { get; }
        public DateTime ExpirationTime { get; }

        public CacheItem(T value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }
    public class MemoryCache<T> : ICache<T>
    {
        private readonly Dictionary<string, CacheItem<T>> _cache;

        public MemoryCache()
        {
            _cache = new Dictionary<string, CacheItem<T>>();
        }

        public bool Contains(string key)
        {
            return _cache.ContainsKey(key);
        }

        public T Get(string key)
        {
            if (_cache.TryGetValue(key, out var cacheItem))
            {
                if (cacheItem.ExpirationTime > DateTime.Now)
                {
                    return cacheItem.Value;
                }
                else
                {
                    _cache.Remove(key);
                }
            }

            return default(T);
        }

        public void Set(string key, T value, TimeSpan expirationTime)
        {
            // mặc định cache 12 tiếng
            if (expirationTime == default)
            {
                expirationTime = TimeSpan.FromHours(12);
            }
            var cacheItem = new CacheItem<T>(value, DateTime.Now.Add(expirationTime));
            _cache[key] = cacheItem;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
