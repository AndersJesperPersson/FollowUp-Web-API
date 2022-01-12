namespace SurveyAPI.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class QuestionCreationDTO
    {

        [StringLength(50, ErrorMessage = "Frågan får inte vara längre än 50 tecken")]
        public string Item { get; set; }
    }
}
