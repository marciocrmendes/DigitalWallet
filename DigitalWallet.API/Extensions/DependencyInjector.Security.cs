using System.Text;
using DigitalWallet.Application.Providers;
using DigitalWallet.CrossCutting;
using DigitalWallet.CrossCutting.Options;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DigitalWallet.API.Extensions
{
    public static class DependencyInjectorSecutiry
    {
        public static IServiceCollection AddSecurity(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddHttpContextAccessor()
                .AddJwtConfiguration(configuration)
                .AddIdentityApiEndpoints<User>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.MaxValue;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<DigitalWalletDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<AppUser>().AddScoped<TokenProvider>();

            services.AddSwaggerDependencies();

            return services;
        }

        public static WebApplication UseSecurity(this WebApplication app)
        {
            app.UseAuthentication().UseAuthorization();

            return app;
        }

        private static IServiceCollection AddJwtConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddAuthorization()
                .AddAuthentication(authOptions =>
                {
                    authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearerOptions =>
                {
                    var jwtSettings =
                        configuration.GetSection("JwtSettings").Get<JwtSettings>()
                        ?? throw new InvalidOperationException(
                            "JWT settings are not configured properly."
                        );

                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.TokenSecurityKey)
                        ),
                        ValidAudience = jwtSettings.Issuer,
                        ValidIssuer = jwtSettings.Audience,
                        ClockSkew = TimeSpan.Zero,
                    };
                    bearerOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Don't write to response here, let the exception handler middleware handle it
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        },
                    };

                    bearerOptions.SaveToken = true;
                });

            return services;
        }
    }
}
