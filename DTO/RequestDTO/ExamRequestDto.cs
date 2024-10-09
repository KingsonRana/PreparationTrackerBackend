using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.RequestDTO
{
    public class ExamRequestDto
    {
        
        public string? ExamName { get; set; }
        public DateTime? ExamDate { get; set; }
        public double DailyHoursSpent { get; set; } = 0;
    }
}
