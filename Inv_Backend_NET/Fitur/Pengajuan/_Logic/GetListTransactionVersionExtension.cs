using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Logic;

public static class GetListTransactionVersionExtension
{
    public static int GetListTransactionVersion(this IMemoryCache memoryCache, User user)
    {
        return memoryCache.Get<int>(user.IsAdmin ? 
            MyConstants.CacheKeys.PengajuanTableVersionForAdmin :
            MyConstants.CacheKeys.PengajuanTableVersionByUser(user));
    }
}