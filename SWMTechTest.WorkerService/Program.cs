using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SWMTechTest.Common;
using SWMTechTest.WorkerService.Clients.PeopleClient;
using SWMTechTest.WorkerService.Services;
using SWMTechTest.WorkerService.Settings;
using System.Diagnostics.CodeAnalysis;

namespace SWMTechTest.WorkerService
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddOptions<RemoteUsersApiSettings>().Bind(hostContext.Configuration.GetSection("RemoteUsersApiSettings"));
                    services.AddOptions<WorkerServiceSettings>().Bind(hostContext.Configuration.GetSection("WorkerServiceSettings"));
                    services.AddHttpClient<IRemoteUsersClient, RemoteUsersClient>();
                    services.AddTransient<IUsersUpdaterService, UsersUpdaterService>();
                    services.ConfigureCommonServices(hostContext.Configuration);
                });
    }
}