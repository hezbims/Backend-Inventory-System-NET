using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Database.Interceptor;

public class PengajuanChangedInterceptor : SaveChangesInterceptor
{
    private readonly IMemoryCache _memoryCache;
    public PengajuanChangedInterceptor(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        MyDbContext? db = eventData.Context as MyDbContext;
        if (db == null)
            return base.SavedChanges(eventData, result);

        var hasPengajuan = !db.ChangeTracker
            .Entries<Pengajuan>()
            .Where(entry =>
                entry.State switch
                {
                    EntityState.Added or EntityState.Deleted or EntityState.Modified => true,
                    _ => false
                })
            .IsNullOrEmpty();

        if (hasPengajuan)
        {
            var previousTableVersion = _memoryCache.Get<int>(
                MyConstants.CacheKeys.PengajuanTableVersion);
            
            _memoryCache.Set(
                MyConstants.CacheKeys.PengajuanTableVersion,
                previousTableVersion + 1);
        }
        
        return base.SavedChanges(eventData, result);
    }
}