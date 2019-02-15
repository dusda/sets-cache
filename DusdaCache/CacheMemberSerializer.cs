using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DusdaCache
{
  public static class CacheMemberSerializer
  {
    public static string Get<T>(T item)
    {
      var type = typeof(CacheMemberAttribute);
      var props = typeof(T).GetProperties()
        .Where(p => Attribute.IsDefined(p, type))
        .OrderBy(p => ((CacheMemberAttribute)p.GetCustomAttributes(type, false).Single()).Position)
        .Select(p => GetValue(item, p))
        .ToList();

      var key = string.Join(string.Empty, props);

      return key;
    }

    public static string[] GetSet<T>(T obj)
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

        keys.Add(sb.ToString());
        sb.Clear();
      }

      return keys.Distinct().ToArray();
    }

    public static T Parse<T>(string key) where T : class, new()
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

          index++;
        }
        else
        {
          string val = "";

          index+= val.Length;
        }
      }

      return item;
    }

    static readonly string[] defaults = new string[] { "0", "-", string.Empty };
    static string GetValue<T>(T value, PropertyInfo prop)
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
        fVal = $"-{val}";

      return !defaults.Contains(fVal) ? fVal.ToLower() : "#";
    }
  }
}