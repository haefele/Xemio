using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Xemio.Logic.Requests
{
    [Serializable]
    public class RequestValidationFailedException : Exception
    {
        public IList<ValidationFailure> Errors { get; }

        public RequestValidationFailedException(IList<ValidationFailure> errors)
        {
            this.Errors = errors;
        }
    }
}