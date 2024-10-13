using dotenv.net;
using Inventory_Backend_NET.Startup;

var builder = WebApplication.CreateBuilder(args);

// Khusus kalo ngegunain minimal API, agar muncul di swagger
// builder.Services.AddEndpointsApiExplorer();

builder
    .PrepareDependencyInjectionServices()
    .PrepareSwaggerWithJwtInputService()
    .PrepareAuthenticationServices()
    .PrepareCorsServices()
    .PrepareSpaServices();



var app = builder.Build();

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
app.MapControllers();
app.PrepareSpaServices();




DotEnv.Load();
var env = DotEnv.Read();
app.Run(env["APP_URL"]);