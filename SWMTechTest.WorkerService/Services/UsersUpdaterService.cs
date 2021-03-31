using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SWMTechTest.Common.Services;
using SWMTechTest.WorkerService.Clients.PeopleClient;
using SWMTechTest.WorkerService.Clients.PeopleClient.Models;
using SWMTechTest.WorkerService.Extensions;
using SWMTechTest.WorkerService.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SWMTechTest.WorkerService.Services
{
    public interface IUsersUpdaterService
    {
        Task UpdateUsers(CancellationToken cancellationToken);
    }

    public class UsersUpdaterService : IUsersUpdaterService
    {
        private readonly IRemoteUsersClient _remoteUsersClient;
        private readonly IUsersService _usersService;
        private readonly ILogger<UsersUpdaterService> _logger;
        private readonly RemoteUsersApiSettings _settings;

        public UsersUpdaterService(IRemoteUsersClient remoteUsersClient, IUsersService usersService, ILogger<UsersUpdaterService> logger, IOptions<RemoteUsersApiSettings> settings)
        {
            _remoteUsersClient = remoteUsersClient;
            _usersService = usersService;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task UpdateUsers(CancellationToken cancellationToken)
        {
            try
            {
                var remoteUsers = await _remoteUsersClient.GetUsers(cancellationToken).ConfigureAwait(false);
                await ProcessRemoteUsers(remoteUsers, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating People Message: '{e.Message}'", e);
            }
        }

        private async Task ProcessRemoteUsers(IEnumerable<RemoteUser> remoteUsers, CancellationToken cancellationToken)
        {
            var listOfIds = await AddOrUpdateRemoteUsers(remoteUsers, cancellationToken).ConfigureAwait(false);
            await DeleteRemoteUsersNolongerInTheRemoteData(listOfIds, cancellationToken).ConfigureAwait(false);
        }

        private async Task<ICollection<int>> AddOrUpdateRemoteUsers(IEnumerable<RemoteUser> users, CancellationToken cancellationToken)
        {
            var idCollection = new HashSet<int>();
            foreach (var remoteUser in users)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (remoteUser.IsValid())
                {
                    var user = remoteUser.ToUser();
                    await _usersService.AddOrUpdateUser(user).ConfigureAwait(false);
                    idCollection.Add(user.Id);
                }
                else
                {
                    _logger.LogInformation($"RemoteUser is invalid Id:'{remoteUser.Id}'");
                }
            }

            return idCollection;
        }

        private async Task DeleteRemoteUsersNolongerInTheRemoteData(ICollection<int> ids, CancellationToken cancellationToken)
        {
            var pageNumber = 1;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var users = await _usersService.GetAllUsers(pageNumber, _settings.DefaultPageSize).ConfigureAwait(false);

                if (!users.Any())
                    break;

                foreach (var user in users)
                {
                    if (!ids.Contains(user.Id))
                    {
                        await _usersService.DeleteUserById(user.Id).ConfigureAwait(false);
                    }
                }

                pageNumber++;
            }
        }
    }
}