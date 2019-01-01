using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic.Requests;
using Xemio.Logic.Services.JsonWebToken;

namespace Xemio.Server.AspNetCore
{
    public static class XemioAuthorizationMiddleware
    {
        public static void UseXemioAuthorization(this IApplicationBuilder self)
        {
            self.Use(async (context, next) =>
            {
                var authorizationHeader = context.Request.Headers["Authorization"];
                var token = ParseAuthorizationHeader(authorizationHeader);

                var requestContext = context.RequestServices.GetService<IRequestContext>();
                requestContext.CurrentUser = token;

                await next();
            });
        }

        private static AuthToken ParseAuthorizationHeader(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return null;

            var parts = authorizationHeader.Split(' ');

            if (parts.Length != 2)
                return null;

            if (string.Equals(parts[0], "Bearer", StringComparison.OrdinalIgnoreCase) == false)
                return null;

            try
            {
                return new AuthToken(parts[1]);
            }
            catch (InvalidJsonWebTokenException)
            {
                return null;
            }
        }
    }
}
