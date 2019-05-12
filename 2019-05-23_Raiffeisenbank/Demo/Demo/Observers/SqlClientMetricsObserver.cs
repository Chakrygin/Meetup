using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

using Microsoft.Extensions.DiagnosticAdapter;

using Prometheus.Client;

namespace Demo.Observers
{
    public sealed class SqlClientMetricsObserver : DiagnosticObserverBase
    {
        public static Gauge SqlConnectionCount { get; } =
            Metrics.CreateGauge(
                name: "demo_sql_connection_count",
                help: "",
                labelNames: new[] {"data_source", "database"});

        public static Counter SqlCommandCount { get; } =
            Metrics.CreateCounter(
                name: "demo_sql_command_count",
                help: "",
                labelNames: new[] {"data_source", "database", "query"});

        public static Histogram SqlCommandDurationSeconds { get; } =
            Metrics.CreateHistogram(
                name: "demo_sql_command_duration_seconds",
                help: "",
                labelNames: new[] {"data_source", "database", "query"});

        private readonly AsyncLocal<Stopwatch> _asyncLocalStopwatch = new AsyncLocal<Stopwatch>();

        protected override bool IsMatch(string name)
        {
            return name == "SqlClientDiagnosticListener";
        }

        [DiagnosticName("System.Data.SqlClient.WriteConnectionOpenBefore")]
        public void OnConnectionOpenBefore()
        { }

        [DiagnosticName("System.Data.SqlClient.WriteConnectionOpenAfter")]
        public void OnConnectionOpenAfter(SqlConnection connection)
        {
            SqlConnectionCount
                .WithLabels(connection.DataSource, connection.Database)
                .Inc();
        }

        [DiagnosticName("System.Data.SqlClient.WriteConnectionCloseBefore")]
        public void OnConnectionCloseBefore(SqlConnection connection)
        {
            SqlConnectionCount
                .WithLabels(connection.DataSource, connection.Database)
                .Dec();
        }

        [DiagnosticName("System.Data.SqlClient.WriteConnectionCloseAfter")]
        public void OnConnectionCloseAfter()
        { }

        [DiagnosticName("System.Data.SqlClient.WriteCommandBefore")]
        public void OnCommandBefore(SqlCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            _asyncLocalStopwatch.Value = stopwatch;

            var connection = command.Connection;

            SqlCommandCount
                .WithLabels(connection.DataSource, connection.Database, command.CommandText)
                .Inc();
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
        public void OnCommandAfter(SqlCommand command)
        {
            var stopwatch = _asyncLocalStopwatch.Value;
            if (stopwatch != null)
            {
                var connection = command.Connection;

                SqlCommandDurationSeconds
                    .WithLabels(connection.DataSource, connection.Database, command.CommandText)
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
