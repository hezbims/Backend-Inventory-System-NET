using System.Security.Claims;
using System.Text;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend_NET.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NeoSmart.Caching.Sqlite.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IJwtTokenBuilder, JwtTokenBuilder>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSqliteCache(
    options =>
    {
        options.CachePath = Path.Combine(Environment.CurrentDirectory, "Cache/cache.db");
    }
);
Console.WriteLine(Path.Combine(Environment.CurrentDirectory, "Cache/cache.db"));
builder.Services.AddControllers();
builder.Services.AddDbContext<MyDbContext>(
    options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"))
);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Masukkan token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var validIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
        var issuerKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer!,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(issuerKey!)    
            )
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context  =>
            {
                context.HandleResponse();
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("{\"message\" : \"Unauthorized\"}");
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly , policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy(Policies.AllUsers , policy => policy.RequireClaim(ClaimTypes.Role));
});

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MyDbContext>();
    
    if (args.Contains("refresh"))
    {
        db.RefreshDatabase();
    }
    if (args.Contains("test-seeder"))
    {
        services.TestSeeder();
    } 
}

if (args.Length > 0) { return; }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();