namespace SetsCache.Tests
{
  public class IntsOnlySearch
  {
    [CacheMember(1)]
    public int Beds { get; set; }
    [CacheMember(2)]
    public int Baths { get; set; }
    [CacheMember(3)]
    public PropertyType PropertyType { get; set; }
  }
}