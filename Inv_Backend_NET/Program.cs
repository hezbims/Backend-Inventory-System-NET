using Inventory_Backend_NET.Startup;
using Inventory_Backend_NET.TestOnlyEndpoint;

namespace Inventory_Backend_NET;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder
            .PrepareDependencyInjectionServices()
            .PrepareSwagger()
            .PrepareAuthenticationServices()
            .PrepareCorsServices()
            .PrepareMonitongServices()
            .PrepareExceptionHandlingServices();



        var app = builder.Build();
        app.MigrateDatabases();

        var containsSeederKeyword = app.HandleSeedingCommandFromCli(args: args);
        if (containsSeederKeyword) return;

        if (app.Environment.IsEnvironment("Local") || app.Environment.IsEnvironment("E2E"))
        {
            app.UseSwaggerUI();
        }
        app.UseSwagger();
        app.UseWebSockets();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapControllers();

        if (app.Environment.IsEnvironment("E2E"))
            app.MapTestEndpoints();

        var config = builder.Configuration;
        app.Run(config["AppUrl"]);
    }
}