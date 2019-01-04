using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Xemio.Server.AspNetCore
{
    public class BadRequestProblemDetails : ProblemDetails
    {
        public BadRequestProblemDetails()
        {
            this.Errors = new List<ValidationError>();
        }

        public List<ValidationError> Errors { get; set; }
    }
}