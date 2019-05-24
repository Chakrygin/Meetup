using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public sealed class DemoController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var links = new[]
            {
                "/metrics",
                "/sql",
                "/http",
            };

            var html = links
                .Aggregate(new StringBuilder(), (sb, link) => sb
                    .AppendLine($@"<a href=""{link}"">{link}</a><br>"))
                .ToString();

            return Content(html, MediaTypeNames.Text.Html);
        }

        [HttpGet("sql")]
        public async Task<string> SqlMetricsDemo([FromQuery] string name = "SQL metrics")
        {
            var csb = new SqlConnectionStringBuilder();
            csb.DataSource = "localhost";
            csb.InitialCatalog = "master";
            //csb.IntegratedSecurity = true;
            csb.UserID = "sa";
            csb.Password = "Password12!";

            var connectionString = csb.ToString();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.ExecuteScalarAsync<string>(
                    "SELECT 'Hello, ' + @name + '!'", new {name});

                return result;
            }
        }

        [HttpGet("http")]
        public async Task<string> HttpMetricsDemo()
        {
            var requestUri =
                "http://localhost:5000/hello";

            using (var http = new HttpClient())
            using (var response = await http.GetAsync(requestUri))
            {
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        [HttpGet("hello")]
        public IActionResult Hello([FromQuery]string name = "HTTP metrics")
        {
            var message = $"Hello, {name}!";
            return Ok(message);
        }
    }
}
