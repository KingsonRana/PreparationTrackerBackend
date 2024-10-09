using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.Model
{
    public class Exam
    {
        [Key]
        public Guid ExamId { get; set; } =  Guid.NewGuid();
        [Required]
        public string ExamName { get; set; }
        [Required]
        public DateTime ExamDate { get; set; } 
        public ICollection<Topic> Topics { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public Guid CreatedBy { get; set; } 
        public DateTime UpdatedOn { get; set; }
        public Guid? UpdatedBy { get; set; } 
        public double DailyHoursSpent { get; set; }
        [Required]
        public Guid UserId { get; set; } 
        public User User { get; set; } 
    }

}
