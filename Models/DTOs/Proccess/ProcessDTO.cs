namespace comercializadora_de_pulpo_api.Models.DTOs.Proccess
{
    public class ProcessDTO
    {
        public Guid Id { get; set; }

        public string ProductSku { get; set; } = null!;

        public string User { get; set; } = null!;

        public string Product { get; set; } = null!;

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int StatusId { get; set; }
    }
}
