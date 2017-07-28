using System;
using System.Collections.Concurrent;

namespace WebsiteProvider.Helpers
{
    public class Storage<T>
    {
        private readonly ConcurrentDictionary<string, T> responses = new ConcurrentDictionary<string, T>();

        public string Put(T response)
        {
            var key = Guid.NewGuid().ToString("N");
            responses.TryAdd(key, response);
            return key;
        }

        public T Get(string key)
        {
            return responses.ContainsKey(key) ? responses[key] : default(T);
        }

        public T Remove(string key)
        {
            T result;
            responses.TryRemove(key, out result);
            return result;
        }
    }
}