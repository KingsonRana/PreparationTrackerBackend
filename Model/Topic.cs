using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.Model
{
    public class Topic
    {
        public int Id { get; set; }
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int minQuestion {  get; set; }
        public int QuestionSolved { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
        

        // Navigation property for the one-to-many relationship
        public ICollection<Problems> Problems { get; set; }

    }
}
