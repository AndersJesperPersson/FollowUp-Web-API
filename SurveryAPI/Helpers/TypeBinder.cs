namespace SurveyAPI.Helpers
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyNAme = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(propertyNAme);

            if (value == ValueProviderResult.None) // if theres no value, nothing to bind. 
            {
                return Task.CompletedTask;
            }
            else
            {
                try
                {
                    var deserializeValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
                    bindingContext.Result = ModelBindingResult.Success(deserializeValue);
                }
                catch
                {
                    bindingContext.ModelState.TryAddModelError(propertyNAme, "Det givna värdet är inte av korrekt typ.");
                }
                return Task.CompletedTask;
            }
        }
    }
}
