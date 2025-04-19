using Inventory_Backend_NET.Fitur._Dto;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dto.Request;

public record GetPengajuansRequestParams(
    [FromQuery(Name = "search_keyword")]
    String SearchKeyword,
    
    [FromQuery(Name = "last_id")]
    int LastId = Int32.MaxValue,
    
    [FromQuery(Name = "last_date")]
    long LastDate = Int64.MaxValue,
    
    [FromQuery(Name = "id_pengaju")]
    int? IdPengaju = null);