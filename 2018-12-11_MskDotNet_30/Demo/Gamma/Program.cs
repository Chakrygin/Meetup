﻿using System;

using Jaeger;
using Jaeger.Samplers;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Gamma
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var envs = new[]
            {
                ("ASPNETCORE_URLS", "http://+:5003"),
                ("ASPNETCORE_ENVIRONMENT", "Development"),

                (Configuration.JaegerServiceName, "Gamma"),
                (Configuration.JaegerAgentHost, "localhost"),
                (Configuration.JaegerAgentPort, "5775"),
                (Configuration.JaegerSamplerType, ConstSampler.Type),
                (Configuration.JaegerSamplerParam, "1.0"),
            };

            Array.ForEach(envs, env =>
                Environment.SetEnvironmentVariable(env.Item1, env.Item2));


            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
