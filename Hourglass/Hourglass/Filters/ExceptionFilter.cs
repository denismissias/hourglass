using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Hourglass.Filters
{
    /// <summary>
    /// Global exception filter that handles uncaught exceptions and returns standardized error responses
    /// </summary>
    public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
    {
        /// <summary>
        /// Handles exceptions by logging them and returning a standardized error response
        /// </summary>
        /// <param name="context">The exception context</param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            logger.LogError(exception, "An unhandled exception occurred: {ExceptionMessage}", exception.Message);

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var response = new
            {
                message = "An error occurred while processing your request.",
                traceId = context.HttpContext.TraceIdentifier
            };

            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new Microsoft.AspNetCore.Mvc.JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
