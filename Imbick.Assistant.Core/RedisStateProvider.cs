namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    public class RedisStateProvider
        : IState {

        //public RedisStateProvider(IConnectionMultiplexer connectionMultiplexer) {
        //    _connectionMultiplexer = connectionMultiplexer;
        //}

        public RedisStateProvider(string host, short port = 6379) {
            var configurationOptions = new ConfigurationOptions {EndPoints = {{host, port}}, DefaultDatabase = 1};
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }

        public void Set(string key, string value) {
            _connectionMultiplexer.GetDatabase().StringSetAsync(key, value);
        }

        public async Task<string> Get(string key) {
            return await _connectionMultiplexer.GetDatabase().StringGetAsync(key);
        }

        public async Task<long> ListLength(string key) {
            return await _connectionMultiplexer.GetDatabase().ListLengthAsync(key);
        }

        public async Task<T> ListGetByIndex<T>(string key, long index) {
            var value = await _connectionMultiplexer.GetDatabase().ListGetByIndexAsync(key, index);
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public void ListSetByIndex<T>(string key, long index, T value) {
            var serialisedValue = JsonConvert.SerializeObject(value);
            _connectionMultiplexer.GetDatabase().ListRightPush(key, serialisedValue);
        }

        public void ListRemove<T>(string key, long count, T value) {
            var serialisedValue = JsonConvert.SerializeObject(value);
            _connectionMultiplexer.GetDatabase().ListRemoveAsync(key, serialisedValue, count);
        }

        public async Task<bool> ListContains<T>(string key, T value, Func<T, T, bool> comparison) {
            var length = _connectionMultiplexer.GetDatabase().ListLength(key);
            for (var i = 0; i < length; i++) {
                var serialisedValue = await _connectionMultiplexer.GetDatabase().ListGetByIndexAsync(key, i);
                var deserialisedValue = JsonConvert.DeserializeObject<T>(serialisedValue);
                if (comparison(value, deserialisedValue))
                    return true;
            }
            return false;
        }

        public void ListAdd(string key, string value) {
            _connectionMultiplexer.GetDatabase().ListRightPush(key, value);
        }

        public async Task<List<T>> GetList<T>(string key) {
            var value = await _connectionMultiplexer.GetDatabase().ListRangeAsync(key);
            return JsonConvert.DeserializeObject<List<T>>(value.ToString());
        }

        private readonly IConnectionMultiplexer _connectionMultiplexer;
    }
}