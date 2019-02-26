using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace SetsCache.Services
{
  public class SeoService
  {
    ISetsCache _setsCache;
    ICacheMemberSerializer _serializer;
    readonly string _list = "seo";
    volatile ConnectionMultiplexer _connection;
    readonly RedisCacheOptions _options;
    IDatabase _cache;

    readonly SemaphoreSlim _lock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

    public SeoService(
      ISetsCache cache,
      ICacheMemberSerializer serializer,
      IOptions<RedisCacheOptions> options)
    {
      _setsCache = cache;
      _serializer = serializer;
      _options = options.Value;
    }
    public async Task<T> GetData<T>(T item)
      where T : class, new()
    {
      var keys = _serializer.GetSubsets(item);
      var data = await _setsCache.Get<T>(keys[0]);

      if (data == null)
        await Push(item);

      return data;
    }

    public async Task<TSub> GetData<T, TSub>(T item)
      where T : class, new()
      where TSub : class, new()
    {
      TSub sub = null;
      var key = _serializer.Get(item, sub);
      sub = await _setsCache.GetSub<TSub>(key);

      if (sub == null)
        await Push(item, sub);

      return sub;
    }

    async Task Push<T>(T item)
    {
      await ConnectAsync();

      var keys = _serializer.GetSubsets(item)
        .Select(f => (RedisValue)f)
        .ToArray();

      await _cache.ListRightPushAsync(_list, keys);
    }

    async Task Push<T, TSub>(T item, TSub sub)
    {
      await ConnectAsync();

      var keys = _serializer.GetSubsets(item, sub)
        .Select(f => (RedisValue)f)
        .ToArray();

      await _cache.ListRightPushAsync(_list, keys);
    }

    async Task ConnectAsync(CancellationToken token = default(CancellationToken))
    {
      token.ThrowIfCancellationRequested();

      if (_cache != null)
        return;

      await _lock.WaitAsync(token);
      try
      {
        if (_cache == null)
        {
          if (_options.ConfigurationOptions != null)
            _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
          else
            _connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration);

          _cache = _connection.GetDatabase();
        }
      }
      finally
      {
        _lock.Release();
      }
    }
  }
}