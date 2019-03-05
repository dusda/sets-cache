namespace SetsCache.Tests
{
  public class SomeSearchNullable
  {
    [CacheMember(1)]
    public PropertyType? PropertyType { get; set; }
    [CacheMember(2)]
    public int Bedrooms { get; set; }
    [CacheMember(3)]
    public int Bathrooms { get; set; }
  }
}