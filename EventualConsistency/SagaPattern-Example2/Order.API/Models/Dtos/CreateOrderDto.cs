namespace Order.API.Models.Dtos
{
    public class CreateOrderDto
    {
        public long BuyerId { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; }
    }
}
