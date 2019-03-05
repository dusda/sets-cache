namespace SetsCache.Tests
{
  //1-Portland-2-OR-5
  public class SomeSearch : SearchBase<Listing>
  {
    [CacheMember(1)]
    public PropertyType PropertyType { get; set; }
    [CacheMember(5)]
    public int Beds { get; set; }
    [CacheMember(3)]
    public int Baths { get; set; }
    [CacheMember(2)]
    public string City { get; set; }
    [CacheMember(4)]
    public string State { get; set; }
  }
}