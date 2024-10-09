using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.RequestDTO
{
    public class UserSignupRequestDto
    {
        [Required]
        public string Name { get; set; }
        public string Gender { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public DateTime? DOB { get; set; } = DateTime.MinValue;
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
