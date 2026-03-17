using Order.API.Models.Enums;

namespace Order.API.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
