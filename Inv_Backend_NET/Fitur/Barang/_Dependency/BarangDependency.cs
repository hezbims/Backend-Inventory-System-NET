using Inventory_Backend_NET.Fitur.Barang._Cqrs.Query;

namespace Inventory_Backend_NET.Fitur.Barang._Dependency;

public static class BarangDependency
{
    public static IServiceCollection AddBarangDependency(this IServiceCollection services)
    {
        services.AddScoped<GetBarangsQuery>();

        return services;
    }
}