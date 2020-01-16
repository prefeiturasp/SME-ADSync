using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace SME.ADSync.HangFire.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
