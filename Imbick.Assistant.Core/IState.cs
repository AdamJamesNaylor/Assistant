
namespace Imbick.Assistant.Core {
    using System;
    using System.Threading.Tasks;

    public interface IState {
        void Set(string key, string value);
        Task<string> Get(string key);
        Task<long> ListLength(string key);
        Task<T> ListGetByIndex<T>(string key, long index);
        void ListSetByIndex<T>(string key, long index, T value);
        void ListRemove<T>(string key, long count, T value);
        Task<bool> ListContains<T>(string key, T value, Func<T, T, bool> comparison);
    }
}