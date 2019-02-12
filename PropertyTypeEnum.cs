using System.ComponentModel;

namespace ListingHash
{
  public enum PropertyType
  {
    [Description("Any")]
    Any,
    [Description("House")]
    House,
    [Description("Apartment")]
    Apartment,
    [Description("Condo")]
    Condo,
    [Description("Multi-Family Home")]
    MultiFamilyHome
  }
}