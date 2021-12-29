namespace SurveyAPI.Models
{
 using System.ComponentModel.DataAnnotations;

    public class Survey
    {
        [Key]
        public Guid Id { get; set; }
        public Mission Mission { get; set; }
        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public DateTime Created { get; set; }
        public bool IsSent { get; set; }



    }
}
