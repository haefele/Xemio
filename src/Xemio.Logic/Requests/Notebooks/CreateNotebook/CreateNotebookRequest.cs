using System.Linq;
using FluentValidation;
using MediatR;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Notebooks.CreateNotebook
{
    [AuthorizedRequest]
    public class CreateNotebookRequest : IRequest<Notebook>
    {
        public string Name { get; set; }
        public string ParentNotebookId { get; set; }
    }

    public class CreateNotebookRequestValidator : AbstractValidator<CreateNotebookRequest>
    {
        public CreateNotebookRequestValidator()
        {
            this.RuleFor(f => f.Name).NotEmpty().MinimumLength(3).NoSurroundingWhitespace();
            this.RuleFor(f => f.ParentNotebookId).NoSurroundingWhitespace();
        }
    }
}
