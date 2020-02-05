using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ADSync.Infra.Escopo;
using System;

namespace SME.ADSync.IoC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void TryAddScopedWorkerService<TService, TImplementation>(this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(TService)) || typeof(IDisposable).IsAssignableFrom(typeof(TImplementation)))
            {
                collection.TryAddTransient<TImplementation>();
                collection.TryAddTransient(typeof(TService), x => WorkerServiceScope.AddTransientDisposableServices((IDisposable)x.GetService(typeof(TImplementation))));
            }
            else
                collection.TryAddTransient<TService, TImplementation>();
        }

        public static void TryAddScopedWorkerService<TImplementation>(this IServiceCollection collection, Func<IServiceProvider, TImplementation> implementationFactory)
            where TImplementation : class
        {
            collection.TryAddTransient<TImplementation>(implementationFactory);
        }
    }
}
