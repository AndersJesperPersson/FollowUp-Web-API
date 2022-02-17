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

        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
  
        public string City { get; set; }

        public string Image { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<EmployeeDTO>>))]
        public List<EmployeeDTO>? Employees { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<SurveyDTO>>))]
        public List<SurveyDTO>? Surveys { get; set; }
    }
}
