using Microsoft.AspNetCore.Mvc.Filters;

namespace Hourglass.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.WriteAsync(exception.Message);
        }
    }
}
