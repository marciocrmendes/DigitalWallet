using DigitalWallet.Application.UseCases.Wallets;

namespace DigitalWallet.API.Endpoints;

public static class WalletEndpoints
{
    public static IEndpointRouteBuilder RegisterWalletEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        var group = endpoints.MapGroup("/wallets").RequireAuthorization().WithTags("Wallets");

        group
            .MapPost("/", CreateWallet)
            .WithName(nameof(CreateWallet))
            .WithSummary("Create a new wallet")
            .WithDescription("This endpoint allows you to create a new wallet for a user.")
            .Produces<CreateWalletUseCase.Response>(201)
            .ProducesProblem(400)
            .ProducesProblem(500);

        group
            .MapPost("/{walletId:guid}/balance", AddBalanceToWallet)
            .WithName(nameof(AddBalanceToWallet))
            .WithSummary("Add balance to wallet")
            .WithDescription("This endpoint allows you to add balance to a specific wallet.")
            .Produces<AddBalanceToWalletUseCase.Response>(200)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500);

        group
            .MapGet("/{walletId:guid}/balance", GetWalletBalance)
            .WithName(nameof(GetWalletBalance))
            .WithSummary("Get wallet balance")
            .WithDescription(
                "This endpoint allows you to retrieve the balance of a specific wallet."
            )
            .Produces<GetWalletBalanceUseCase.Response>(200)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(500);

        return endpoints;
    }

    private static async Task<IResult> CreateWallet(
        CreateWalletUseCase.Request createWalletDto,
        CreateWalletUseCase useCase
    )
    {
        var result = await useCase.ExecuteAsync(createWalletDto);

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Created($"/api/wallets/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> GetWalletBalance(
        Guid walletId,
        GetWalletBalanceUseCase useCase
    )
    {
        var result = await useCase.ExecuteAsync(new GetWalletBalanceUseCase.Request(walletId));

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> AddBalanceToWallet(
        Guid walletId,
        AddBalanceToWalletUseCase.Request request,
        AddBalanceToWalletUseCase useCase
    )
    {
        var useCaseRequest = new AddBalanceToWalletUseCase.Request(
            walletId,
            request.Amount,
            request.Description,
            request.Reference
        );

        var result = await useCase.ExecuteAsync(useCaseRequest);

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error!.ToProblemDetails());
        }

        return Results.Ok(result.Value);
    }
}
