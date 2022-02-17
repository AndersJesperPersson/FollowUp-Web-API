namespace SurveyAPI.DTO
{
    public class AnswerAndQuestionDTO
    {
        public Guid Id { get; set; }
        public string Reply { get; set; }
        public QuestionDTO Question { get; set; }
    }
}
