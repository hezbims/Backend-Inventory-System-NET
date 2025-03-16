using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.Seeder;

namespace Inventory_Backend.Tests.Fitur._Preparation;

/// <summary>
/// Melakukan seeding pada tabel
/// <c>Pengajus</c>,
/// <c>Barangs</c>,
/// <c>Kategoris</c>,
/// <c>Users</c>,
/// <c>Pengajuans</c>
/// </summary>
public class BasicDataset(
    KategoriSeeder kategoriSeeder,
    UserSeeder userSeeder,
    PengajuSeeder pengajuSeeder,
    BarangSeeder barangSeeder,
    PengajuanSeeder pengajuanSeeder) : IBaseDataset
{
    public TestData Run(
        bool seedBarang = true,
        bool seedPengajuan = true)
    {
        Inventory_Backend_NET.Database.Models.User admin = userSeeder.Run(
            isAdmin: true,
            username: "admin");
        Inventory_Backend_NET.Database.Models.User nonAdmin = userSeeder.Run(
            isAdmin: false,
            username: "non_admin");
        Inventory_Backend_NET.Database.Models.Pengaju pemasok = pengajuSeeder.Run(
            isPemasok: false, name: "pemasok");
        Inventory_Backend_NET.Database.Models.Pengaju grup = pengajuSeeder.Run(
            isPemasok: true, name: "grup");
        List<Inventory_Backend_NET.Database.Models.Pengaju> pengajus = [pemasok, grup];
        
        List<Kategori> kategoris = kategoriSeeder.Run(3);
        List<Barang> barangs = [];

        if (!seedBarang)
            return new TestData(admin, nonAdmin);
        for (int kategoriIndex = 1; kategoriIndex <= 2; kategoriIndex++)
            for (int nomorKolom = 1 ; nomorKolom <= 3 ; nomorKolom++)
                barangs.Add(barangSeeder.Run(
                    name: $"barang-{kategoris[kategoriIndex].Nama}-{nomorKolom}",
                    kodeBarang: $"R1-{kategoriIndex}-{nomorKolom}",
                    kategori: kategoris[kategoriIndex],
                    nomorRak: 1,
                    nomorLaci: kategoriIndex,
                    nomorKolom: nomorKolom));

        if (!seedPengajuan)
            return new TestData(
                admin, nonAdmin);
        
        List<StatusPengajuan> statusPengajuans = [
            StatusPengajuan.Ditolak, 
            StatusPengajuan.Diterima, 
            StatusPengajuan.Menunggu];
        
        // Pengajuan milik non admin
        foreach (var statusPengajuan in statusPengajuans)
        {
            pengajuanSeeder.Run(
                pengaju: grup,
                user: nonAdmin,
                barangAjuans:
                [
                    new BarangAjuan(
                        barang: barangs[0],
                        quantity: 1,
                        keterangan: null),
                    new BarangAjuan(
                        barang: barangs[1],
                        quantity: 2,
                        keterangan: "barang-ajuan-non-admin")
                ],
                status: statusPengajuan);
        }

        // pengajuan dari admin
        foreach (var statusPengajuan in statusPengajuans)
            foreach (var pengaju in pengajus)
                pengajuanSeeder.Run(
                    pengaju: pengaju,
                    user: admin,
                    barangAjuans: [
                        new BarangAjuan(
                            barang: barangs[0],
                            quantity: 1,
                            keterangan: null),
                        new BarangAjuan(
                            barang: barangs[1],
                            quantity: 2,
                            keterangan: "barang-ajuan-admin")
                    ],
                    status: statusPengajuan);
        
        return new TestData(admin, nonAdmin);
    }

    public record TestData(
        Inventory_Backend_NET.Database.Models.User Admin,
        Inventory_Backend_NET.Database.Models.User NonAdmin
    );
}