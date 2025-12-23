using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Products
{
    public class ProductBatchesRequestDTO : PaginationRequest
    {
        public String? Sku {  get; set; }
        public Guid? ProductId { get; set; }
        public int? Status { get; set; }
    }
}
