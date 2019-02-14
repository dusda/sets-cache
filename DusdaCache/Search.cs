using System.Collections.Generic;

namespace DusdaCache
{
  public abstract class SearchBase<T>
  {
    public IList<T> Items {get;set;}
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public abstract string Hash();
  }

  public class ListingSearch : SearchBase<Listing>
  {
    [HashMember(1)]
    public PropertyType PropertyType { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    [HashMember(2)]
    public int Bedrooms { get; set; }
    [HashMember(3)]
    public int Bathrooms { get; set; }
    [HashMember(4, "location")]
    public string City { get; set; }
    [HashMember(5, "location")]
    public string State { get; set; }
    [HashMember(6, "location")]
    public string Zip { get; set; }
    
    public override string Hash()
    {
      // Here's the key scheme
      // {Property Stuff}-{Location}
      // {PropertyType}{Beds}{Baths}-{City}-{State}-{Zip}
      // ex: 031-portland,or-97209

      return $"{(int)PropertyType}{Bedrooms}{Bathrooms}-{City},{State}-{Zip}"
        .ToLower();
    }
  }
}