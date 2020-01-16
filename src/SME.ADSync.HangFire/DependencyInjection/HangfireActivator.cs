using Hangfire;
using System;

namespace SME.ADSync.HangFire.DependencyInjection
{
    public class HangfireActivator : JobActivator
    {
        private readonly IServiceProvider serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override object ActivateJob(Type jobType)
        {
            return serviceProvider.GetService(jobType);
        }
    }
}
