using System;
using Newtonsoft.Json;
using Xunit;

namespace DusdaCache.Tests
{
  public class CacheKeyTests
  {
    [Fact]
    public void DefaultHashTest()
    {
      var search = new ListingSearch();
      var hash = CacheMemberSerializer.GetHash(search);

      Assert.Equal("######", hash);
    }

    [Fact]
    public void CacheTest()
    {
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Any,
        Bedrooms = 10,
        Bathrooms = 1,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var hash = CacheMemberSerializer.GetHash(search);

      Assert.Equal("#a1-portland-or-97209", hash);
    }

    [Fact]
    public void CacheSetTest()
    {
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Apartment,
        Bedrooms = 10,
        Bathrooms = 1
      };

      var set = new string[]
      {
        "2a1###",
        "#a1###",
        "2#1###",
        "##1###",
        "2a####",
        "#a####",
        "2#####",
        "######"
      };

      var res = CacheMemberSerializer.GetHashSet(search);

      Assert.Equal(set, res);
    }

    [Fact]
    public void CacheMemberThrowsException()
    {
      var search = new ListingSearch
      {
        Bedrooms = 16
      };

      Action action = () => CacheMemberSerializer.GetHash(search);

      //Currently only supports ints up to 15, since it's stored as hex.
      Assert.Throws<ArgumentException>(action);
    }
  }
}
