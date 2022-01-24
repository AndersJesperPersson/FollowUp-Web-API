namespace SurveyAPI.Models
{
    public class SurveysQuestions
    {
        public Guid SurveyId { get; set; }

        public Guid QuestionId { get; set; }

        public Question Question { get; set; }

        public Survey Survey { get; set; }
    }
}
