using System.Collections.Generic;

namespace SetsCache.Tests
{
  public abstract class SearchBase<T>
  {
    public IList<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
  }

  public class ListingSearch : SearchBase<Listing>
  {
    [CacheMember(1)]
    public PropertyType PropertyType { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    [CacheMember(2)]
    public int Bedrooms { get; set; }
    [CacheMember(3)]
    public int Bathrooms { get; set; }
    [CacheMember(4)]
    public string City { get; set; }
    [CacheMember(5)]
    public string State { get; set; }
    [CacheMember(6)]
    public string Zip { get; set; }
  }
}