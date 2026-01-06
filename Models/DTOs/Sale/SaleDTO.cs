using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Sale
{

    public class SalesResponseDTO : PaginationResponse {
        public List<SaleDTO> Sales { get; set; } = [];
    }
    public class SaleDTO
    {
        public Guid Id { get; set; }
        public String Employee { get; set; } = null!;
        public String Client { get; set; } = null!;
        public DateTime Date {  get; set; }
        public Decimal Total { get; set; }


    }
}
