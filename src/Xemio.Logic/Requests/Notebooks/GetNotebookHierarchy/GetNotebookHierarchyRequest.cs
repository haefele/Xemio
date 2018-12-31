using MediatR;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy
{
    [AuthorizedRequest]
    public class GetNotebookHierarchyRequest : IRequest<NotebookHierarchy>
    {
    }
}
