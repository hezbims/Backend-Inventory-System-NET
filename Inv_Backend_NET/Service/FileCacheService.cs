using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend_NET.Service;

public class FileCacheService : IFileCacheService
{
    public string? Get(string key)
    {
        var fileCachePath = GetCacheFile();
        var cacheContent = ParseCacheFile(fileCachePath);
        
        throw new NotImplementedException();
    }

    public void Set(string key, string value)
    {
        throw new NotImplementedException();
    }

    private string GetCacheFile()
    {
        string basePath = Environment.CurrentDirectory;
        string cacheFile = Path.Combine(basePath, "Cache/Cache.txt");
        if (!File.Exists(cacheFile))
        {
            using (var f = File.Create(cacheFile));
        }

        return cacheFile;
    }

    private List<Tuple<string, string>> ParseCacheFile(string filePath)
    {
        var content = File.ReadAllLines(filePath);
        var results = new List<Tuple<string, string>>();
        foreach (var line in content)
        {
            var splitResult = line.Split('\t');
            results.Add(new Tuple<string , string>(
                splitResult[0] , splitResult[1])
            );
        }

        return results;
    }
}