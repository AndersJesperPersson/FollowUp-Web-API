namespace SurveyAPI.APIBehavior
{
    using Microsoft.AspNetCore.Mvc;


    /// <summary>
    /// Overrides APIController attribute so we handle badrequest in a way that works fine with my own created action filter ParseBadRequest. 
    /// </summary>
    public class BadRequestBehavior
    {
        public static void Parse(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var response = new List<string>();
                foreach (var key in context.ModelState.Keys)
                {
                    foreach (var error in context.ModelState[key].Errors)
                    {
                        response.Add($"{key}: {error.ErrorMessage}");
                    }
                }
                return new BadRequestObjectResult(response);
            };
        }
    }
}
