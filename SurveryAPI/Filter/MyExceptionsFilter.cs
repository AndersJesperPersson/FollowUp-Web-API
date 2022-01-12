namespace SurveyAPI.Filter
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class MyExceptionsFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionsFilter> _logger;
        public MyExceptionsFilter(ILogger<MyExceptionsFilter> logger)
        {
            _logger = logger;

        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
