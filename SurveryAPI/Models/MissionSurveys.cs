namespace SurveyAPI.Models
{
    public class MissionSurveys
    {
        public Guid SurveyId { get; set; }

        public Guid MissionId { get; set; }

        public Survey Survey { get; set; }

        public Mission Mission { get; set; }




    }
}
