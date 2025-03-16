using System.Text.Json.Serialization;
using Inventory_Backend.Tests.Fitur.Pengaju._Dto;
using Inventory_Backend.Tests.Fitur.User._Dto;

namespace Inventory_Backend.Tests.Fitur.Pengajuan.GetPengajuanTest._Dto;

public record GetPengajuansResponseTestDto(
    [property: JsonPropertyName("data")]
    List<GetPengajuansResponseTestDto.ResultData> Data,
    [property: JsonPropertyName("meta")]
    GetPengajuansResponseTestDto.MetaData Meta
    )
{
    public record ResultData(
        [property: JsonPropertyName("user")]
        GetUserResponseTestDto User,
        [property: JsonPropertyName("stakeholder")]
        GetStakeholdersResponseTestDto Stakeholder,
        [property: JsonPropertyName("transaction_code")]
        string TransactionCode, 
        [property: JsonPropertyName("status")]
        string status);

    public record MetaData(
        [property: JsonPropertyName("version")]
        int Version);
}