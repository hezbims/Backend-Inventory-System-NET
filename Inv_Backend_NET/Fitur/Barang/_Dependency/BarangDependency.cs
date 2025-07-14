using Inventory_Backend_NET.Fitur.Barang._Cqrs.Query;
using Inventory_Backend_NET.Fitur.Barang.Handler;

namespace Inventory_Backend_NET.Fitur.Barang._Dependency;

public static class BarangDependency
{
    public static IServiceCollection AddBarangDependency(this IServiceCollection services)
    {
        services.AddScoped<GetBarangsQuery>();
        services.AddTransient<ProductQuantityChangedEventHandler>();

        return services;
    }
}