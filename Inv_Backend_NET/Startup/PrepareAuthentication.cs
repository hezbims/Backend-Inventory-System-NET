using System.Security.Claims;
using System.Text;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Startup;

public static class PrepareAuthentication
{
    public static WebApplicationBuilder PrepareAuthenticationServices(this WebApplicationBuilder builder)
    {
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
        return builder;
    }
}