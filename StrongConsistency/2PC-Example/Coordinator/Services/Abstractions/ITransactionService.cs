namespace Coordinator.Services.Abstractions
{
    public interface ITransactionService
    {
        Task<Guid> CreateTransactionAsync();
        Task PreapreServicesAsync(Guid transactionId);
        Task<bool> CheckReadyServicesAsync(Guid transactionId);
        Task CommitAsync(Guid transactionId);
        Task<bool> CheckTransactionStateServicesAsync(Guid transactionId);
        Task RoolbackAsync(Guid transactionId);
    }
}
