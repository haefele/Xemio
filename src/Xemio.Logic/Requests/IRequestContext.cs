using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Logic.Requests
{
    public interface IRequestContext : IDisposable
    {
        AuthToken CurrentUser { get; set; }

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default);
        Task Publish<TNotification>(TNotification notification, CancellationToken token = default) where TNotification : INotification;

        Task CommitAsync(CancellationToken token = default);
    }
}