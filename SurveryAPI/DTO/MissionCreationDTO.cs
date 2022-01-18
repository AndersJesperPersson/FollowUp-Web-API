namespace SurveyAPI.DTO
{
    using SurveyAPI.Models;
    using System.ComponentModel.DataAnnotations;

    public class MissionCreationDTO
    {

    
        public string ContactPerson { get; set; }
       
        public string CompanyName { get; set; }

        public bool isActive { get; set; }
      
        public string City { get; set; }
        public IFormFile Image { get; set; }
    }
}
