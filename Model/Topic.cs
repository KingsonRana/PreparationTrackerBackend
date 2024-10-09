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

            public Guid? ParentId { get; set; }
            public Topic? Parent { get; set; }
            public ICollection<Problems> Problems { get; set; } = new List<Problems>();
            public ICollection<Topic> SubTopics { get; set; } = new List<Topic>();
            public Exam Exam { get; set; }
            public Guid ExamId { get; set; }

        }
    }
