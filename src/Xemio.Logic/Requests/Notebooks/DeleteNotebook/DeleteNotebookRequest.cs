using FluentValidation;
using MediatR;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Requests.Notebooks.DeleteNotebook
{
    [AuthorizedRequest]
    public class DeleteNotebookRequest : IRequest
    {
        public string NotebookId { get; set; }
    }

    public class DeleteNotebookRequestValidator : AbstractValidator<DeleteNotebookRequest>
    {
        public DeleteNotebookRequestValidator()
        {
            this.RuleFor(f => f.NotebookId).NotEmpty().NoSurroundingWhitespace();
        }
    }
}