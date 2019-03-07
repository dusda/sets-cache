namespace SetsCache.Tests
{
  public class StringsOnlySearch
  {
    [CacheMember(1)]
    public string City { get; set; }
    [CacheMember(2)]
    public string State { get; set; }
  }
}