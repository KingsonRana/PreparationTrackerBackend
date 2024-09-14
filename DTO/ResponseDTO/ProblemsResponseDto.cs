namespace PreparationTracker.DTO.ResponseDTO
{
    public class ProblemsResponseDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; } 
        public PreparationTracker.Enum.ProblemLevel Level { get; set; }
        public string Link { get; set; }
        public PreparationTracker.Enum.RequireReWork RequireReWork { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
