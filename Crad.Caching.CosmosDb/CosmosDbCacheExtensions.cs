using Crad.Caching.CosmosDb;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosmosDbCacheExtensions
    {
        public static IServiceCollection AddDistributedCosmosDbCache(this IServiceCollection services, Action<CosmosDbCacheOptions> options)
        {
            OptionsServiceCollectionExtensions.AddOptions(services);
            OptionsServiceCollectionExtensions.Configure<CosmosDbCacheOptions>(services, options);

            services.AddSingleton<IDistributedCache, CosmosDbCache>();

            return services;
        }
    }
}
