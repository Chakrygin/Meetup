using System.Diagnostics;

using Demo.Observers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Prometheus.Client.AspNetCore;

namespace Demo
{
    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            DiagnosticListener.AllListeners.Subscribe(new AspNetCoreMetricsObserver());
            DiagnosticListener.AllListeners.Subscribe(new HttpHandlerMetricsObserver());
            DiagnosticListener.AllListeners.Subscribe(new SqlClientMetricsObserver());

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePrometheusServer();
            app.UseMvc();
        }
    }
}
