using System.Security.Claims;
using System.Text;
using dotenv.net;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Seeder;
using Inventory_Backend_NET.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NeoSmart.Caching.Sqlite.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSqliteCache(
    options =>
    {
        options.CachePath = Path.Combine(Environment.CurrentDirectory, "Cache/cache.db");
    }
);


builder.Services.AddControllers(options =>
{
    // untuk ngegunain JsonPropertyName kalo ada er
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    
    options.MaxModelValidationErrors = 100;             
});

builder.Services.AddDbContext<MyDbContext>(
    optionsAction: options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(name: MyConstants.AppSettingsKey.MyConnectionString)
        )
);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme , new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Masukkan token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
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
                context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("{\"message\" : \"Unauthorized\"}");
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        name: MyConstants.Policies.AdminOnly, 
        configurePolicy: policy => policy.RequireRole(MyConstants.Roles.Admin)
    );
    options.AddPolicy(
        name: MyConstants.Policies.AllUsers, 
        configurePolicy: policy => policy.RequireClaim(ClaimTypes.Role)
    );
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

// var spaPath = Path.Combine(
//     Directory.GetParent(
//         Environment.CurrentDirectory
//     )!.Parent!.FullName,
//     "web"
// );

builder.Services.AddSpaStaticFiles(options =>
{
    options.RootPath = "web";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<MyDbContext>();
    
        if (args.Contains("refresh"))
            db.RefreshDatabase();

        if (args.Contains("user-only"))
            services.SeedUser();
        else if (args.Contains("test-seeder"))
            services.TestSeeder(args: args);
    }

    if (args.Length > 0) { return; }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// app.UseDefaultFiles();

app.UseSpaStaticFiles(new StaticFileOptions
{
    // biar bisa ngeserve file don
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

DotEnv.Load();
var env = DotEnv.Read();
app.Run(env["APP_URL"]);