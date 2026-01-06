using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Proccess
{
    public class ProcessRequestDTO : PaginationRequest
    {
        public Guid? User { get; set; }
        public Guid? Product { get; set; }
        public int? Status { get; set; }
        public bool? IsMovil {  get; set; }
        public DateTime? Date { get; set; }
    }
}
