namespace SurveyAPI.DTO
{
    public class QuestionDTO
    {
        // Don´t need any attributes here, cause this class only taking care of sending the obejct to frontend. Not creation.
        public Guid Id { get; set; }  
        public string Item { get; set; }
    }
}
