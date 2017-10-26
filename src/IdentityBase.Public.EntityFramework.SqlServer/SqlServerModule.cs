namespace IdentityBase.Public.EntityFramework.SqlServer
{
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class SqlServerModule : IModule
    {
        public void ConfigureServices(
            IServiceCollection services,
            IConfiguration config)
        {
            services.AddEntityFrameworkStores((options) =>
            {
                string migrationsAssembly =
                    typeof(SqlServerModule)
                    .GetTypeInfo().Assembly.GetName().Name;

                options.DbContextOptions = (dbBuilder) =>
                {
                    dbBuilder.UseSqlServer(
                        config["EntityFramework:SqlServer:ConnectionString"],
                        o => o.MigrationsAssembly(migrationsAssembly)
                    );
                };

                config.GetSection("EntityFramework").Bind(options);
            });
        }

        public void Configure(IApplicationBuilder app)
        {

        }
    }
}