namespace SurveyAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Question
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(150, ErrorMessage="Frågan får inte vara längre än 150 tecken")]
        public string Item { get; set; }
        

    }
}
