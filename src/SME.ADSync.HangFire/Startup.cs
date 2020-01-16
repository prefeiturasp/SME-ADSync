using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ADSync.HangFire.Filters;
using System;

namespace SME.ADSync.HangFire
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireDashboard("/worker", new DashboardOptions()
            {
                IsReadOnlyFunc = (DashboardContext context) => !env.IsDevelopment(),
                Authorization = new[] { new DashboardAuthorizationFilter() },
                StatsPollingInterval = 10000 // atualiza a cada 10s
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(c => c
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseFilter<AutomaticRetryAttribute>(new AutomaticRetryAttribute() { Attempts = 0 })
                .UseSqlServerStorage(configuration.GetConnectionString("ADSync-HangFire-SqlServer")));
        }
    }

}
