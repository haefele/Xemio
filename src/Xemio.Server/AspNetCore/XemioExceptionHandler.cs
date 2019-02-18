using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Server.AspNetCore
{
    public static class XemioExceptionHandler
    {
        private static readonly Dictionary<Type, (int statusCode, string title, Func<Exception, IServiceProvider, string> detailFactory, Func<Exception, IServiceProvider, ProblemDetails> problemDetailsFactory)> _exceptionHandlers = new Dictionary<Type, (int, string, Func<Exception, IServiceProvider, string>, Func<Exception, IServiceProvider, ProblemDetails>)>();

        static XemioExceptionHandler()
        {
            Handle<UnauthorizedRequestException>(StatusCodes.Status401Unauthorized, "Unauthorized", "You are not logged in.");
            
            Handle<RequestValidationFailedException>(StatusCodes.Status400BadRequest, "Bad request", "You request is invalid.", e => 
                new BadRequestProblemDetails
                {
                    Errors = e.Errors
                        .Select(f => new ValidationError { PropertyName = f.PropertyName, Error = f.ErrorMessage })
                        .ToList()
                });

            Handle<IncorrectPasswordException>(StatusCodes.Status404NotFound, "Login failed", "Email address or password is incorrect.");
            Handle<NoUserWithEmailAddressExistsException>(StatusCodes.Status404NotFound, "Login failed", "Email address or password is incorrect.");

            Handle<EmailAddressAlreadyInUseException>(StatusCodes.Status409Conflict, "Registration failed", "Email address is already in use.");

            Handle<RequestAuthorizationAttributeMissingException>(StatusCodes.Status500InternalServerError, "Authorization attribute missing", f => $"The request \"{f.RequestType.FullName}\" is missing a authorization attribute.");
        }

        private static void Handle<T>(int statusCode, string title, string detail, Func<T, ProblemDetails> problemDetailsFactory = null)
            where T : Exception
        {
            Handle(statusCode, title, f => detail, problemDetailsFactory);
        }
        private static void Handle<T>(int statusCode, string title, Func<T, string> detail, Func<T, ProblemDetails> problemDetailsFactory = null)
            where T : Exception
        {
            Handle(statusCode, title, (s, e) => detail(s), problemDetailsFactory);
        }
        private static void Handle<T>(int statusCode, string title, Func<T, IServiceProvider, string> detail, Func<T, ProblemDetails> problemDetailsFactory = null)
            where T : Exception
        {
            _exceptionHandlers.Add(typeof(T), (statusCode, title, (e, s) => detail.Invoke((T)e, s), (e, s) => problemDetailsFactory?.Invoke((T)e)));
        }

        public static ProblemDetails GetError(Exception exception, IServiceProvider requestServices)
        {
            var exceptionType = exception.GetType();

            if (!_exceptionHandlers.ContainsKey(exceptionType))
                return new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unexpected error",
                    Detail = "An unexpected error occurred.",
                };
            
            var handler = _exceptionHandlers[exceptionType];

            var problemDetails = handler.problemDetailsFactory(exception, requestServices) ?? new ProblemDetails();
            problemDetails.Status = handler.statusCode;
            problemDetails.Title = handler.title;
            problemDetails.Detail = handler.detailFactory(exception, requestServices);

            return problemDetails;
        }
    }
}