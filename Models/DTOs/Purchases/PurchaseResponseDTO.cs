using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Purchases
{
    public class PurchaseResponseDTO : PaginationResponse
    {
        public List<PurchaseDTO> Purchases { get; set; } = [];
    }
}
