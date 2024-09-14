namespace PreparationTracker.DTO.ResponseDTO
{
    public class TopicResponseDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int minQuestion { get; set; }
        public int QuestionSolved { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
