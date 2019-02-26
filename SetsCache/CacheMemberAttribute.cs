using System;

namespace SetsCache
{
  /// <summary>
  /// Use to mark a property for serialization to a cachekey/value.
  /// https://stackoverflow.com/questions/9062235/get-properties-in-order-of-declaration-using-reflection
  /// </summary>
  [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
  public sealed class CacheMemberAttribute : System.Attribute
  {
    readonly int _position;

    public CacheMemberAttribute(
      int position = 0)
    {
      _position = position;
    }

    public int Position { get => _position; }
  }
}