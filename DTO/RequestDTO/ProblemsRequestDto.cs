using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.RequestDTO
{
    public class ProblemsRequestDto
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public PreparationTracker.Enum.ProblemLevel Level { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public PreparationTracker.Enum.RequireReWork RequireReWork { get; set; }
    }
}
