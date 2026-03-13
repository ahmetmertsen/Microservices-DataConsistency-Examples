using Accounts.API.Models.Enums;

namespace Accounts.API.Models.Entities
{
    public class AccountTransferIntent
    {
        public string TransactionId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
