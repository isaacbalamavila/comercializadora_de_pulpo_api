using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Purchases
{
    public class PurchaseRequestDTO : PaginationRequest
    {
        public Guid? Supplier { get; set; }
        public int? RawMaterial { get; set; }
        public DateTime? Date { get; set; }
        public string? Search { get; set; }
    }
}
