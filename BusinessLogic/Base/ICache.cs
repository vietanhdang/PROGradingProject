namespace BusinessLogic.Base
{
    public interface ICache<T>
    {
        bool Contains(string key);
        T Get(string key);
        void Set(string key, T value, TimeSpan expirationTime = default);
        void Remove(string key);
        void Clear();
    }
}
