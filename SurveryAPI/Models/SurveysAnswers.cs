namespace SurveyAPI.Models
{
    public class SurveysAnswers
    {
        public Guid SurveyId { get; set; }

        public Guid AnswerId { get; set; }

        public Answer Answer { get; set; }

        public Survey Survey { get; set; }
    }
}
