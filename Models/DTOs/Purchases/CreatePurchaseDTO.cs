using System.ComponentModel.DataAnnotations;

namespace comercializadora_de_pulpo_api.Models.DTOs.Purchases
{
    public class CreatePurchaseDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        public int RawMaterialId { get; set; }

        [Required]
        public decimal TotalKg { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
