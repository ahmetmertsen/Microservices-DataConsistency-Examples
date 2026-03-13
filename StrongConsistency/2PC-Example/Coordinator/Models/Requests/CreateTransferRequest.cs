namespace Coordinator.Models.Requests
{
    public record CreateTransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);
}
