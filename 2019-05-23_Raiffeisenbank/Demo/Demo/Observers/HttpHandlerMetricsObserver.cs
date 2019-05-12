using System.Diagnostics;
using System.Net.Http;

using Microsoft.Extensions.DiagnosticAdapter;

using Prometheus.Client;

namespace Demo.Observers
{
    public sealed class HttpHandlerMetricsObserver : DiagnosticObserverBase
    {
        private static readonly Counter ExternalCallCount = Metrics.CreateCounter(
            name: "demo_external_call_count",
            help: "",
            labelNames: new[] {"host", "path", "status_code"});

        private static readonly Histogram ExternalCallDurationSeconds = Metrics.CreateHistogram(
            name: "demo_external_call_duration_seconds",
            help: "",
            labelNames: new[] {"host", "path", "status_code"});

        protected override bool IsMatch(string name)
        {
            return name == "HttpHandlerDiagnosticListener";
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut")]
        public void OnHttpRequestOut()
        { }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public void OnHttpRequestOutStart(HttpRequestMessage request)
        {
            request.Properties["HttpHandlerMetricsObserver.Stopwatch"] = Stopwatch.StartNew();
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public void OnHttpRequestOutStop(HttpRequestMessage request, HttpResponseMessage response)
        {
            var stopwatch = (Stopwatch) request.Properties["HttpHandlerMetricsObserver.Stopwatch"];
            if (stopwatch != null)
            {
                var requestUri = request.RequestUri;

                ExternalCallCount
                    .WithLabels(requestUri.Host, requestUri.LocalPath, response.StatusCode.ToString())
                    .Inc();

                ExternalCallDurationSeconds
                    .WithLabels(requestUri.Host, requestUri.LocalPath, response.StatusCode.ToString())
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
