namespace SurveyAPI.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class QuestionCreationDTO
    {

        [StringLength(150, ErrorMessage = "Frågan får inte vara längre än 150 tecken")]
        public string Item { get; set; }
    }
}
