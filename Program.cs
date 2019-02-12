using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ListingHash
{
  class Program
  {
    static void Main(string[] args)
    {
      var search = new ListingSearch
      {
        PropertyType = PropertyType.Any,
        Bedrooms = 10,
        Bathrooms = 1,
        City = "Portland",
        State = "OR",
        Zip = "97209"
      };

      var hash = HashMemberParser.GetHash(search);
      var set = HashMemberParser.GetHashSet(search);

      var items = new int[] { 1, 2, 3 };
      var res = SubsetSolver.Solve(items);

      var json = JsonConvert.SerializeObject(res, Formatting.Indented);
      Console.WriteLine(json);

    }

    static List<Listing> GetListings()
    {
      var items = new List<Listing>
      {
          new Listing
          {
              Id = 1,
              Title = "Awesome place",
              Address = new Address
              {
                Address1 = "125 NW 20th Pl",
                City = "Portland",
                State = "OR",
                Zip = "97209",
                Country = "USA"
            },
            PropertyType = PropertyType.House
          },

          new Listing
          {
              Id = 2,
              Title = "Better Place",
              Address = new Address
              {
                  Address1 = "7114 SE 17th Ave",
                  City = "Portland",
                  State = "OR",
                  Zip = "97209",
                  Country = "USA"
              },
              PropertyType = PropertyType.House
          }
      };

      return items;
    }
  }
}
