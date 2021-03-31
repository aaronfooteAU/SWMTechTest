using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using SWMTechTest.Common.Data.Repositories;
using SWMTechTest.Common.Services;
using System.Diagnostics.CodeAnalysis;

namespace SWMTechTest.Common
{
    [ExcludeFromCodeCoverage]
    public static class DependencyExtension
    {
        public static void ConfigureCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(configuration.GetConnectionString("MySql")));
        }
    }
}