dotnet pack -c Release;
dotnet nuget push SetsCache/bin/Release/SetsCache.*.nupkg --api-key $NUGET_API_KEY -Source https://api.nuget.org/v3/index.json;
dotnet nuget push SetsCache.Redis/bin/Release/SetsCache.Redis.*.nupkg --api-key $NUGET_API_KEY -Source https://api.nuget.org/v3/index.json;