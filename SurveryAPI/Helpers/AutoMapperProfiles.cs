namespace SurveyAPI.Helpers
{
    using AutoMapper;
    using SurveyAPI.DTO;
    using SurveyAPI.Models;

    public class AutoMapperProfiles:Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<QuestionDTO, Question>().ReverseMap();
            CreateMap<QuestionCreationDTO, Question>();
        }
    }
}
