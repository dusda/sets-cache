using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Xunit;

namespace DusdaCache.Tests
{
  public class SeoTests
  {
    IServiceProvider services;

    public SeoTests()
    {
      services = new ServiceCollection()
        .AddDistributedRedisCache(o =>
        {
          o.Configuration = "localhost:6388";
          o.InstanceName = "SeoTests:";
          o.ConfigurationOptions = ConfigurationOptions.Parse(o.Configuration);
          o.ConfigurationOptions.AllowAdmin = true;
        })
        .AddSingleton<ICacheMemberSerializer, CacheMemberSerializer>()
        .AddScoped<ISetsCache, Redis.DisduCache>()
        .AddScoped<Services.SeoService>()
        .AddScoped<Services.SeoFiller>()
        .BuildServiceProvider();
    }

    [Fact]
    public async Task PopulateList()
    {
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        City = "Portland",
        State = "OR"
      };
    
      Func<Task<IDatabase>> getDb = async () =>
      {
        var options = services.GetService<IOptions<RedisCacheOptions>>();
        var client = await ConnectionMultiplexer.ConnectAsync(options.Value.ConfigurationOptions);
        return client.GetDatabase();
      };

      try
      {
        var serializer = services.GetService<ICacheMemberSerializer>();
        var service = services.GetService<Services.SeoService>();
        var seo = await service.GetData<ListingSearch, ListingSearchSeo>(search);

        var db = await getDb();

        var length = await db.ListLengthAsync("seo");
        var subCount = serializer.GetSubsets(search, seo).LongLength;
        Assert.Equal(length, subCount);
      }
      finally
      {
        await Flush();
      }
    }

    [Fact]
    public async Task Fills()
    {
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        City = "Portland",
        State = "OR"
      };

      try
      {
        var serializer = services.GetService<ICacheMemberSerializer>();
        var service = services.GetService<Services.SeoService>();

        //ask for it, this will populate the seo list with keys
        var seo = await service.GetData<ListingSearch, ListingSearchSeo>(search);
        Assert.Null(seo);

        //fill
        var filler = services.GetService<Services.SeoFiller>();
        long length = await filler.FillAsync();
        while (length > 0)
          length = await filler.FillAsync();

        //ask agin, this time it will have it.
        seo = await service.GetData<ListingSearch, ListingSearchSeo>(search);
        Assert.NotNull(seo);
        Assert.Equal(seo.City, "Portland");
      }
      finally
      {
        await Flush();
      }
    }

    async Task Flush()
    {
      var options = services.GetService<IOptions<RedisCacheOptions>>();
      var client = await ConnectionMultiplexer.ConnectAsync(options.Value.ConfigurationOptions);
      var endpoints = client.GetEndPoints();
      var server = client.GetServer(endpoints[0]);
      await server.FlushAllDatabasesAsync();
    }
  }
}