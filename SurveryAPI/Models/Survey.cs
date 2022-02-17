namespace SurveyAPI.Models
{
 using System.ComponentModel.DataAnnotations;

    public class Survey
    {
       
        public Guid SurveyId { get; set; }

        public DateTime Created { get; set; }

        public DateTime sendDate { get; set; }
        public bool IsSent { get; set; }

        public List<SurveysQuestions> SurveysQuestions { get; set; }
        public List<SurveysAnswers> SurveysAnswers { get; set; }

    }
}
