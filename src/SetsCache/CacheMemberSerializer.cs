using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SetsCache
{
  public interface ICacheMemberSerializer
  {
    string Get<T>();
    string Get<T>(T item);
    string Get<T, TSub>(T item, TSub sub);
    string[] GetSubsets<T>(T obj);
    string[] GetSubsets<T, TSub>(T obj, TSub sub);
    T Parse<T>(string key) where T : class, new();
    (T item, TSub sub) Parse<T, TSub>(string key)
      where T : class, new()
      where TSub : class, new();
  }

  public class CacheMemberSerializer : ICacheMemberSerializer
  {
    readonly char[] delimiters = new char[] { ':', '-', '#' };
    public string Get<T>()
    {
      var type = typeof(CacheMemberAttribute);
      var classMember = typeof(T).GetCustomAttribute(type);
      if (classMember == null)
        throw new ArgumentException("Cache keys can only be derived without an instance when defined at class-level");

      return typeof(T).Name;
    }

    public string Get<T>(T item)
    {
      var type = typeof(CacheMemberAttribute);

      //if defined at class-level, just return the name of the object.
      var classMember = typeof(T).GetCustomAttribute(type);
      if (classMember != null)
        return typeof(T).Name;

      //otherwise build it from the properties
      var props = typeof(T).GetProperties()
        .Where(p => Attribute.IsDefined(p, type))
        .OrderBy(p => ((CacheMemberAttribute)p.GetCustomAttributes(type, false).Single()).Position)
        .Select(p => GetValue(item, p))
        .ToList();

      var key = CleanKey(string.Join(string.Empty, props));

      return key;
    }

    public string Get<T, TSub>(T item, TSub sub)
    {
      var keys = new string[] { Get(item), Get(sub) };
      var key = string.Join(':', keys);

      return key;
    }

    public string[] GetSubsets<T>(T obj)
    {
      var type = typeof(CacheMemberAttribute);
      var props = typeof(T).GetProperties()
        .Where(p => Attribute.IsDefined(p, type))
        .OrderBy(p => ((CacheMemberAttribute)p.GetCustomAttributes(type, false).Single()).Position)
        .ToList();

      var items = new int[props.Count];
      for (int i = 0; i < items.Length; i++)
        items[i] = i + 1;

      var subsets = SetSolver.Solve(items, fill: true);
      var keys = new List<string>();
      var sb = new StringBuilder();

      int[] subset;
      foreach (var item in subsets)
      {
        subset = item.ToArray();
        for (int i = 0; i < item.Count(); i++)
          if (subset[i] == 0)
            sb.Append("#");
          else
            sb.Append(GetValue(obj, props[subset[i] - 1]));

        keys.Add(CleanKey(sb.ToString()));
        sb.Clear();
      }

      return keys.Distinct().ToArray();
    }

    public string[] GetSubsets<T, TSub>(T obj, TSub sub)
    {
      var subKey = Get(sub);
      var keys = GetSubsets(obj)
        .Select(f => f + $":{subKey}")
        .ToArray();

      return keys;
    }

    public T Parse<T>(string key) where T : class, new()
    {
      var type = typeof(CacheMemberAttribute);
      var props = typeof(T).GetProperties()
        .Where(p => Attribute.IsDefined(p, type))
        .OrderBy(p => ((CacheMemberAttribute)p.GetCustomAttributes(type, false).Single()).Position)
        .ToList();

      //need to parse #'s for ints, dash-delimited strings for strings
      //###-portland##
      int index = 0;
      var item = new T();
      foreach (var prop in props)
      {
        if (prop.PropertyType == typeof(int) || prop.PropertyType.IsEnum)
        {
          var str = key.Substring(index, 1);
          ushort val = 0;

          if (str != "#")
            val = ushort.Parse(str, NumberStyles.HexNumber);

          prop.SetValue(item, val);

          index++;
        }
        else
        {
          if (key.Substring(index, 1) == "-")
          {
            var chars = key.Substring(index + 1)
              .TakeWhile(f => !delimiters.Contains(f))
              .ToArray();

            prop.SetValue(item, new string(chars));
            index += chars.Length + 1;
          }
        }
      }

      return item;
    }

    public (T item, TSub sub) Parse<T, TSub>(string key)
      where T : class, new()
      where TSub : class, new()
    {
      var keys = key.Split(':');
      if (keys.Length != 2)
        throw new ArgumentException("Key must be in item:sub format.");

      (T, TSub) item = (Parse<T>(keys[0]), Parse<TSub>(keys[1]));

      return item;
    }

    readonly string[] defaults = new string[] { "0", "--", string.Empty };
    string GetValue<T>(T value, PropertyInfo prop)
    {
      var attr = prop.GetCustomAttributes(typeof(CacheMemberAttribute), false)
        .FirstOrDefault() as CacheMemberAttribute;
      var val = prop.GetValue(value);

      //attempt to parse as a hexidecimal int (0-f),
      //fallback to a string.
      string fVal = string.Empty;
      if ((prop.PropertyType == typeof(int) || prop.PropertyType.IsEnum))
      {
        if ((int)val > 0xf)
          throw new ArgumentException("Integer value must be less than or equal to 15");
        fVal = ((int)val).ToString("X");
      }
      else
        fVal = $"-{val}-";

      return !defaults.Contains(fVal) ? fVal : "#";
    }

    string CleanKey(string key) => key.Replace("--", "-").Trim('-');
  }
}