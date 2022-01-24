namespace SurveyAPI.DTO
{
    public class SurveyDTO
    {

        public Guid SurveyId { get; set; }

        public DateTime Created { get; set; }

        public DateTime sendDate { get; set; }
        public bool IsSent { get; set; }

        public List<QuestionDTO> Questions { get; set; }

        public List<AnswerDTO> Answers { get; set; }

    }
}
