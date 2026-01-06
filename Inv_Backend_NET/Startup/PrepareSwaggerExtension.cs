using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Inventory_Backend_NET.Startup;

public static class PrepareSwaggerExtension
{
    public static WebApplicationBuilder PrepareSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "1.0.0",
                Title = "Inventory System API Specification"
            });
            
            option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme , new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Masukkan token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            option.OperationFilter<AllowAnonymousOperationFilter>();
            
            option.EnableAnnotations();
            option.ExampleFilters();
            option.SupportNonNullableReferenceTypes();
        });
        
        builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
        
        return builder;
    }
    
    
    [UsedImplicitly]
    private class AllowAnonymousOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodAttributes = context.ApiDescription.CustomAttributes().ToArray();
            var controllerAttributes = (context.ApiDescription.ActionDescriptor as ControllerActionDescriptor)
                ?.ControllerTypeInfo
                .GetCustomAttributes()
                .ToArray();
            
            bool methodAllowAnonymous = methodAttributes.Any(attr => attr is AllowAnonymousAttribute);
            bool controllerAllowAnonymous = controllerAttributes?.Any(attr => attr is AllowAnonymousAttribute) == true;

            if (methodAllowAnonymous || controllerAllowAnonymous)
                return;
            
            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme // sesuaikan sama cheme di security definition
                            }
                        },
                        []
                    }
                }
            ];
        }
    }

}