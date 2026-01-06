using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Products
{
    public class ProductBatchesRequestDTO : PaginationRequest
    {
        public String? Search {  get; set; }
        public Guid? ProductId { get; set; }
        public bool? Availables { get; set; }
    }
}
