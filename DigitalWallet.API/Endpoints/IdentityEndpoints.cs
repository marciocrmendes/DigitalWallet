using DigitalWallet.Application.Interfaces;
using DigitalWallet.CrossCutting.Validation;
using Microsoft.AspNetCore.Mvc;
using static DigitalWallet.Application.UseCases.Identity.LoginUseCase;

namespace DigitalWallet.API.Endpoints
{
    public static class IdentityEndpoints
    {
        public static IEndpointRouteBuilder RegisterIdentityEndpoints(
            this IEndpointRouteBuilder endpoints
        )
        {
            var group = endpoints.MapGroup("/api/identity").AllowAnonymous().WithTags("Auth");

            group
                .MapPost("/", Auth)
                .WithName("Auth")
                .WithSummary("Authentication")
                .WithDescription(
                    "This endpoint allows you to authenticate a user and obtain a JWT token."
                )
                .Accepts<Request>("application/json")
                .Produces<Response>(201)
                .Produces<ProblemDetails>(500);

            return endpoints;
        }

        private static async Task<IResult> Auth(
            Request request,
            IUseCase<Request, Result<Response>> loginUseCase
        )
        {
            var result = await loginUseCase.ExecuteAsync(request);

            if (!result.IsSuccess)
            {
                return Results.Problem(result.Error!.ToProblemDetails());
            }

            return Results.Ok(result.Value);
        }
    }
}
