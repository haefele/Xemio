using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xemio.Logic.Requests;

namespace Xemio.Server.AspNetCore
{
    public static class XemioExceptionHandler
    {
        private static readonly Dictionary<Type, (int statusCode, string title, string detail)> _exceptionHandlers = new Dictionary<Type, (int statusCode, string title, string detail)>();

        static XemioExceptionHandler()
        {
            Handle<UnauthorizedRequestException>(StatusCodes.Status401Unauthorized, "Unauthorized", "Whaaaat");
            Handle<RequestValidationFailedException>(StatusCodes.Status400BadRequest, "Bad request", "Something something");
        }

        private static void Handle<T>(int statusCode, string title, string detail)
        {
            _exceptionHandlers.Add(typeof(T), (statusCode, title, detail));
        }

        public static ProblemDetails GetError(Exception exception)
        {
            var exceptionType = exception.GetType();

            if (!_exceptionHandlers.ContainsKey(exceptionType))
                return new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unexpected error.",
                    Detail = "Some unexpected error occurred.",
                };
            
            var handler = _exceptionHandlers[exceptionType];

            return new ProblemDetails
            {
                Status = handler.statusCode,
                Title = handler.title,
                Detail = handler.detail
            };
        }
    }
}