namespace SurveyAPI.DTO
{
    using Microsoft.AspNetCore.Mvc;
    using SurveyAPI.Helpers;
    using SurveyAPI.Models;

    public class SurveyCreationDTO
    {
        public List<Guid> questionIds { get; set; }
        public DateTime sendDate { get; set; }

        public Guid? missionId { get; set; }
       
    }
}
