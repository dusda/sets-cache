using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DusdaCache
{
  class Program
  {
    static void Main(string[] args)
    {
      
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
