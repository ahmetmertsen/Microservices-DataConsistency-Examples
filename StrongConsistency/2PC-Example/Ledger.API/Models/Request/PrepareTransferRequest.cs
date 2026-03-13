namespace Ledger.API.Models.Request
{
    public record PrepareTransferRequest(string TransactionId, Guid FromAccountId, Guid ToAccountId, decimal Amount)
    {
    }
}
