using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MediatR;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Notebooks.UpdateNotebook
{
    [AuthorizedRequest]
    public class UpdateNotebookRequest : IRequest<Notebook>
    {
        public string NotebookId { get; set; }

        public bool UpdateName { get; set; }
        public string Name { get; set; }

        public bool UpdateParentNotebookId { get; set; }
        public string ParentNotebookId { get; set; }
    }

    public class UpdateNotebookRequestValidator : AbstractValidator<UpdateNotebookRequest>
    {
        public UpdateNotebookRequestValidator()
        {
            this.RuleFor(f => f.NotebookId).NotEmpty().NoSurroundingWhitespace();
            this.RuleFor(f => f.Name).NotEmpty().MinimumLength(3).NoSurroundingWhitespace().When(f => f.UpdateName);
            this.RuleFor(f => f.ParentNotebookId).NoSurroundingWhitespace().When(f => f.UpdateParentNotebookId);
        }
    }
}
