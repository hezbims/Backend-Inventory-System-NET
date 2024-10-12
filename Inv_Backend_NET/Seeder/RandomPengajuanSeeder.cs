using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;

namespace Inventory_Backend_NET.Seeder;

/// <summary>
/// Memasukkan pengajuan ke dalam database dengan pengaju dan barang ajuan yang bersifat Random
/// </summary>
public class RandomPengajuanSeeder
{
    private readonly MyDbContext _db;
    private readonly GetKodeTransaksiPengajuanUseCase _getKodeTransaksi;
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    public RandomPengajuanSeeder(MyDbContext db)
    {
        _db = db;
        _getKodeTransaksi = new GetKodeTransaksiPengajuanUseCase(db: _db);
    }

    public void Run(
        Random rand,
        int totalPengajuan
    )
    {
        var listUser = _db.Users.ToList();
        var listBarang = _db.Barangs.ToList();
        var listPengaju = _db.Pengajus.ToList();
        var listGrup = listPengaju.Where(pengaju => !pengaju.IsPemasok).ToList();
        
        StatusPengajuan[] listStatus = 
        {
            StatusPengajuan.Diterima,
            StatusPengajuan.Ditolak,
            StatusPengajuan.Menunggu
        };
        
        for (int i = 0; i < totalPengajuan; ++i)
        {
            var user = listUser[rand.Next(listUser.Count)];
            
            // Kalau user yang terpilih admin, pasti statusnya langsung diterima
            var status = 
                user.IsAdmin ?
                    listStatus.Single(status => status.Value == StatusPengajuan.DiterimaValue) :
                    listStatus[rand.Next(listStatus.Length)];
            
            // Kalau user yang terpilih admin,
            // pengajunya bisa siapa saja. Sedangkan kalau user biasa, pasti pengajunya adalah grup
            var pengaju = user.IsAdmin ? 
                listPengaju[rand.Next(listPengaju.Count)] :
                listGrup[rand.Next(listGrup.Count)];
            
            var barangAjuans = Enumerable.Range(0 , listBarang.Count)
                .OrderBy(_ => Guid.NewGuid()) // acak urutan indexnya
                .Take(rand.Next(4) + 1) // Ambil 1-5 barang
                .Select(index => listBarang[index]) // ubah dari index ke barang
                .Select(barang => new BarangAjuan( // ubah dari barang ke barang ajuan
                        barang: barang,
                        quantity: rand.Next(5) + 1,
                        keterangan: rand.Next(2) == 0 ? null : "abc"
                    )
                ) 
                .ToList();

            var dateCreatedAt = _timeProvider.GetLocalNow();
            _db.Pengajuans.Add(
                new Pengajuan(
                    pengaju: pengaju,
                    status: status,
                    user: user,
                    barangAjuans: barangAjuans,
                    kodeTransaksi: _getKodeTransaksi.Run(
                        dateCreatedAt: dateCreatedAt,
                        pengaju: pengaju
                    ),
                    createdAt: dateCreatedAt.ToUnixTimeMilliseconds()
                )
            );
            Console.WriteLine("Coba save pengajuan QQQ");
            _db.SaveChanges();
        }
    }
}