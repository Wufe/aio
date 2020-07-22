using Aio.Persistence.Database.Mongo.Configuration;
using Aio.Persistence.Database.Mongo.Configuration.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aio.Web.Configuration {
    public static class DependencyInjectionExtensions {
        
        public static void AddDatabaseSettings(this IServiceCollection services, IConfiguration configuration) {
            services
                .Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.FIELD_NAME));
            
            services
                .AddSingleton<IDatabaseSettings>(serviceProvider =>
                    serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        }

    }
}