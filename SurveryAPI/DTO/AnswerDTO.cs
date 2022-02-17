namespace SurveyAPI.DTO
{
    using SurveyAPI.Models;

    public class AnswerDTO
    {
        public Guid Id { get; set; }
        public string Reply { get; set; }
        public Guid QuestionID { get; set; }
    }
}
