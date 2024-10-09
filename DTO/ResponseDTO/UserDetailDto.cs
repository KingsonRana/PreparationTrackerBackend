namespace PreparationTracker.DTO.ResponseDTO
{
    public class UserDetailDto
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Gender { get; set; }

        public int Age { get; set; }

        public DateTime DOB { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
