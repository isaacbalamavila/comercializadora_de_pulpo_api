namespace comercializadora_de_pulpo_api.Models.DTOs.Pagination
{
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
