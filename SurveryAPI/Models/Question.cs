namespace SurveyAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Question
    {
        [Key]
        public Guid Id { get; set; }
        public string Item { get; set; }

    }
}
