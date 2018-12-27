using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Xemio.Logic.Requests
{
    public class RequestValidationPreProcessor<T> : IRequestPreProcessor<T>
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestValidationPreProcessor(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public Task Process(T request, CancellationToken cancellationToken)
        {
            var validator = this._serviceProvider.GetService<IValidator<T>>();
            if (validator != null)
            {
                var result = validator.Validate(request);
                if (result.IsValid == false)
                {
                    throw new RequestValidationFailedException(result.Errors);
                }
            }

            return Task.CompletedTask;
        }
    }
}