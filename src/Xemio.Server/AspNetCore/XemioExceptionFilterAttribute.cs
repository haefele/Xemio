using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Xemio.Server.AspNetCore
{
    public class XemioExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<XemioExceptionFilterAttribute> _logger;
        
        public XemioExceptionFilterAttribute(ILogger<XemioExceptionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            this._logger.LogError(context.Exception, "Swaggi message upsi daysi");

            var error = XemioExceptionHandler.GetError(context.Exception);

            context.Result = new ObjectResult(error)
            {
                StatusCode = error.Status,
            };
        }
    }
}