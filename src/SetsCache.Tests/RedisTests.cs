using System;
using Xunit;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SetsCache.Tests
{
  public class RedisTests
  {
    IServiceProvider services;
    public RedisTests()
    {
      services = new ServiceCollection()
        .AddStackExchangeRedisCache(o =>
        {
          o.Configuration = "localhost:6388";
          o.InstanceName = "RedisTests";
        })
        .AddSingleton<ICacheMemberSerializer, CacheMemberSerializer>()
        .AddScoped<ISetsCache, Redis.RedisSetsCache>()
        .BuildServiceProvider();

      //check that redis is running
      var cache = services.GetService<IDistributedCache>();
      try
      {
        var item = cache.Get("bleh"); //should return null
      }
      catch (StackExchange.Redis.RedisConnectionException)
      {
        throw new Exception("Looks like redis isn't running, did you run docker-compose up?");
      }
    }

    [Fact]
    public async Task GetSubGeneric()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var sub = stuff.sub;

      var res = await cache.GetSub(search, sub);

      Assert.Equal(sub.City, res.City);
      Assert.Equal(sub.State, sub.State);
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

      var res = await cache.GetSub<ListingSearchSeo>(key);

      Assert.Equal(sub.City, res.City);
      Assert.Equal(sub.State, res.State);
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
      var res = await cache.GetSub<ListingSearchSeo>(key);

      Assert.Equal(items.City, res.City);
      Assert.Equal(items.State, res.State);
      Assert.Equal(items.Views, res.Views);
    }

    [Fact]
    public async Task GetSetWithKeyAndSubKeyOnly()
    {
      var stuff = await Set();
      var cache = stuff.cache;
      var search = stuff.search;
      var items = stuff.sub;

      string key = "222-Portland-OR-97209:ListingSearchSeo";
      var res = await cache.GetSub<ListingSearchSeo>(key);

      Assert.Equal(items.City, res.City);
      Assert.Equal(items.State, res.State);
      Assert.Equal(items.Views, res.Views);
    }

    [Fact]
    public void Subsets()
    {
      var _serializer = services.GetService<ICacheMemberSerializer>();
      var _cache = services.GetService<ISetsCache>();

      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 3,
        Bathrooms = 2,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var m = (city: "Portland", state: "OR", count: 2);

      //[items] should link to stuff, like [apartments] = /places-for-rent/portland/or?PropertyTypes=2
      var title = $"{m.count} Places for Rent in {m.city}, {m.state} | Rentler";
      var template = "Rentler makes it easy to find houses or apartments for " +
        $"rent in {m.city}, {m.state}. Unlike many other rental sites, Rentler " +
        "lets you search [houses], [townhomes], [condos], or [apartments] for " +
        "rent - all in one place. And our easy to use search can help you find " +
        "somewhere with just the right amenities, whether you want a " +
        "[house with a washer and dryer], or an [apartment with a pool].";

      var keys = _serializer.GetSubsets(search);
    }

    /// <summary>
    /// Sets up test data, returns a tuple with everything needed in the tests here.
    /// </summary>
    async Task<(
      ISetsCache cache,
      ListingSearch search,
      ListingSearchSeo sub)>
    Set()
    {
      var cache = services.GetService<ISetsCache>();

      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 2,
        Bathrooms = 2,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var sub = new ListingSearchSeo
      {
        Views = 1,
        City = "Portland",
        State = "OR"
      };

      await cache.SetSub(search, sub);

      return (cache, search, sub);
    }
  }
}