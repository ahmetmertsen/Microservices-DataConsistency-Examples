namespace Ledger.API.Models.Entities
{
    public class Ledger
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public long FromAccount { get; set; }
        public long ToAccount { get; set; }
        public decimal amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
