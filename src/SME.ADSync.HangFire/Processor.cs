using Hangfire;
using Microsoft.Extensions.Configuration;
using SME.ADSync.Background;
using SME.ADSync.Core.Interfaces;
using System;
using System.Linq.Expressions;

namespace SME.ADSync.HangFire
{
    public class Processor : IProcessor
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public Processor(IConfiguration configuration, string connectionString)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public bool Registrado { get; private set; }

        public string Executar(Expression<Action> metodo)
        {
            return BackgroundJob.Enqueue(metodo);
        }

        public string Executar<T>(Expression<Action<T>> metodo)
        {
            return BackgroundJob.Enqueue<T>(metodo);
        }

        public void ExecutarPeriodicamente(Expression<Action> metodo, string cron)
        {
            RecurringJob.AddOrUpdate(metodo, cron);
        }

        public void ExecutarPeriodicamente<T>(Expression<Action<T>> metodo, string cron)
        {
            RecurringJob.AddOrUpdate(metodo, cron);
        }

        public void Registrar()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString(connectionString));

            GlobalJobFilters.Filters.Add(new ContextFilterAttribute());
            Registrado = true;
        }
    }
}
