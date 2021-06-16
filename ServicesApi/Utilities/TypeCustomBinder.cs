namespace ServicesApi.Utilities
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Newtonsoft.Json;

    public class TypeCustomBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var propertyValue = bindingContext.ValueProvider.GetValue(propertyName);
            if (propertyValue == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var deserializeObject = JsonConvert.DeserializeObject<T>(propertyValue.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializeObject);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "Property value is not a data type valid");
            }

            return Task.CompletedTask;
        }
    }
}