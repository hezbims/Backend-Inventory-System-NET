using Inventory_Backend_NET.Database;

namespace Inventory_Backend_NET.Seeder;

/// <summary>
/// Memasukkan data-data berikut ke database :
/// <ul>
///     <li>10 kategori baru </li>
///     <li>700+ barang dari CSV </li>
///     <li>3 user baru </li>
///     <li>10 pengaju baru </li>
///     <li>37 pengajuan random baru</li>
/// </ul>
/// </summary>
public class CompleteSeeder
{
    private readonly string[] _cliArgs;
    private readonly MyDbContext _db;

    private readonly TenKategoriSeeder _tenKategoriSeeder;
    private readonly BarangFromCsvSeeder _barangFromCsvSeeder;
    private readonly ThreeUserSeeder _threeUserSeeder;
    private readonly TenPengajuSeeder _tenPengajuSeeder;
    private readonly RandomPengajuanSeeder _randomPengajuanSeeder;

    public CompleteSeeder(
        IServiceProvider serviceProvider,
        string[] cliArgs
    )
    {
        _cliArgs = cliArgs;
        _db = serviceProvider.GetService<MyDbContext>()!;

        _tenKategoriSeeder = new TenKategoriSeeder(db: _db);
        _barangFromCsvSeeder = new BarangFromCsvSeeder(db: _db);
        _threeUserSeeder = new ThreeUserSeeder(db: _db);
        _tenPengajuSeeder = new TenPengajuSeeder(db: _db);
        _randomPengajuanSeeder = new RandomPengajuanSeeder(db: _db);
    }
    public void Run()
    {
        var rand = new Random(5);

        using var transaction = _db.Database.BeginTransaction();

        var listKategori = _tenKategoriSeeder.Run();
        _threeUserSeeder.Run();
        _tenPengajuSeeder.Run();
        _barangFromCsvSeeder.Run(listKategori: listKategori, rand: rand);

        if (!_cliArgs.Contains("no-pengajuan"))
        {
            _randomPengajuanSeeder.Run(rand: rand, totalPengajuan: 37);
        }
        
        transaction.Commit();
    }
}