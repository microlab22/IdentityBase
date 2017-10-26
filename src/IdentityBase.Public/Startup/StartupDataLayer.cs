namespace IdentityBase.Public
{
    using System;
    using IdentityBase.Services;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class StartupDataLayer
    {
        public static void ValidateDataLayerServices(
            this IServiceCollection services,
            ILogger logger)
        {
            if (services.IsAdded<IStoreInitializer>())
            {
                logger.LogInformation("IStoreInitializer registered.");
            }

            if (!services.IsAdded<IClientStore>())
            {
                throw new Exception("IClientStore not registered.");
            }

            if (!services.IsAdded<IResourceStore>())
            {
                throw new Exception("IResourceStore not registered.");
            }

            if (!services.IsAdded<ICorsPolicyService>())
            {
                throw new Exception("ICorsPolicyService not registered.");
            }

            if (!services.IsAdded<IPersistedGrantStore>())
            {
                throw new Exception("IPersistedGrantStore not registered.");
            }

            if (!services.IsAdded<IUserAccountStore>())
            {
                throw new Exception("IUserAccountStore not registered.");
            }
        }
    }
}
