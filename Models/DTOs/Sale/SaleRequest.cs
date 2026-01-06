namespace comercializadora_de_pulpo_api.Models.DTOs.Sale
{
    public class SaleRequest
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public int PaymentMethodId { get; set; }
        public List<SaleItem> Products { get; set; } = [];
    }

    public class SaleItem { 
        public Guid ProductId {get; set;}
        public int Quantity { get; set;}
    }
}
