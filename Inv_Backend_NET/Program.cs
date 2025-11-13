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
            .PrepareSwaggerWithJwtInputService()
            .PrepareAuthenticationServices()
            .PrepareCorsServices()
            .PrepareMonitongServices()
            .PrepareExceptionHandlingServices();



        var app = builder.Build();
        app.MigrateDatabases();

        var containsSeederKeyword = app.HandleSeedingCommandFromCli(args: args);
        if (containsSeederKeyword) return;

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
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