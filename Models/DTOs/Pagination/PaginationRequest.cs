using System.ComponentModel.DataAnnotations;

namespace comercializadora_de_pulpo_api.Models.DTOs.Pagination
{
    public class PaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor o igual a uno")]
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue, ErrorMessage = "Los elementos por página deben ser mayor o igual a uno")]
        public int PageSize { get; set; } = 10;
    }
}
