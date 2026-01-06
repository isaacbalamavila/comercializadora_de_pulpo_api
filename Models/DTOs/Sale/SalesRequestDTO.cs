using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Sale
{
    public class SalesRequestDTO : PaginationRequest
    {
        public Guid? EmployeeId { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime? Date { get; set; }
    }
}
