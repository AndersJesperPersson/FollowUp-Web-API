namespace SurveyAPI.DTO
{
    using Microsoft.AspNetCore.Mvc;
    using SurveyAPI.Helpers;
    using SurveyAPI.Models;
    using System.ComponentModel.DataAnnotations;

    public class MissionCreationDTO
    {


        public string ContactPerson { get; set; } 
        public string CompanyName { get; set; } 
        public bool IsActive { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string City { get; set; }

        public IFormFile Image { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<Guid>>))]
        public List<Guid>? EmployeesIds { get; set; } 

        [ModelBinder(BinderType = typeof(TypeBinder<List<Guid>>))]
        public List<Guid>? SurveysIds  { get; set; } 


    }
}
