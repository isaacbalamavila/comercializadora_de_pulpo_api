using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory
{
    public class SuppliesResponseDTO : PaginationResponse
    {
        public List<SupplyDTO> Supplies { get; set; } = [];
    }
}
