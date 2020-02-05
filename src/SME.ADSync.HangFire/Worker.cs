using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ADSync.Background;
using SME.ADSync.Core.Interfaces;
using SME.ADSync.HangFire.DependencyInjection;
using System;
using System.IO;

namespace SME.ADSync.HangFire
{
    public class Worker : IWorker
    {
        private readonly IConfiguration configuration;
        private readonly IServiceCollection serviceCollection;
        private readonly string connectionString;
        private BackgroundJobServer backgroundJobServer;
        private IWebHost host;

        public Worker(IConfiguration configuration,
                      IServiceCollection serviceCollection,
                      string connectionString)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public void Dispose()
        {
            host?.Dispose();
            backgroundJobServer.Dispose();
        }

        public void Registrar()
        {
            RegistrarHangfireServer();
            RegistrarDashboard();
        }

        private void RegistrarDashboard()
        {
            host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables("ADSync_");
                })
                .UseStartup<Startup>()
                .UseUrls(new[] { "http://*:5001" })
                .Build();

            host.RunAsync();
        }

        private void RegistrarHangfireServer()
        {
            var pollInterval = configuration.GetValue<int>("BackgroundWorkerQueuePollInterval", 5);
            Console.WriteLine($"ADSync Worker Service - BackgroundWorkerQueuePollInterval parameter = {pollInterval}");

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseActivator<HangfireActivator>(new HangfireActivator(serviceCollection.BuildServiceProvider()))
                .UseFilter<AutomaticRetryAttribute>(new AutomaticRetryAttribute() { Attempts = 0 })
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(pollInterval)
                });

            GlobalJobFilters.Filters.Add(new ContextFilterAttribute());

            var workerCount = configuration.GetValue<int>("BackgroundWorkerParallelDegree", 1);
            Console.WriteLine($"ADSync Worker Service - BackgroundWorkerParallelDegree parameter = {workerCount}");

            backgroundJobServer = new BackgroundJobServer(new BackgroundJobServerOptions()
            {
                WorkerCount = workerCount
            });
        }
    }
}
