using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Supplies_Inventory
{
    public class SuppliesRequestDTO  :PaginationRequest
    {
        public string? Search { get; set; }

        public int? RawMaterial { get; set; }

        public bool? Availables { get; set; }

    }
}
