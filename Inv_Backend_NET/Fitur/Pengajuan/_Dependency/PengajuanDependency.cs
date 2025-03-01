using Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dependency;

public static class PengajuanDependency
{
    public static IServiceCollection AddPengajuanDependency(this IServiceCollection services)
    {
        services.AddScoped<GetPengajuanSse>();

        return services;
    }
}