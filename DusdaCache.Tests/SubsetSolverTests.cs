using System.Linq;
using Xunit;

namespace DusdaCache.Tests
{
  public class SubsetSolverTests
  {
    [Fact]
    public void Solves3Correctly()
    {
      var set = new int[] { 1, 2, 3 };
      var res = SetSolver.Solve(set)
        .Select(f => f.ToArray())
        .ToArray();

      var solved = new int[8][]
      {
        new int[] {1,2,3},
        new int[] {2,3},
        new int[] {1,3},
        new int[] {3},
        new int[] {1,2},
        new int[] {2},
        new int[] {1},
        new int[] {}
      };

      Assert.Equal(solved, res);
    }

    [Fact]
    public void FillsCorrectly()
    {
      var set = new int[] { 1, 2, 3 };

      var res = SetSolver.Solve(set, fill: true)
        .Select(f => f.ToArray())
        .ToArray();

      var solved = new int[8][]
      {
          new int[] {1, 2, 3},
          new int[] {0, 2, 3},
          new int[] {1, 0, 3},
          new int[] {0, 0, 3},
          new int[] {1, 2, 0},
          new int[] {0, 2, 0},
          new int[] {1, 0, 0},
          new int[] {0, 0, 0}
      };

      Assert.Equal(solved, res);
    }
  }
}