namespace Accounts.API.Models.Requests
{
    public record PrepareTransferRequest(string TransactionId, Guid FromAccountId, Guid ToAccountId, decimal Amount)
    {
    }
}
