using System.Threading.Tasks;

namespace DusdaCache.Services
{
  public class SeoService
  {
    ISetsCache _cache;
    ICacheMemberSerializer _serializer;
    public SeoService(
      ISetsCache cache,
      ICacheMemberSerializer serializer)
    {
      _cache = cache;
      _serializer = serializer;
    }
    public async Task<T> GetData<T>(T item)
      where T : class, new()
    {
      var keys = _serializer.GetSubsets(item);
      var data = await _cache.Get<T>(keys[0]);

      if (data == null)
        await Fill(item);

      return data;
    }

    public async Task<TSub> GetData<T, TSub>(T item, TSub sub)
      where T : class, new()
      where TSub : class, new()
    {
      var key = _serializer.Get(item, sub);
      var data = await _cache.GetSub<TSub>(key);

      if (data == null)
        await Fill(item, sub);

      return data;
    }

    async Task Fill<T>(T item)
    {
      var keys = _serializer.GetSubsets(item);

      //for each key, fill with appropriate data.
    }

    async Task Fill<T, TSub>(T item, TSub sub)
    {
      var keys = _serializer.GetSubsets(item, sub);

      //for each key, fill with appropriate data.
    }
  }
}