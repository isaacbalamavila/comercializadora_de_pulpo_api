using System.ComponentModel.DataAnnotations;

namespace comercializadora_de_pulpo_api.Models.DTOs.Reports
{
    public class ReportRequestDTO
    {
        [Required(ErrorMessage ="La fecha de inicio es obligatoria")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "La fecha final es obligatoria")]
        public DateTime EndDate { get; set; }
    }
}
