using comercializadora_de_pulpo_api.Models.DTOs.Pagination;

namespace comercializadora_de_pulpo_api.Models.DTOs.Proccess
{
    public class ProcessResponseDTO : PaginationResponse
    {
        public List<ProcessDTO> Processes { get; set; } = [];
    }
}
