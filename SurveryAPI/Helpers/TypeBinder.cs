namespace SurveyAPI.Helpers
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class TypeBinder<T> : IModelBinder
    {

        /// <summary>
        /// When sending data FromForm into actions method it´s nesseccary to make a custom binding to be able to validate the value in Lists. 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
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
                    var deserializeValue = JsonConvert.DeserializeObject<T>(value.FirstValue);  // dependet of Newtonsoft.Json nugget package. 
                    bindingContext.Result = ModelBindingResult.Success(deserializeValue);
                }
                catch
                {
                    // give feedback if the value is not in correct value. 
                    bindingContext.ModelState.TryAddModelError(propertyNAme, "Det givna värdet är inte av korrekt typ.");
                }
                return Task.CompletedTask;
            }
        }
    }
}
