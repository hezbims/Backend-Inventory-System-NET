namespace Inventory_Backend_NET.Models;

public class Kategori
{
    public int Id { get; set; }
    public string Nama { get; set; } = null!;

    public ICollection<Barang> Barangs{ get; } = new List<Barang>();
}