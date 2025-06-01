using System.Text.Json.Serialization;
using DigitalWallet.Application.UseCases.Users;
using DigitalWallet.CrossCutting.Options;
using DigitalWallet.Domain.Interfaces.UnitOfWork;
using DigitalWallet.Infrastructure.Context;
using DigitalWallet.Infrastructure.Interceptors;
using DigitalWallet.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DigitalWallet.API.Extensions
{
    public static class DependencyInjectorAPI
    {
        public static IServiceCollection AddApplicationDepencencies(
            this IServiceCollection services,
            IConfigurationManager configuration
        )
        {
            services
                .AddJsonConfiguration()
                .AddCorsConfiguration()
                .AddDatabaseContextConfiguration(configuration)
                .AddUseCases()
                .AddRepositories()
                .AddOptions(configuration);

            return services;
        }

        private static IServiceCollection AddJsonConfiguration(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
            });

            return services;
        }

        private static IServiceCollection AddDatabaseContextConfiguration(
            this IServiceCollection services,
            IConfigurationManager configuration
        )
        {
            services
                .AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>()
                .AddDbContext<DigitalWalletDbContext>(
                    (sp, options) =>
                    {
                        var connnectionString = configuration.GetConnectionString(
                            "DefaultConnection"
                        );

                        if (string.IsNullOrWhiteSpace(connnectionString))
                        {
                            throw new InvalidOperationException(
                                "Connection string 'Postgres' is not configured."
                            );
                        }

                        options
                            .UseNpgsql(
                                connnectionString,
                                npgsqlOptions =>
                                {
                                    npgsqlOptions.EnableRetryOnFailure(
                                        3,
                                        TimeSpan.FromSeconds(3),
                                        null
                                    );
                                    npgsqlOptions.MigrationsAssembly(
                                        typeof(DigitalWalletDbContext).Assembly
                                    );
                                }
                            )
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();

                        options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    }
                );

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.Scan(scan =>
                scan.FromAssemblyOf<TransactionRepository>()
                    .AddClasses(classes => classes.AssignableTo(typeof(BaseRepository<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            return services;
        }

        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.Scan(scan =>
                scan.FromAssemblyOf<CreateUserUseCase>()
                    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("UseCase")))
                    .AsSelf()
                    .WithScopedLifetime()
            );

            return services;
        }

        private static IServiceCollection AddOptions(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }

        private static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                        builder
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                )
            );

            return services;
        }
    }
}
