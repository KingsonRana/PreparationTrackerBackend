using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.Model
{
    public class Problems
    {
        
        public int Id { get; set; }
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid? ParentTopicId { get; set; }
        public string Name { get; set; }
        public PreparationTracker.Enum.ProblemLevel Level { get; set; } // Consider using an enum for Level
        public string Link { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        public PreparationTracker.Enum.RequireReWork RequireReWork { get; set; } // Consider using an enum for RequireReWork

        // Foreign key to Topic
        public Guid TopicGuid { get; set; }
        public Topic Topic { get; set; }
      
    }
}
