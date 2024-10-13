using Microsoft.Net.Http.Headers;

namespace Inventory_Backend_NET.Startup;

public static class PrepareSpa
{
    public static WebApplication PrepareSpaServices(this WebApplication app)
    {
        app.UseSpaStaticFiles(new StaticFileOptions
        {
            // biar bisa ngeserve file ENV dari SPA
            ServeUnknownFileTypes = true,
            OnPrepareResponse = ctx =>
            {
                // Set cache control headers
                ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                ctx.Context.Response.Headers.Append("Expires", "-1");
            }
        });
        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "web";
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true
                    };
                }
            };
        });
        return app;
    }

    public static WebApplicationBuilder PrepareSpaServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSpaStaticFiles(options =>
        {
            options.RootPath = "web";
        });
        return builder;
    }
}