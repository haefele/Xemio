using Xemio.Logic.Requests;

namespace Xemio.Logic.Services.Requests
{
    public interface IRequestManager
    {
        IRequestContext StartRequestContext();
    }
}