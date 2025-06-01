using DigitalWallet.API.Endpoints;

namespace DigitalWallet.API.Extensions
{
    public static class DependencyInjectorEndpoints
    {
        public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpoints)
        {
            return endpoints
                .MapGroup("/api/v1")
                .RequireAuthorization()
                .RegisterIdentityEndpoints()
                .RegisterUserEndpoints()
                .RegisterWalletEndpoints()
                .RegisterTransactionEndpoints();
        }
    }
}
