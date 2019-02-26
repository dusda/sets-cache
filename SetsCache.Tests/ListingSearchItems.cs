using ProtoBuf;

namespace SetsCache.Tests
{
  [ProtoContract, CacheMember]
  public class ListingSearchSeo
  {
    [ProtoMember(1)]
    public int Views { get; set; }

    [ProtoMember(2)]
    public string City { get; set; }

    [ProtoMember(3)]
    public string State { get; set; }
  }
}