using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SWMTechTest.WorkerService.Clients.PeopleClient.Models;
using SWMTechTest.WorkerService.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SWMTechTest.WorkerService.Clients.PeopleClient
{
    public interface IRemoteUsersClient
    {
        Task<IEnumerable<RemoteUser>> GetUsers(CancellationToken cancellationToken);
    }

    [ExcludeFromCodeCoverage]
    public class RemoteUsersClient : IRemoteUsersClient
    {
        private readonly HttpClient _httpClient;
        private readonly RemoteUsersApiSettings _settings;
        private readonly ILogger<RemoteUsersClient> _logger;

        public RemoteUsersClient(HttpClient httpClient, IOptions<RemoteUsersApiSettings> settings, ILogger<RemoteUsersClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BaseAddress);
            _logger = logger;
        }

        public async Task<IEnumerable<RemoteUser>> GetUsers(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(_settings.GetUsersEndpoint, cancellationToken).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                try
                {
                    using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var sr = new StreamReader(stream))
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();

                        var obj = serializer.Deserialize<IEnumerable<RemoteUser>>(reader);

                        return obj;
                    }
                }
                catch (Exception e)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError($"Failed to Deserialize Json strem\nMessage:'{e.Message}'\nJson:'{jsonContent}'");
                    throw;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Error retreiving Users json StatusCode:'{e.StatusCode}', message:'{e.Message}'", e);
                throw;
            }
        }
    }
}