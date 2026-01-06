using Inventory_Backend_NET.Startup;

namespace Inventory_Backend_NET;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Khusus kalo ngegunain minimal API, agar muncul di swagger
        // builder.Services.AddEndpointsApiExplorer();

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

        if (app.Environment.IsEnvironment("Local"))
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



        var config = builder.Configuration;
        app.Run(config["AppUrl"]);
    }
}