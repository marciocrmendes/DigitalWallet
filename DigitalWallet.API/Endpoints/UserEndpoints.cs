using DigitalWallet.Application.UseCases.Users;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallet.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder RegisterUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users").RequireAuthorization().WithTags("Users");

        group
            .MapPost("/", CreateUser)
            .AllowAnonymous()
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("This endpoint allows you to create a new user in the system.")
            .Accepts<CreateUserUseCase.Request>("application/json")
            .Produces<CreateUserUseCase.Response>(201)
            .Produces<ProblemDetails>(500);

        group
            .MapGet("/{userId:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("This endpoint allows you to retrieve a user by their ID.")
            .Produces<GetUserByIdUseCase.Response>(200)
            .Produces<ProblemDetails>(500);

        group
            .MapGet("/{userId:guid}/wallets", GetUserWallets)
            .WithName(nameof(GetUserWallets))
            .WithSummary("Get Wallets by UserId")
            .WithDescription("This endpoint allows you to retrieve wallets by user ID.")
            .Produces<GetUserWalletsUseCase.Response>(200)
            .ProducesProblem(400)
            .ProducesProblem(500);

        group
            .MapGet("/{userId:guid}/transactions", GetUserTransactions)
            .WithName(nameof(GetUserTransactions))
            .WithSummary("Get user transactions")
            .WithDescription(
                "This endpoint allows you to retrieve all transactions for a user with optional date filtering."
            )
            .Produces<GetUserTransactionsUseCase.Response>(200)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500);

        return endpoints;
    }

    private static async Task<IResult> CreateUser(
        CreateUserUseCase.Request request,
        CreateUserUseCase createUserUseCase
    )
    {
        var result = await createUserUseCase.ExecuteAsync(request);

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Created($"/api/users/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> GetUserById(
        Guid userId,
        GetUserByIdUseCase getUserByIdUseCase
    )
    {
        var result = await getUserByIdUseCase.ExecuteAsync(new GetUserByIdUseCase.Request(userId));

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetUserWallets(Guid userId, GetUserWalletsUseCase useCase)
    {
        var result = await useCase.ExecuteAsync(new GetUserWalletsUseCase.Request(userId));

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetUserTransactions(
        Guid userId,
        GetUserTransactionsUseCase useCase,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        var request = new GetUserTransactionsUseCase.Request(userId, startDate, endDate);
        var result = await useCase.ExecuteAsync(request);

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Ok(result.Value);
    }
}
