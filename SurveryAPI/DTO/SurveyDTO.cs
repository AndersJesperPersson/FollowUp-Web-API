namespace SurveyAPI.DTO
{
    using Microsoft.AspNetCore.Mvc;
    using SurveyAPI.Helpers;

    public class SurveyDTO
    {

        public Guid SurveyId { get; set; }

        public DateTime Created { get; set; }

        public DateTime sendDate { get; set; }
        public bool IsSent { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<QuestionDTO>>))]
        public List<QuestionDTO> Questions { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<AnswerDTO>>))]
        public List<AnswerDTO> Answers { get; set; }

    }
}
