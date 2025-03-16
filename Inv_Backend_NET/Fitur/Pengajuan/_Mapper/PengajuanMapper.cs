using Inventory_Backend_NET.Fitur.Autentikasi._Mapper;
using Inventory_Backend_NET.Fitur.Pengaju._Mapper;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Response;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Mapper;

public static class PengajuanMapper
{
    public static GetPengajuansResponseDto.ResultData ToGetPengajuansResponseDtoData(
        this Database.Models.Pengajuan pengajuan)
    {
        return new GetPengajuansResponseDto.ResultData(
            Id: pengajuan.Id,
            TransactionCode: pengajuan.KodeTransaksi,
            UpdatedAt: pengajuan.WaktuUpdate,
            Stakeholder: pengajuan.Pengaju.ToGetStakeholdersResponseDto(),
            User: pengajuan.User.ToGetUserResponseDto(),
            Status: pengajuan.Status.Value);
    }
}