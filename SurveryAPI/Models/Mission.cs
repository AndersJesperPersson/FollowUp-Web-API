
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SurveyAPI.Models

{
    public class Mission
    {
        [Key]
        public Guid Id { get; set; }
        public string ContactPerson { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; }
        public List<Survey> Survey { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
