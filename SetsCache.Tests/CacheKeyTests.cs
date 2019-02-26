using System;
using Newtonsoft.Json;
using Xunit;

namespace SetsCache.Tests
{
  public class CacheKeyTests
  {
    [Fact]
    public void DefaultHashTest()
    {
      var serializer = new CacheMemberSerializer();
      var search = new ListingSearch();
      var hash = serializer.Get(search);

      Assert.Equal("######", hash);
    }

    [Fact]
    public void CacheTest()
    {
      var serializer = new CacheMemberSerializer();      
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Any,
        Bedrooms = 10,
        Bathrooms = 1,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var hash = serializer.Get(search);

      Assert.Equal("#A1-Portland-OR-97209", hash);
    }

    [Fact]
    public void CacheSetTest()
    {
      var serializer = new CacheMemberSerializer();
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 10,
        Bathrooms = 1
      };

      var set = new string[]
      {
        "2A1###",
        "#A1###",
        "2#1###",
        "##1###",
        "2A####",
        "#A####",
        "2#####",
        "######"
      };

      var res = serializer.GetSubsets(search);

      Assert.Equal(set, res);
    }
  }
}
