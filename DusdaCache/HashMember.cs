namespace DusdaCache
{
  /// <summary>
  /// Use to mark a property for serialization to a cachekey/value.
  /// https://stackoverflow.com/questions/9062235/get-properties-in-order-of-declaration-using-reflection
  /// </summary>
  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public sealed class HashMemberAttribute : System.Attribute
  {
    readonly int _position;
    readonly string _group;

    public HashMemberAttribute(
      int position,
      string group = null)
    {
      _position = position;
      _group = group ?? "default";
    }

    public int Position { get => _position; }    
    public string Group { get => _group; }
  }
}