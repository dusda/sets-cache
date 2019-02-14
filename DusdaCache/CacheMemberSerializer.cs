using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DusdaCache
{
  public static class CacheMemberSerializer
  {
    public static string GetHash<T>(T item)
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

    public static string[] GetHashSet<T>(T obj)
    {
      var type = typeof(CacheMemberAttribute);
      var props = typeof(T).GetProperties()
        .Where(p => Attribute.IsDefined(p, type))
        .OrderBy(p => ((CacheMemberAttribute)p.GetCustomAttributes(type, false).Single()).Position)
        .ToList();

      var items = new int[props.Count];
      for (int i = 0; i < items.Length; i++)
        items[i] = i + 1;

      var subsets = SubsetSolver.Solve(items, fill: true);
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

    static readonly string[] defaults = new string[] { "0", "-", string.Empty };
    static string GetValue<T>(T value, PropertyInfo prop)
    {
      var attr = prop.GetCustomAttributes(typeof(CacheMemberAttribute), false)
        .FirstOrDefault() as CacheMemberAttribute;
      var val = prop.GetValue(value);

      //attempt to parse as a hexidecimal int (0-f),
      //fallback to a string.
      string fVal = string.Empty;
      if (prop.PropertyType == typeof(int) || prop.PropertyType.IsEnum)
      {
        if ((int)val > 0xf)
          throw new ArgumentException($"integer Property {prop.Name} must be no greater than 15.");
        fVal = ((int)val).ToString("X");
      }
      else
        fVal = $"-{val}";

      return !defaults.Contains(fVal) ? fVal.ToLower() : "#";
    }
  }
}