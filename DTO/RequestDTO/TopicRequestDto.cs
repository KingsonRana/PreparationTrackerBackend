using System.ComponentModel.DataAnnotations;

namespace PreparationTracker.DTO.RequestDTO
{
    public class TopicRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int minQuestion { get; set; }

        public Guid? ParentId {  get; set; }
    }
}
