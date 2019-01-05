using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MediatR;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Notebooks.GetNotebook
{
    [AuthorizedRequest]
    public class GetNotebookRequest : IRequest<Notebook>
    {
        public string NotebookId { get; set; }
    }

    public class GetNotebookRequestValidator : AbstractValidator<GetNotebookRequest>
    {
        public GetNotebookRequestValidator()
        {
            this.RuleFor(f => f.NotebookId).NotEmpty();
        }
    }
}
