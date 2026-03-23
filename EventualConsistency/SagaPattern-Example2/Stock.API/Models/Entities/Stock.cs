namespace Stock.API.Models.Entities
{
    public class Stock
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
