namespace ServicesApi.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class CustomActionFiler : IActionFilter
    {
        private readonly ILogger<CustomActionFiler> logger;

        public CustomActionFiler(ILogger<CustomActionFiler> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.logger.LogInformation("Before on action executing");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            this.logger.LogInformation("After on action executed");
        }
    }
}
