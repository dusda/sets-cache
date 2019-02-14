using System.ComponentModel;

namespace DusdaCache
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