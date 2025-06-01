using Microsoft.OpenApi.Models;

namespace DigitalWallet.API.Extensions;

public static class ServicesCollectionExtensionsSwagger
{
    public static IServiceCollection AddSwaggerDependencies(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => $"{type.Name}_{type.MetadataToken}");

            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Digital Wallet API",
                    Version = "v1",
                    Description = "API documentation for Digital Wallet",
                }
            );

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                }
            );
        });

        return services;
    }

    public static WebApplication UseSwaggerDependencies(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Digital Wallet Management API v1"
                );
                options.RoutePrefix = string.Empty;
            });
        }

        return app;
    }
}
