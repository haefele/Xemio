using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Xemio.Client.Data.Entities;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;

namespace Xemio.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotebooksController : ControllerBase
    {
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public NotebooksController(IRequestContext requestContext, IMapper mapper)
        {
            this._requestContext = requestContext;
            this._mapper = mapper;
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
    }
}