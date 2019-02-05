using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Endpoints.Notebooks;
using Xemio.Client.Data.Entities;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Notebooks.CreateNotebook;
using Xemio.Logic.Requests.Notebooks.DeleteNotebook;
using Xemio.Logic.Requests.Notebooks.GetNotebook;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;
using Xemio.Logic.Requests.Notebooks.UpdateNotebook;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Server.Endpoints
{
    [ApiController]
    [Route("[controller]")]
    public class NotebooksController : ControllerBase
    {
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;
        private readonly IEntityIdManager _entityIdManager;

        public NotebooksController(IRequestContext requestContext, IMapper mapper, IEntityIdManager entityIdManager)
        {
            this._requestContext = requestContext;
            this._mapper = mapper;
            this._entityIdManager = entityIdManager;
        }

        [HttpPost]
        public async Task<ActionResult<NotebookDTO>> CreateNotebook(CreateNotebookAction action, CancellationToken token)
        {
            var request = new CreateNotebookRequest
            {
                Name = action.Name,
                ParentNotebookId = this._entityIdManager.TryAddCollectionName<Notebook>(action.ParentNotebookId)
            };

            var notebook = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            var result = this._mapper.Map<Notebook, NotebookDTO>(notebook);
            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        [HttpGet("/{notebookId}")]
        public async Task<ActionResult<NotebookDTO>> GetNotebook(string notebookId, CancellationToken token)
        {
            var request = new GetNotebookRequest
            {
                NotebookId = this._entityIdManager.AddCollectionName<Notebook>(notebookId)
            };

            var notebook = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            var result = this._mapper.Map<Notebook, NotebookDTO>(notebook);
            return this.Ok(result);
        }

        [HttpGet("Hierarchy")]
        public async Task<ActionResult<NotebookHierarchyDTO>> GetNotebookHierarchy(CancellationToken token)
        {
            var request = new GetNotebookHierarchyRequest();

            var notebookHierarchy = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            var result = this._mapper.Map<NotebookHierarchy, NotebookHierarchyDTO>(notebookHierarchy);
            return this.Ok(result);
        }

        [HttpPatch("/{notebookId}")]
        public async Task<ActionResult<NotebookDTO>> UpdateNotebook(string notebookId, UpdateNotebookAction action, CancellationToken token)
        {
            var request = new UpdateNotebookRequest
            {
                NotebookId = this._entityIdManager.AddCollectionName<Notebook>(notebookId),

                UpdateName = action.UpdateName,
                Name = action.Name,

                UpdateParentNotebookId = action.UpdateParentNotebookId,
                ParentNotebookId = this._entityIdManager.TryAddCollectionName<Notebook>(action.ParentNotebookId),
            };

            var notebook = await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            var result = this._mapper.Map<Notebook, NotebookDTO>(notebook);
            return this.Ok(result);
        }

        [HttpDelete("/{notebookId}")]
        public async Task<ActionResult> DeleteNotebook(string notebookId, CancellationToken token)
        {
            var request = new DeleteNotebookRequest
            {
                NotebookId = this._entityIdManager.AddCollectionName<Notebook>(notebookId),
            };

            await this._requestContext.Send(request, token);

            await this._requestContext.CommitAsync(token);

            return this.Ok();
        }
    }
}