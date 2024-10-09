namespace PreparationTracker.DTO.RequestDTO
{
    public class UserUpdateRequestDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DOB { get; set; }
    }

}
