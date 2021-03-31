using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SWMTechTest.ConsoleApp.Clients;
using SWMTechTest.ConsoleApp.Settings;
using System;
using System.Threading.Tasks;

namespace SWMTechTest.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            await RunApp(host.Services).ConfigureAwait(false);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
               {
                   builder.AddJsonFile("appsettings.json", optional: false);
               })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<UsersApiSettings>().Bind(hostContext.Configuration.GetSection("UsersApiSettings"));
                    services.AddHttpClient<IUserServiceClient, UserServiceClient>();
                    services.AddTransient<TechTestAnswers>();
                });

        private static async Task RunApp(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var answerGenerator = provider.GetRequiredService<TechTestAnswers>();
            await answerGenerator.ProduceAnswers().ConfigureAwait(false);
        }
    }
}