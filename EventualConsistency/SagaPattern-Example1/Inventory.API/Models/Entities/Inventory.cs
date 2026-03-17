namespace Inventory.API.Models.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
