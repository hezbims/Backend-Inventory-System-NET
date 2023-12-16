using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend_NET.UseCases.PostPengajuan;

public class UpsertCurrentPengajuanUseCase
{
    private readonly MyDbContext _db;
    private readonly IDistributedCache _cache;

    public UpsertCurrentPengajuanUseCase(MyDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }
    
    public Pengajuan Exec(
        Pengajuan? previousPengajuan,
        SubmitPengajuanBody requestBody,
        User submitter
    )
    {
        var currentPengaju = _db.Pengajus.Single(pengaju => pengaju.Id == requestBody.IdPengaju);

        
        var currentBarangAjuans = requestBody.BarangAjuans.Select(
            barangAjuanBody => barangAjuanBody.ToBarangAjuan()
        ).ToList();

        if (currentBarangAjuans.Count == 0)
        {
            throw new BadHttpRequestException("Tolong ajukan minimal satu barang!");
        }

        
        User pemilikPengajuan = previousPengajuan?.User ?? submitter;

        
        StatusPengajuan statusPengajuan;

        int totalQuantityOfCurrentPengajuan = requestBody.BarangAjuans.Sum(barangAjuan => barangAjuan.Quantity);
        if (!submitter.IsAdmin && totalQuantityOfCurrentPengajuan == 0)
            throw new BadHttpRequestException("Tidak boleh mengajukan dengan total quantity 0");
        if (previousPengajuan != null && totalQuantityOfCurrentPengajuan == 0)
            statusPengajuan = StatusPengajuan.Ditolak;
        else
            statusPengajuan = StatusPengajuan.GetByEditor(submitter);

        Pengajuan currentPengajuan = new Pengajuan(
            cache: _cache,
            pengaju: currentPengaju,
            status: statusPengajuan,
            user: pemilikPengajuan,
            barangAjuans: currentBarangAjuans,
            id: previousPengajuan?.Id
        );
            
        if (previousPengajuan == null)
        {
            _db.Pengajuans.Add(currentPengajuan);
        }
        else
        {
            _db.BarangAjuans.RemoveRange(previousPengajuan.BarangAjuans);
            _db.Entry(previousPengajuan).State = EntityState.Detached;
            _db.Pengajuans.Update(currentPengajuan);
        }

        return currentPengajuan;
    }
}