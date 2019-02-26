using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace SetsCache.Services
{
  public class SeoFiller
  {
    volatile ConnectionMultiplexer _connection;
    readonly RedisCacheOptions _options;
    readonly SemaphoreSlim _lock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
    readonly string _list = "seo";
    IDatabase _cache;
    ISetsCache _setsCache;
    ICacheMemberSerializer _serializer;

    public SeoFiller(
      IOptions<RedisCacheOptions> options,
      ICacheMemberSerializer serializer,
      ISetsCache setsCache)
    {
      _options = options.Value;
      _setsCache = setsCache;
      _serializer = serializer;
    }

    public async Task<long> FillAsync()
    {
      await ConnectAsync();

      var keyTask = _cache.ListLeftPopAsync(_list);
      var countTask = _cache.ListLengthAsync(_list);

      Task.WaitAll(keyTask, countTask);
      var count = countTask.Result;

      if (string.IsNullOrWhiteSpace(keyTask.Result))
        return count;

      string key = keyTask.Result;
      var search = _serializer.Parse<Tests.ListingSearch>(key);
      var seo = new Tests.ListingSearchSeo();

      //go search

      //get something like this back
      var items = new Tests.ListingSearchSeo
      {
        City = "Portland",
        State = "OR",
        Views = 354235
      };

      //cache it
      await _setsCache.SetSub(search, items);

      return count;
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