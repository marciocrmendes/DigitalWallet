namespace DigitalWallet.Application.Interfaces
{
    public interface IUseCase<TRequest, TResponse>
    {
        public Task<TResponse> ExecuteAsync(TRequest request);
    }

    public interface IUseCase<TRequest>
    {
        public Task ExecuteAsync(TRequest request);
    }
}
