    using System.ComponentModel.DataAnnotations;

    namespace PreparationTracker.Model
    {
        public class User
        {
            [Key]
            public Guid UserId { get; set; } = Guid.NewGuid();
            [Required]
            public string Name { get; set; }
            public string Gender { get; set; }
            [Required]
            public int Age { get; set; }
            [Required]
            public DateTime? DOB { get; set; } = DateTime.MinValue;
            public DateTime CreatedOn { get; set; } = DateTime.Now;
            public DateTime UpdatedOn { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Phone { get; set; }
            [Required]
            public string Password { get; set; } 
            public ICollection<Exam> Exams { get; set; } 
        }

    }
