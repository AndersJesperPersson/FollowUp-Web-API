
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SurveyAPI.Models

{
    public class Mission
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ContactPerson { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public string City { get; set; }

        public string Image { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<MissionSurveys> MissionSurveys { get; set; }
        public List<MissionEmployees> MissionEmployees { get; set; }

    }
}
