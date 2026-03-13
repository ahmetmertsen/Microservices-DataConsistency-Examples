using Ledger.API.Models.Enums;

namespace Ledger.API.Models.Entities
{
    public class Ledger
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
