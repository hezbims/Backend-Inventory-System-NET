using Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dependency;

public static class PengajuanDependency
{
    public static IServiceCollection AddPengajuanDependency(this IServiceCollection services)
    {
        services.AddScoped<GetPengajuanSse>();
        services.AddScoped<GetPengajuans>();
        services.AddScoped<GetKodeTransaksiPengajuanUseCase>();

        return services;
    }
}