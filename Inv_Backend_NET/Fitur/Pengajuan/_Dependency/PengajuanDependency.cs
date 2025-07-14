using Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Inventory_Backend_NET.Fitur.Pengajuan.Application.Handler;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.Repository;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dependency;

public static class PengajuanDependency
{
    public static IServiceCollection AddPengajuanDependency(this IServiceCollection services)
    {
        services.AddScoped<GetPengajuanSse>();
        services.AddScoped<GetPengajuans>();
        services.AddScoped<GetKodeTransaksiPengajuanUseCase>();

        services.AddTransient<TransactionRepositoryImpl>();
        services.AddTransient<CreateTransactionHandler>();

        return services;
    }
}