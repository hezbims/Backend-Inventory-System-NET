using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Barang._Data;

public record GetBarangsRequestParams(
    [FromQuery(Name = "keyword")]
    string SearchKeyword,
    
    [FromQuery(Name = "page")]
    int? Page,
    
    [FromQuery(Name = "id_kategori")]
    int? IdKategori
);