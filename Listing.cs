using System.Security.Cryptography;
using System.Text;

namespace ListingHash
{
  public class Listing
  {
    public Listing() { }
    public long Id { get; set; }
    public string Title { get; set; }

    public Address Address { get; set; }

    public PropertyType PropertyType { get; set; }
  }
}