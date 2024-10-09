using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.RequestDTO
{
    public class UserLogInRequestDto
    {
        [Required]
        public string Email { get; set; }
  
        [Required]
        public string Password { get; set; }
    }
}
