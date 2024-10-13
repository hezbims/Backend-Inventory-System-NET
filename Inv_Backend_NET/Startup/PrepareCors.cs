namespace Inventory_Backend_NET.Startup;

public static class PrepareCors
{
    public static WebApplicationBuilder PrepareCorsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => 
            options.AddDefaultPolicy(corsBuilder =>
            {
                // TODO : nanti ubah kalo udah deployment
                corsBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            })
        );
        return builder;
    }
}