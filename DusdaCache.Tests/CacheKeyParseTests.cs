using Xunit;

namespace DusdaCache.Tests
{
  public class CacheKeyParseTests
  {
    [Fact]
    public void Parses()
    {
      var key = "#a1-Portland-OR-97209";
      var item = CacheMemberSerializer.Parse<ListingSearch>(key);

      Assert.Equal(0, (int)item.PropertyType);
      Assert.Equal(10, item.Bedrooms);
      Assert.Equal(1, item.Bathrooms);
      Assert.Equal("Portland", item.City);
      Assert.Equal("OR", item.State);
      Assert.Equal("97209", item.Zip);
    }
  }
}