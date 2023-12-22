using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.UseCases.PostPengajuan;

public class UpsertCurrentPengajuanUseCase
{
    private readonly MyDbContext _db;
    private readonly TimeProvider _timeProvider;

    public UpsertCurrentPengajuanUseCase(
        MyDbContext db, 
        TimeProvider timeProvider
    )
    {
        _db = db;
        _timeProvider = timeProvider;
    }
    
    public Pengajuan By(
        Pengajuan? previousPengajuan,
        SubmitPengajuanBody requestBody,
        User submitter
    )
    {
        var currentPengaju = _db.Pengajus.Single(pengaju => pengaju.Id == requestBody.IdPengaju);

        
        var currentBarangAjuans = requestBody.BarangAjuans!.Select(
            barangAjuanBody => barangAjuanBody.ToBarangAjuan()
        ).ToList();

        if (currentBarangAjuans.Count == 0)
        {
            throw new BadHttpRequestException("Tolong ajukan minimal satu barang!");
        }

        
        User pemilikPengajuan = previousPengajuan?.User ?? submitter;

        
        StatusPengajuan statusPengajuan;

        int totalQuantityOfCurrentPengajuan = requestBody.BarangAjuans!.Sum(barangAjuan => barangAjuan.Quantity);
        if (!submitter.IsAdmin && totalQuantityOfCurrentPengajuan == 0)
            throw new BadHttpRequestException("Tidak boleh mengajukan dengan total quantity 0");
        if (previousPengajuan != null && totalQuantityOfCurrentPengajuan == 0)
            statusPengajuan = StatusPengajuan.Ditolak;
        else
            statusPengajuan = StatusPengajuan.GetByEditor(submitter);

        Pengajuan currentPengajuan = new Pengajuan(
            db: _db,
            pengaju: currentPengaju,
            status: statusPengajuan,
            user: pemilikPengajuan,
            barangAjuans: currentBarangAjuans,
            id: previousPengajuan?.Id,
            timeProvider: _timeProvider
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