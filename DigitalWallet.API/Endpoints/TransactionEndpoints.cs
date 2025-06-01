using DigitalWallet.Application.UseCases.Wallets;

namespace DigitalWallet.API.Endpoints
{
    public static class TransactionEndpoints
    {
        public static IEndpointRouteBuilder RegisterTransactionEndpoints(
            this IEndpointRouteBuilder endpoints
        )
        {
            var group = endpoints
                .MapGroup("/transaction")
                .RequireAuthorization()
                .WithTags("Transaction");

            group
                .MapPost("/transfer", CreateTransfer)
                .WithName(nameof(CreateTransfer))
                .WithSummary("Create transfer between wallets")
                .WithDescription("This endpoint allows you to transfer funds between two wallets.")
                .Produces<CreateTransferUseCase.Response>(200)
                .ProducesProblem(400)
                .ProducesProblem(404)
                .ProducesProblem(500);

            return endpoints;
        }

        private static async Task<IResult> CreateTransfer(
            CreateTransferUseCase.Request request,
            CreateTransferUseCase useCase
        )
        {
            var useCaseRequest = new CreateTransferUseCase.Request(
                request.FromWalletId,
                request.ToWalletId,
                request.Amount,
                request.Currency,
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
}
