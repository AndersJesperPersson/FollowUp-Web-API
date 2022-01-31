namespace SurveyAPI.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class UserCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
