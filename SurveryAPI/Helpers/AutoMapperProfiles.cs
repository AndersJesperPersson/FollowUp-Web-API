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

            CreateMap<AnswerCreationDTO, Answer>();
            CreateMap<AnswerDTO, Answer>().ReverseMap();
            CreateMap<Answer, AnswerAndQuestionDTO>().ReverseMap();
            

            CreateMap<MissionDTO, Mission>().ReverseMap();
            CreateMap<MissionCreationDTO, Mission>()
                .ForMember(x => x.Image, options => options.Ignore())
                .ForMember(x => x.MissionSurveys, options => options.MapFrom(MapMissionSurveys))
                .ForMember(x => x.MissionEmployees, options => options.MapFrom(MapMissionEmployees));
            CreateMap<Mission, MissionLandingPageDTO>();
            CreateMap<Mission, MissionDTO>()
                .ForMember(x => x.Surveys, options => options.MapFrom(MapMissionSurveys))
                .ForMember(x => x.Employees, options => options.MapFrom(MapMissionEmployees));
                

            CreateMap<SurveyCreationDTO, Survey>()
                .ForMember(x => x.SurveysQuestions, options => options.MapFrom(MapSurveysQuestions));
            //.ForMember(x=> x.SurveysAnswers, options => options.MapFrom(MapSurveysAnswers))
            CreateMap<Survey, SurveyDTO>()
                .ForMember(x => x.Questions, options => options.MapFrom(MapSurveysQuestions))
                .ForMember(x => x.Answers, options => options.MapFrom(MapSurveysAnswers));
        }


// Following methods do the same thing for different objects. They map underlaying objects to correct value so 
// it´s possible to either work with the object in .net or to send a correct object to front end. 
        private List<EmployeeDTO> MapMissionEmployees(Mission mission, MissionDTO missionDTO)
        {

            var result = new List<EmployeeDTO>();

            if (mission.MissionEmployees is not null)
            {
                foreach (var employee in mission.MissionEmployees)
                {

                    result.Add(new EmployeeDTO()
                    {
                        Id = employee.EmployeeId,
                        Email = employee.Employee.Email


                    });
                }
                return result;
            }
            return result;
        }


        private List<SurveyDTO> MapMissionSurveys(Mission mission, MissionDTO missionDTO)
        {

            var result = new List<SurveyDTO>();

            if (mission.MissionSurveys is not null)
            {
                foreach (var survey in mission.MissionSurveys)
                {

                    var questionsresult = new List<QuestionDTO>();

                    foreach (var question in survey.Survey.SurveysQuestions) 
                    {
                        questionsresult.Add(new QuestionDTO() { Id = question.QuestionId, Item = question.Question.Item });
                    }

                    var answersResult = new List<AnswerDTO>();

                    foreach (var answer in survey.Survey.SurveysAnswers)
                    {
                        answersResult.Add(new AnswerDTO() { Id = answer.AnswerId, Reply = answer.Answer.Reply});
                    }

                    result.Add(new SurveyDTO()
                    {
                        SurveyId = survey.SurveyId,
                        Questions = questionsresult,
                        Answers = answersResult
                        
                    });    
                
                }

                return result;
            }
            return result;
        }


private List<MissionSurveys> MapMissionEmployees(MissionCreationDTO missionCreationDTO, Mission mission)
        {
            var result = new List<MissionSurveys>();

            if (missionCreationDTO.SurveysIds is null)
            {
                return result;
            }

            foreach (var id in missionCreationDTO.SurveysIds)
            {
                result.Add(new MissionSurveys() { SurveyId = id });
            }

            return result;
        }

        private List<MissionEmployees> MapMissionSurveys(MissionCreationDTO missionCreationDTO,Mission mission)
        {
            var result = new List<MissionEmployees>();
            if(missionCreationDTO.SurveysIds is null)
            {
                return result;
            }

            foreach(var id in missionCreationDTO.EmployeesIds)
            {
                result.Add(new MissionEmployees() { EmployeeId = id });
            }

            return result;
        }

        private List<AnswerDTO> MapSurveysAnswers(Survey survey, SurveyDTO surveyDTO)
        {
            var result = new List<AnswerDTO>();

            if (survey.SurveysAnswers is not null)
            {
                foreach (var answer in survey.SurveysAnswers)
                {
                    result.Add(new AnswerDTO() { Id= answer.AnswerId, QuestionID = answer.Answer.Question.Id, 
                    Reply = answer.Answer.Reply});
                }
                return result;
            }
            return result;
        }

        private List<QuestionDTO> MapSurveysQuestions(Survey survey, SurveyDTO surveyDTO)
        {
            var result = new List<QuestionDTO>();

            if(survey.SurveysQuestions is not null)
            {
                foreach(var question in survey.SurveysQuestions)
                {
                    result.Add(new QuestionDTO() { Id = question.QuestionId, Item = question.Question.Item });
                }
                return result;
            }
            return result;
        }
        private List<SurveysQuestions> MapSurveysQuestions(SurveyCreationDTO surveyCreationDTO, Survey survey)
        {
            var result = new List<SurveysQuestions>();

            if(surveyCreationDTO.questionIds is null)
            {
                return result;
            }

            foreach(var id in surveyCreationDTO.questionIds)
            {
                result.Add(new SurveysQuestions() { QuestionId = id });
            }

            return result;
        }

    }
}
