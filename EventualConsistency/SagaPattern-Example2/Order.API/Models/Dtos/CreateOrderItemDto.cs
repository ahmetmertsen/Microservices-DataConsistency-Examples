namespace Order.API.Models.Dtos
{
    public class CreateOrderItemDto
    {
        public long ProductId { get; set; }
        public int Count { get; set; }  
    }
}
