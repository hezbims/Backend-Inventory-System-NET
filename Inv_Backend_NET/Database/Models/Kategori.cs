using Inventory_Backend_NET.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Database.Models;

[EntityTypeConfiguration(typeof(KategoriConfiguration))]
public class Kategori
{
    public int Id { get; set; }
    public string Nama { get; set; } = null!;

    public ICollection<Barang> Barangs{ get; } = new List<Barang>();
    
    private Kategori(){}

    public Kategori(
        string nama,
        int? id = null
    )
    {
        Nama = nama;
        Id = id ?? default;
    }
}