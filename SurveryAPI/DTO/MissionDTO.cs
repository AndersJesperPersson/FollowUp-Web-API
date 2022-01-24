namespace SurveyAPI.DTO
{
    using Microsoft.AspNetCore.Mvc;
    using SurveyAPI.Helpers;
    using SurveyAPI.Models;

    public class MissionDTO
    {
        public Guid Id { get; set; }

        public string ContactPerson { get; set; }
  
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
  
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
  
        public string City { get; set; }

        public string Image { get; set; }

        public List<EmployeeDTO>? Employees { get; set; }
        public List<SurveyDTO>? Surveys { get; set; }
    }
}
