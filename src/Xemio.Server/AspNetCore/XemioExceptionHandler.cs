using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xemio.Logic.Extensions;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Auth;
using Xemio.Logic.Requests.Notebooks;

namespace Xemio.Server.AspNetCore
{
    public static class XemioExceptionHandler
    {
        private static readonly Dictionary<Type, (int statusCode, string title, Func<Exception, string> detailFactory, Func<Exception, ProblemDetails> problemDetailsFactory)> _exceptionHandlers = new Dictionary<Type, (int, string, Func<Exception, string>, Func<Exception, ProblemDetails>)>();

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

            Handle<NotebookDoesNotExistException>(StatusCodes.Status404NotFound, "Notebook not found", "The specified notebook does not exist.");

            Handle<RequestAuthorizationAttributeMissingException>(StatusCodes.Status500InternalServerError, "Authorization attribute missing", f => $"The request \"{f.RequestType.FullName}\" is missing a authorization attribute.");
        }

        private static void Handle<T>(int statusCode, string title, string detail, Func<T, ProblemDetails> problemDetailsFactory = null)
            where T : Exception
        {
            Handle<T>(statusCode, title, f => detail, problemDetailsFactory);
        }
        private static void Handle<T>(int statusCode, string title, Func<T, string> detail, Func<T, ProblemDetails> problemDetailsFactory = null)
            where T : Exception
        {
            _exceptionHandlers.Add(typeof(T), (statusCode, title, e => detail.Invoke((T)e), e => problemDetailsFactory?.Invoke((T)e)));
        }

        public static ProblemDetails GetError(Exception exception)
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

            var problemDetails = handler.problemDetailsFactory(exception) ?? new ProblemDetails();
            problemDetails.Status = handler.statusCode;
            problemDetails.Title = handler.title;
            problemDetails.Detail = handler.detailFactory(exception);

            return problemDetails;
        }
    }

    public class BadRequestProblemDetails : ProblemDetails
    {
        public BadRequestProblemDetails()
        {
            this.Errors = new List<ValidationError>();
        }

        public List<ValidationError> Errors { get; set; }
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }
    }
}