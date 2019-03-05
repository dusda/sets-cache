# Overview

![build status](https://api.travis-ci.com/dusda/sets-cache.svg?branch=master)

SetsCache is aimed at addressing the caching of highly variable objects in a consistent and proactive way that is easy to maintain. Generating cache keys and filling a range of possible hits *before* they're requested is the main idea.

## Setup

There is a docker-compose in SetsCache.Tests, be sure to run that before running the tests, as it requires a redis instance to be running.

## Cache Keys

Say you have something you need to cache that can vary a great deal, like a search.

```csharp
public class ListingSearch
  {
    public PropertyType PropertyType { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    ...
```

You can cache these as they come along, using some serialized naming scheme. This project offers a simple way to make a reliable cache key, using attribute tags.

```csharp
public class ListingSearch
  {
    [CacheMember(1)]
    public PropertyType PropertyType { get; set; }
    [CacheMember(2)]
    public int Bedrooms { get; set; }
    [CacheMember(3)]
    public int Bathrooms { get; set; }
    [CacheMember(4)]
    public string City { get; set; }
    [CacheMember(5)]
    public string State { get; set; }
    public string Zip { get; set; }
```

With those attributes decorating your class, an object like this:

```csharp
var search = new ListingSearch()
{
  PropertyType = PropertyType.Apartments,
  Bedrooms = 3,
  Bathrooms = 1,
  City = "Portland",
  State = "OR",
  Zip = "97202"
};
```

You'll get a cache key like this: `231-Portland-OR`.

You can decorate at the class-level, too. It'll turn a class like 'ListingSearchSeo' into a subkey like `231-Portland-OR:ListingSearchSeo`.

## Proactive Caching

A purely reactive approach has it's problems, though. Ideally, you'd like to be able to guess at what other stuff should be cached before someone asks for it. This is done with subsets:

```csharp
//a simple set of three numbers
var set = new int[]{1, 2, 3};

//these are all of the possible subsets for it
var subsets = new int[8][]
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
```

Each `CacheMember` is an item in the set, and therefore each subset can be used as a cache key. So, a key like `231-Portland-OR` (three bed two bath apartments in Portland, OR) becomes:

```text
231-Portland-OR
#31-Portland-OR
2#1-Portland-OR
##1-Portland-OR
23#-Portland-OR
#3#-Portland-OR
2##-Portland-OR
###-Portland-OR
...and so on
```

SetsCache uses [Gray Code](https://en.wikipedia.org/wiki/Gray_code) and bit-shift operations to find subsets.

## Filling Cache Keys

Once you know the keys, it's just a matter of filling them. There are x^2 - 1 subsets for a given set, so it's best to do this as a background job. Currently SetsCache demonstrates this using Redis Lists (specifically `RPUSH`, `LPOP` and `LLEN`) in a last-in first-out scheme. See the SeoFiller class.

## Practical Use

Here is a typical workflow when you're dealing with a search.

```csharp
var serializer = services.Get<ICacheMemberSerializer>();
var cache = services.Get<ISetsCache>();
var redisClient = services.Get<IDatabase>();

//get some search state
var search = new ListingSearch
{
  PropertyType = PropertyType.Apartments,
  Bedrooms = 2,
  Bathrooms = 1
};

//find subsets for the search, and try to grab the first one from the cache.
var cacheKeys = serializer.GetSubsets(search);
var data = await cache.Get<ListingSearch>(cacheKeys[0]);

if(data != null)
  return data;

//if it's not there, push the keys to redis to work on later.
await redisClient.ListRightPushAsync("seo", cacheKeys);
```

Then, in some background service:

```csharp
var keyTask = cache.ListLeftPopAsync("seo");
var countTask = cache.ListLengthAsync("seo");
Task.WaitAll(keyTask, countTask);

//number of items left in the list
var count = countTask.Result;

if (string.IsNullOrWhiteSpace(keyTask.Result))
  return count;

//parse the cache key
string key = keyTask.Result;
var search = serializer.Parse<Tests.ListingSearch>(key);

//go search or whatever to get your cache data
var items = new Tests.ListingSearchSeo
{
  City = "Portland",
  State = "OR",
  Views = 354235
};

//cache it
await setsCache.SetSub(search, items);

//return the number of remaining items in the list
return count;
```

Since you're returning the count with each operation, you should know when to keep working on the list. Coupling that with a regular check on the Redis list should be sufficient.

## Remarks

Right now this is just a proof of concept, there are some messy bits. Some of the logic can certainly be extracted from the Redis implementation and made generic. Helper methods to add this stuff to the service collection would be nice. A fluent approach to configuring is something I'm going to work on, too. Also the actual method used to derive cache keys could be made configurable, there's no reason it all has to be about subsets.