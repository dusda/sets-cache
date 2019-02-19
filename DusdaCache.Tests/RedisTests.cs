using System;
using Xunit;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace DusdaCache.Tests
{
  public class RedisTests
  {
    IServiceProvider services;
    public RedisTests()
    {
      services = new ServiceCollection()
        .AddDistributedRedisCache(o =>
        {
          o.Configuration = "localhost:6388";
          o.InstanceName = "Tests";
        })
        .AddSingleton<CacheMemberSerializer>()
        .AddScoped<Redis.IDusdaCache, Redis.DisduCache>()
        .BuildServiceProvider();
    }

    [Fact]
    public void RedisRunning()
    {
      var cache = services.GetService<IDistributedCache>();

      try
      {
        var item = cache.Get("bleh"); //should return null
      }
      catch (StackExchange.Redis.RedisConnectionException)
      {
        throw new Exception("Looks like redis isn't running, did you run docker-compose up?");
      }

      Assert.True(true);
    }

    [Fact]
    public async Task GetSubGeneric()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var sub = stuff.sub;

      var res = await cache.GetSub(search, sub);

      Assert.Equal(sub.Nearby, res.Nearby);
      Assert.Equal(sub.Views, res.Views);
    }

    [Fact]
    public async Task GetSetWithKey()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var sub = stuff.sub;
      var key = "222-Portland-OR-97209";

      var res = await cache.GetSub<ListingSearchItems>(key);

      Assert.Equal(sub.Nearby, res.Nearby);
      Assert.Equal(sub.Views, res.Views);
    }

    [Fact]
    public async Task GetSetWithKeyOnly()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var items = stuff.sub;

      string key = "222-Portland-OR-97209";
      var res = await cache.GetSub<ListingSearchItems>(key);

      Assert.Equal(items.Nearby, res.Nearby);
      Assert.Equal(items.Views, res.Views);
    }

    /// <summary>
    /// Sets up test data, returns a tuple with everything needed in the tests here.
    /// </summary>
    async Task<(
      Redis.IDusdaCache cache,
      ListingSearch search,
      ListingSearchItems sub)>
    Set()
    {
      var cache = services.GetService<Redis.IDusdaCache>();

      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 2,
        Bathrooms = 2,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var sub = new ListingSearchItems
      {
        Views = 1,
        Nearby = new string[]
         {
           "Beaverton",
           "Gresham"
         }
      };

      await cache.SetSub(search, sub);

      return (cache, search, sub);
    }
  }
}