using Jaeger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTracing;

namespace Gamma
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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
