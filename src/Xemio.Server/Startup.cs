﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xemio.Logic;
using Xemio.Server.AspNetCore;

namespace Xemio.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(f =>
                {
                    f.Filters.Add<XemioExceptionFilterAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddXemioFramework(this._configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseXemioAuthorization();
            app.UseMvc();
        }
    }
}
