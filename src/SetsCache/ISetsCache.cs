using System.Threading;
using System.Threading.Tasks;

namespace SetsCache
{
    public interface ISetsCache
  {
    Task<T> Get<T>(
      string key,
      CancellationToken token = default(CancellationToken))
        where T : class, new();

    Task<T> Get<T>(
      T item,
      CancellationToken token = default(CancellationToken))
        where T : class, new();

    Task<TSub> GetSub<TSub>(
      string key,
      CancellationToken token = default(CancellationToken))
        where TSub : class, new();

    Task<TSub> GetSub<T, TSub>(
      T item, TSub sub,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
        where TSub : class, new();

    Task Set<T>(
      T item,
      CancellationToken token = default(CancellationToken))
        where T : class, new();

    Task SetSub<T, TSub>(
      T item, TSub sub,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
        where TSub : class, new();
  }
}