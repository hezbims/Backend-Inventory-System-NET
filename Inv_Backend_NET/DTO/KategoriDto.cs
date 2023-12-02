using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.DTO;

public class KategoriDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nama")]
    public string Nama { get; set; } = null!;

    public static KategoriDto From(Kategori kategori)
    {
        return new KategoriDto()
        {
            Id = kategori.Id,
            Nama = kategori.Nama
        };
    }
}