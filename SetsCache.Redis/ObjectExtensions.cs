
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace SetsCache.Extensions
{
  public static class ObjectExtensions
  {
    public static byte[] ToBinaryArray<T>(this T obj)
    {
      if (obj == null)
        return null;

      using (MemoryStream ms = new MemoryStream())
      {
        Serializer.Serialize<T>(ms, obj);
        return ms.ToArray();
      }
    }

    public static T Deserialize<T>(this byte[] bytes)
    {
      if (bytes == null)
        return default(T);

      using (MemoryStream ms = new MemoryStream(bytes))
      {
        return Serializer.Deserialize<T>(ms);
      }
    }

    public static T DeepCopy<T>(this T obj)
    {
      if (!typeof(T).IsSerializable)
      {
        throw new ArgumentException("The type must be serializable.", "source");
      }

      // Don't serialize a null object, simply return the default for that object
      if (Object.ReferenceEquals(obj, null))
      {
        return default(T);
      }

      using (var stream = new MemoryStream())
      {
        IFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, obj);

        stream.Seek(0, SeekOrigin.Begin);

        return (T)formatter.Deserialize(stream);
      }
    }
  }
}