namespace PreparationTracker.DTO.ResponseDTO
{
    public class ExamResponseDto
    {
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public double DailyHoursSpent { get; set; }
    }
}
