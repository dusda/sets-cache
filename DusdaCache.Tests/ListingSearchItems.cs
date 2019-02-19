using ProtoBuf;

namespace DusdaCache.Tests
{
  [ProtoContract, CacheMember]
  public class ListingSearchItems
  {
    [ProtoMember(1)]
    public int Views { get; set; }

    [ProtoMember(2)]
    public string[] Nearby { get; set; }
  }
}