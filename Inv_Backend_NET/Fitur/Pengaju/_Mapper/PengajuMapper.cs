using Inventory_Backend_NET.Fitur.Pengaju._Dto.Response;

namespace Inventory_Backend_NET.Fitur.Pengaju._Mapper;

public static class PengajuMapper
{
    public static GetStakeholdersResponseDto ToGetStakeholdersResponseDto(this Database.Models.Pengaju pengaju)
    {
        return new GetStakeholdersResponseDto(
            Id: pengaju.Id,
            Name: pengaju.Nama,
            IsSupplier: pengaju.IsPemasok);
    }
}