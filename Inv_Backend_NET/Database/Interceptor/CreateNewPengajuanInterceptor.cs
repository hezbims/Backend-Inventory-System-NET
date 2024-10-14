using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Database.Interceptor;

/// <summary>
/// Memastikan total pengajuan by tanggals terupdate ketika ada pengajuan baru 
/// </summary>
public class CreateNewPengajuanInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider; 
    public CreateNewPengajuanInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        MyDbContext? db = eventData.Context as MyDbContext;
        if (db == null)
            return base.SavingChanges(eventData, result);
        
        var insertedPengajuan = db.ChangeTracker
            .Entries<Pengajuan>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();
        if (insertedPengajuan.IsNullOrEmpty())
            return base.SavingChanges(eventData, result);
        if (insertedPengajuan.Count != 1)
            throw new Exception("Pengajuan tidak boleh bulk insert");
        
        var pengajuan = insertedPengajuan.Single();
        var tanggalPengajuan = TimeZoneInfo.ConvertTime(
                DateTimeOffset.FromUnixTimeMilliseconds(pengajuan.WaktuPengajuan),
                _timeProvider.LocalTimeZone
            ).ToString(MyConstants.DateFormat);
        
        var totalPengajuanByTanggal = db.TotalPengajuanByTanggals.FirstOrDefault(
            tanggal => tanggal.Tanggal == tanggalPengajuan
        );
        
        
        if (totalPengajuanByTanggal == null)
        {
            db.TotalPengajuanByTanggals.Add(
                new TotalPengajuanByTanggal
                {
                    Tanggal = tanggalPengajuan,
                    Total = 1
                }
            );
        }
        else
        {
            totalPengajuanByTanggal.Total++;
            db.TotalPengajuanByTanggals.Update(totalPengajuanByTanggal);
        }
        
        return base.SavingChanges(eventData, result);
    }
}