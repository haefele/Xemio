using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Xemio.Logic.Entities;

namespace Xemio.Logic.Requests
{
    public interface IRequestContext : IDisposable
    {
        User CurrentUser { get; set; }

        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default(CancellationToken));
        Task Publish<TNotification>(TNotification notification, CancellationToken token = default(CancellationToken)) where TNotification : INotification;

        Task CommitAsync(CancellationToken token = default(CancellationToken));
    }
}