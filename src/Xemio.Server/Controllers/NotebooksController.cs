using Microsoft.AspNetCore.Mvc;
using Xemio.Logic.Requests;

namespace Xemio.Server.Controllers
{
    [ApiController]
    public class NotebooksController : ControllerBase
    {
        private readonly IRequestContext _requestContext;

        public NotebooksController(IRequestContext requestContext)
        {
            this._requestContext = requestContext;
        }
    }
}