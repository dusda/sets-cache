dotnet pack -c Release;
dotnet nuget push SetsCache/bin/Release/sets-cache.*.nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json;
dotnet nuget push SetsCache.Redis/bin/Release/sets-cache-redis.*.nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json;