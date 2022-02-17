namespace SurveyAPI.Models
{
    public class MissionEmployees
    {
        public Guid MissionId { get; set; }

        public Guid EmployeeId { get; set; }

        public Mission Mission { get; set; }

        public Employee Employee { get; set; }

 
    }
}
