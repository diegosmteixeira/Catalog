using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        //Dependency Injection
        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        //Before Action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("### OnActionExecuting ###");
            _logger.LogInformation("#########################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
            _logger.LogInformation("#########################");
        }

        //After Action
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### OnActionExecuted ###");
            _logger.LogInformation("#########################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
            _logger.LogInformation("#########################");

        }
    }
}