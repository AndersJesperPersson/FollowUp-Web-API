namespace SurveyAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Question
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50, ErrorMessage="Frågan får inte vara längre än 50 tecken")]
        public string Item { get; set; }

    }
}
