using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DiagnosticAdapter;

using Prometheus.Client;

namespace Demo.Observers
{
    public sealed class AspNetCoreMetricsObserver : DiagnosticObserverBase
    {
        private static readonly Counter HttpRequestCount = Metrics.CreateCounter(
            name: "demo_http_request_count",
            help: "",
            labelNames: new[] {"action", "status_code"});

        private static readonly Histogram HttpRequestDurationSeconds = Metrics.CreateHistogram(
            name: "demo_http_request_duration_seconds",
            help: "",
            labelNames: new[] {"action", "status_code"});

        protected override bool IsMatch(string name)
        {
            return name == "Microsoft.AspNetCore";
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn")]
        public void OnHttpRequestIn()
        { }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Start")]
        public void OnHttpRequestInStart(HttpContext httpContext)
        {
            httpContext.Items["AspNetCoreMetricsObserver.Stopwatch"] = Stopwatch.StartNew();
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeAction")]
        public void OnBeforeAction(HttpContext httpContext, ActionDescriptor actionDescriptor)
        {
            httpContext.Items["AspNetCoreMetricsObserver.Action"] = actionDescriptor.DisplayName;
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop")]
        public void OnHttpRequestInStop(HttpContext httpContext)
        {
            var stopwatch = (Stopwatch) httpContext.Items["AspNetCoreMetricsObserver.Stopwatch"];
            if (stopwatch != null)
            {
                var action = (string) httpContext.Items["AspNetCoreMetricsObserver.Action"] ?? httpContext.Request.Path;

                HttpRequestCount
                    .WithLabels(action, httpContext.Response.StatusCode.ToString())
                    .Inc();

                HttpRequestDurationSeconds
                    .WithLabels(action, httpContext.Response.StatusCode.ToString())
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
