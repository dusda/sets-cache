using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using DusdaCache;
using System;
using DusdaCache.Extensions;

namespace DusdaCache.Redis
{
  public class DisduCache : ISetsCache
  {
    IDistributedCache _cache;
    CacheMemberSerializer _serializer;
    public DisduCache(
      IDistributedCache cache,
      CacheMemberSerializer serializer)
    {
      _cache = cache;
      _serializer = serializer;
    }

    public async Task<T> Get<T>(
      string key,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
    {
      var bytes = await _cache.GetAsync(key, token);
      var res = bytes.Deserialize<T>();

      return res;
    }

    public async Task<T> Get<T>(
      T item,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
    {
      var key = _serializer.Get(item);
      var bytes = await _cache.GetAsync(key, token);
      item = bytes.Deserialize<T>();

      return item;
    }

    public async Task<TSub> GetSub<TSub>(
      string key,
      CancellationToken token = default(CancellationToken))
        where TSub : class, new()
    {
      if(!key.Contains(':'))
        key += ":" + _serializer.Get<TSub>();
      var bytes = await _cache.GetAsync(key, token);
      var res = bytes.Deserialize<TSub>();

      return res;
    }

    public async Task<TSub> GetSub<T, TSub>(
      T item,
      TSub sub,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
        where TSub : class, new()
    {
      var key = _serializer.Get(item, sub);
      var bytes = await _cache.GetAsync(key, token);
      sub = bytes.Deserialize<TSub>();

      return sub;
    }

    public async Task Set<T>(
      T item,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
    {
      var key = _serializer.Get(item);
      var bytes = item.ToBinaryArray();
      await _cache.SetAsync(key, bytes);
    }

    public async Task SetSub<T, TSub>(
      T item,
      TSub sub,
      CancellationToken token = default(CancellationToken))
        where T : class, new()
        where TSub : class, new()
    {
      var key = _serializer.Get(item, sub);
      var bytes = sub.ToBinaryArray();
      await _cache.SetAsync(key, bytes);
    }
  }
}