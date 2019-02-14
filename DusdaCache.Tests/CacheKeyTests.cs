using System;
using Newtonsoft.Json;
using Xunit;

namespace DusdaCache.Tests
{
  public class CacheKeyTests
  {
    [Fact]
    public void HashTest()
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
    public void DefaultHashTest()
    {
      var search = new ListingSearch();
      var hash = CacheMemberSerializer.GetHash(search);

      Assert.Equal("######", hash);
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

    void Bleh()
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
      var set = CacheMemberSerializer.GetHashSet(search);

      var items = new int[] { 1, 2, 3 };
      var res = SubsetSolver.Solve(items);

      var json = JsonConvert.SerializeObject(res, Formatting.Indented);
      Console.WriteLine(json);
    }
  }
}
