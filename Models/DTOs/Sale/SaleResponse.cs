using System.Text.Json.Serialization;

namespace comercializadora_de_pulpo_api.Models.DTOs.Sale
{
    public class SaleResponse
    {
        public Guid Id { get; set; }
        public String Employee { get; set; } = null!;
        public String Client { get; set; } = null!;
        public String? ClientRfc {get;set;}
        public String PaymentMethod { get; set; } = null!;
        public DateTime Date { get; set; }

        public List<SaleItemResponse> Products { get; set; } = [];
        public Decimal Total { get; set; }
    }

    public class SaleItemResponse
    {
        [JsonIgnore]
        public Guid? SaleId { get; set; }
        public String Sku { get; set; } = null!;
        public String Name { get; set; } = null!;
        public int Content { get; set; }
        public String Unit { get; set; } = null!;
        public int Quantity { get; set; }
        public Decimal Price { get; set; }
        public Decimal Subtotal { get; set; }
    }
}
