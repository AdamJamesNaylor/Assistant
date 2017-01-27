namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    public class RedisStateProvider
        : IState {
        private readonly ConnectionMultiplexer _redisConnection;

        public RedisStateProvider(string host, short port = 6379) {
            var configurationOptions = new ConfigurationOptions {EndPoints = {{host, port}}, DefaultDatabase = 1};
            _redisConnection = ConnectionMultiplexer.Connect(configurationOptions);
        }

        public void Set(string key, string value) {
            _redisConnection.GetDatabase().StringSetAsync(key, value);
        }

        public async Task<string> Get(string key) {
            return await _redisConnection.GetDatabase().StringGetAsync(key);
        }

        public async Task<long> ListLength(string key) {
            return await _redisConnection.GetDatabase().ListLengthAsync(key);
        }

        public async Task<T> ListGetByIndex<T>(string key, long index) {
            var value = await _redisConnection.GetDatabase().ListGetByIndexAsync(key, index);
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public void ListSetByIndex<T>(string key, long index, T value) {
            var serialisedValue = JsonConvert.SerializeObject(value);
            _redisConnection.GetDatabase().ListRightPush(key, serialisedValue);
        }

        public void ListRemove<T>(string key, long count, T value) {
            var serialisedValue = JsonConvert.SerializeObject(value);
            _redisConnection.GetDatabase().ListRemoveAsync(key, serialisedValue, count);
        }

        public async Task<bool> ListContains<T>(string key, T value, Func<T, T, bool> comparison) {
            var length = _redisConnection.GetDatabase().ListLength(key);
            for (var i = 0; i < length; i++) {
                var serialisedValue = await _redisConnection.GetDatabase().ListGetByIndexAsync(key, i);
                var deserialisedValue = JsonConvert.DeserializeObject<T>(serialisedValue);
                if (comparison(value, deserialisedValue))
                    return true;
            }
            return false;
        }

        public async Task<List<T>> GetList<T>(string key) {
            var value = await _redisConnection.GetDatabase().ListRangeAsync(key, 0, -1);
            return JsonConvert.DeserializeObject<T>(value.ToString());

        }
    }
}