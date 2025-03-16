using System.Text.Json.Serialization;
using Inventory_Backend_NET.Fitur._Dto.Response;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;
using Inventory_Backend_NET.Fitur.Pengaju._Dto.Response;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dto.Response;

public class GetPengajuansResponseDto : MyBaseResponse<List<GetPengajuansResponseDto.ResultData>>
{
    [JsonPropertyName("meta")]
    public required MetaData Meta { get; init; }
    

    public record MetaData(
        [property: JsonPropertyName("version")] 
        int Version,
        [property: JsonPropertyName("has_next_page")]
        bool HasNextPage);
    
    public record ResultData(
        [property: JsonPropertyName("id")]
        int Id,
        [property: JsonPropertyName("transaction_code")]
        string TransactionCode,
        [property: JsonPropertyName("updated_at")]
        long UpdatedAt,
        [property: JsonPropertyName("stakeholder")]
        GetStakeholdersResponseDto Stakeholder,
        [property: JsonPropertyName("user")]
        GetUserDto User,
        [property: JsonPropertyName("status")]
        string Status);
}