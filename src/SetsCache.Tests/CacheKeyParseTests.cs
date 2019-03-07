using Xunit;

namespace SetsCache.Tests
{
  public class CacheKeyParseTests
  {
    [Fact]
    public void Parses()
    {
      var serializer = new CacheMemberSerializer();
      var key = "#a1-Portland-OR-97209";
      var item = serializer.Parse<ListingSearch>(key);

      Assert.Equal(0, (int)item.PropertyType);
      Assert.Equal(10, item.Bedrooms);
      Assert.Equal(1, item.Bathrooms);
      Assert.Equal("Portland", item.City);
      Assert.Equal("OR", item.State);
      Assert.Equal("97209", item.Zip);
    }

    [Fact]
    public void ParsesOnlyStrings()
    {
      var serializer = new CacheMemberSerializer();
      var key = "Sandy-UT";
      var item = serializer.Parse<StringsOnlySearch>(key);

      Assert.Equal("Sandy", item.City);
      Assert.Equal("UT", item.State);
    }

    [Fact]
    public void ParsesOnlyInts()
    {
      var serializer = new CacheMemberSerializer();
      var key = "123";
      var item = serializer.Parse<IntsOnlySearch>(key);

      Assert.Equal(1, item.Beds);
      Assert.Equal(2, item.Baths);
      Assert.Equal(3, (int)item.PropertyType);
    }
  }
}