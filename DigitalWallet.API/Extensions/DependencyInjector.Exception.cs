using DigitalWallet.API.Configurations;

namespace DigitalWallet.API.Extensions
{
    public static class DependencyInjectorException
    {
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();

            return services;
        }
    }
}
