using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class MySqliteCacheExtension
{
    public static void IncrementCache(
        this IDistributedCache cache,
        string key
    )
    {
        var increment = GetIntValue(cache , key) + 1;
        cache.SetString(key , increment.ToString());
    }

    public static int GetIntValue(
        this IDistributedCache cache,
        string key
    )
    {
        try
        {
            return int.Parse(cache.GetString(key)!);
        }
        catch (Exception)
        {
            // return 0
            return default;
        }
    }
}