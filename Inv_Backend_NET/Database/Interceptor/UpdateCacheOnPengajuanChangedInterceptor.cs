using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Database.Interceptor;

public class UpdateCacheOnPengajuanChangedInterceptor : SaveChangesInterceptor
{
    private readonly IMemoryCache _memoryCache;
    public UpdateCacheOnPengajuanChangedInterceptor(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        MyDbContext? db = eventData.Context as MyDbContext;
        if (db == null)
            return base.SavingChanges(eventData, result);

        var entries = db.ChangeTracker
            .Entries<Pengajuan>()
            .Where(entry =>
                entry.State == EntityState.Added ||
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            .ToList();
        
        var hasPengajuan = !entries.IsNullOrEmpty();
        if (hasPengajuan)
        {
            var user = entries.Single().Entity.User;
            var previousTableVersionByUser = _memoryCache.Get<int>(
                MyConstants.CacheKeys.PengajuanTableVersionByUser(user));
            _memoryCache.Set(
                MyConstants.CacheKeys.PengajuanTableVersionByUser(user),
                previousTableVersionByUser + 1);
            
            var previousTableVersionByAdmin = _memoryCache.Get<int>(
                MyConstants.CacheKeys.PengajuanTableVersionForAdmin);
            _memoryCache.Set(
                MyConstants.CacheKeys.PengajuanTableVersionForAdmin,
                previousTableVersionByAdmin + 1);
        }
        
        return base.SavingChanges(eventData, result);
    }
}