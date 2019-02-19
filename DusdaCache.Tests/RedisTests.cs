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
        .AddTransient<DusdaCache.Redis.DusdaCache>()
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
    public async Task GetSetGetGeneric()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var items = stuff.items;

      var res = await cache.Get<ListingSearch, ListingSearchItems>(search);

      Assert.Equal(search, res.Item1);
      Assert.Equal(items.Nearby, res.Item2.Nearby);
      Assert.Equal(items.Views, res.Item2.Views);
    }

    [Fact]
    public async Task GetSetWithKey()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var items = stuff.items;

      var res = await cache.Get<ListingSearch, ListingSearchItems>("222-Portland-OR-97209");

      Assert.Equal(search.PropertyType, res.Item1.PropertyType);
      Assert.Equal(search.Bedrooms, res.Item1.Bedrooms);
      Assert.Equal(search.Bathrooms, res.Item1.Bathrooms);
      Assert.Equal(search.City, res.Item1.City);
      Assert.Equal(search.State, res.Item1.State);
      Assert.Equal(search.Zip, res.Item1.Zip);
      Assert.Equal(items.Nearby, res.Item2.Nearby);
      Assert.Equal(items.Views, res.Item2.Views);
    }

    [Fact]
    public async Task GetSetWithKeyOnly()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var items = stuff.items;

      string key = "222-Portland-OR-97209";
      var res = await cache.Get<ListingSearchItems>(key);

      Assert.Equal(items.Nearby, res.Nearby);
      Assert.Equal(items.Views, res.Views);
    }

    /// <summary>
    /// Sets up test data, returns a tuple with everything needed in the tests here.
    /// </summary>
    async Task<(DusdaCache.Redis.DusdaCache cache, ListingSearch search, ListingSearchItems items)> Set()
    {
      var cache = services.GetService<DusdaCache.Redis.DusdaCache>();

      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 2,
        Bathrooms = 2,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var items = new ListingSearchItems
      {
        Views = 1,
        Nearby = new string[]
         {
           "Beaverton",
           "Gresham"
         }
      };

      await cache.Set(search, items);

      return (cache, search, items);
    }
  }
}