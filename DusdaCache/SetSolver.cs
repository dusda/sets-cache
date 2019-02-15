using System;
using System.Collections.Generic;
using System.Linq;

namespace DusdaCache
{
  public class SetSolver
  {
    /// <summary>
    /// Determines all subsets for a given set of integers.
    /// </summary>
    /// <param name="values">The set to solve.</param>
    /// <param name="fill">Whether to fill empty parts of the set with 0.</param>
    /// <example>
    /// Given a set like {1, 2, 3}, it will return:
    /// {1,2,3}, {2,3}, {1,3}, {3}, {1,2}, {2}, {1}, {}.
    /// 
    /// If fill is enabled, it will place a 0 instead of skipping the combination.
    /// 
    /// </example>
    /// <see>
    /// This implementation is based on Gray code, see
    /// https://en.m.wikipedia.org/wiki/Gray_code.
    /// </see>
    /// <seealso>
    /// This C implementation: https://math.stackexchange.com/a/1305232/642946.
    /// </seealso>    
    public static IEnumerable<IEnumerable<int>> Solve(
      IEnumerable<int> values, bool fill = false)
    {
      uint i, j;
      ushort bit;
      uint max_bits;
      int size = (int)Math.Pow(2, values.Count());
      var subsets = new Stack<IEnumerable<int>>(size);

      IList<int> list;
      for (i = 0; i < size; i++)
      {
        list = new List<int>(values.Count());
        max_bits = (uint)Math.Floor(Math.Log(i, 2));

        for (j = 0; j <= max_bits; j++)
        {
          bit = (ushort)((i >> (int)j) & 1);
          if (bit == 1)
            list.Add(values.ElementAt((int)j));
          else if(fill)
            list.Add(0);
        }

        if(fill)
        {
          while(list.Count < values.Count())
            list.Add(0);
        }
        subsets.Push(list);
      }

      return subsets;
    }
  }
}