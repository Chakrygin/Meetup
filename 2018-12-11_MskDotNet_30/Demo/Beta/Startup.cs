using System;
using System.Net.Http;

using Jaeger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTracing;

namespace Beta
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<HttpClient>(serviceProvider =>
            {
                var http = new HttpClient();
                http.BaseAddress = new Uri("http://localhost:5003/");

                return http;
            });

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                var loggerFactory =
                    serviceProvider.GetRequiredService<ILoggerFactory>();

                var configuration = Configuration.FromEnv(loggerFactory);
                return configuration.GetTracer();
            });

            services.AddOpenTracing();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
