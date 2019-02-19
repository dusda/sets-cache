using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using DusdaCache;
using System;
using DusdaCache.Extensions;

namespace DusdaCache.Redis
{
  public class DusdaCache
  {
    IDistributedCache _cache;
    CacheMemberSerializer _serializer;
    public DusdaCache(
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

    public async Task<(T, TT)> Get<T, TT>(
      string key, CancellationToken token = default(CancellationToken))
      where T : class, new()
      where TT : class, new()
    {
      var item = _serializer.Parse<T>(key);
      var bytes = await _cache.GetAsync(key, token);
      var res = bytes.Deserialize<TT>();

      return (item, res);
    }

    public async Task<(T, TT)> Get<T, TT>(
      T obj, CancellationToken token = default(CancellationToken))
    {
      var key = _serializer.Get<T>(obj);
      var bytes = await _cache.GetAsync(key, token);
      var item = bytes.Deserialize<TT>();

      return (obj, item);
    }

    public async Task Set<T, TT>(
      T obj, TT values,
      CancellationToken token = default(CancellationToken))
    {
      var key = _serializer.Get(obj);
      var bytes = values.ToBinaryArray();
      await _cache.SetAsync(key, bytes, token);
    }

    public async Task Set<T, TT>(
      (T, TT) obj,
      CancellationToken token = default(CancellationToken))
    {
      var key = _serializer.Get(obj.Item1);
      var bytes = obj.Item2.ToBinaryArray();
      await _cache.SetAsync(key, bytes, token);
    }
  }
}