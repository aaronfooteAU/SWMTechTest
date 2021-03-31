using FluentScheduler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SWMTechTest.WorkerService.Services;
using SWMTechTest.WorkerService.Settings;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SWMTechTest.WorkerService
{
    [ExcludeFromCodeCoverage]
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IUsersUpdaterService _usersUpdaterService;
        private readonly CancellationTokenSource _cts;
        private readonly WorkerServiceSettings _settings;

        public Worker(ILogger<Worker> logger, IUsersUpdaterService usersUpdaterService, IOptions<WorkerServiceSettings> settings)
        {
            _logger = logger;
            _usersUpdaterService = usersUpdaterService;
            _cts = new CancellationTokenSource();
            _settings = settings.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            JobManager.Initialize();

            JobManager.AddJob(
                            () => ExecuteJob(),
                            schedule => schedule.NonReentrant().ToRunNow().AndEvery(_settings.TimeBetweenRunsSeconds).Seconds());

            JobManager.Start();

            return Task.CompletedTask;
        }

        private void ExecuteJob()
        {
            _logger.LogInformation("Start: Updating Users");
            try
            {
                //FluentScheduler doesn't support async. It's necessary to block
                //to ensure that NonReentrant() works as expected
                _usersUpdaterService.UpdateUsers(_cts.Token).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception updating users: Message: '{e.Message}'", e);
            }
            _logger.LogInformation("End: Updating users");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cts.Cancel();
            }
            finally
            {
                JobManager.Stop();
            }

            return Task.CompletedTask;
        }
    }
}