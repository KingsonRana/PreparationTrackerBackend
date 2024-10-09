using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.ResponseDTO
{
    public class UserLogInResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
