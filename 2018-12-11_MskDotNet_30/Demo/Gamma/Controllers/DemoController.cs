using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using OpenTracing;

namespace Gamma.Controllers
{
    public class DemoController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITracer _tracer;

        public DemoController(IHostingEnvironment hostingEnvironment, ITracer tracer)
        {
            _hostingEnvironment = hostingEnvironment;
            _tracer = tracer;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var html = "<a href=\"/demo\">/demo</a><br/>";
            return Content(html, MediaTypeNames.Text.Html);
        }

        [HttpGet("demo")]
        public async Task<IActionResult> Demo()
        {
            var sb = new StringBuilder()
                .AppendLine(new string('=', _hostingEnvironment.ApplicationName.Length))
                .AppendLine(_hostingEnvironment.ApplicationName)
                .AppendLine();

            sb.AppendLine("HEADERS IN:");
            foreach (var (name, value) in HttpContext.Request.Headers)
            {
                sb.AppendLine($"\t{name}: {value}");
            }

            sb.AppendLine();

            var spanContext = _tracer.ActiveSpan.Context;
            sb.AppendLine("SPAN CONTEXT:");
            sb.AppendLine($"\tTraceId: {spanContext.TraceId}");
            sb.AppendLine($"\tSpanId: {spanContext.SpanId}");
            sb.AppendLine($"\tBaggageItems:");
            foreach (var baggageItem in spanContext.GetBaggageItems())
            {
                sb.AppendLine($"\t\t{baggageItem.Key}: {baggageItem.Value}");
            }

            sb.AppendLine();

            return Ok(sb.ToString());
        }
    }
}
