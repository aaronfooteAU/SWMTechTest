using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SWMTechTest.ConsoleApp.Clients.Model;
using SWMTechTest.ConsoleApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SWMTechTest.ConsoleApp.Clients
{
    public interface IUserServiceClient
    {
        Task<IEnumerable<UserDto>> GetAllUsers();

        Task<IEnumerable<UserDto>> GetUsersByAge(int age);

        Task<UserDto> GetUserById(int id);
    }

    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly UsersApiSettings _settings;

        public UserServiceClient(HttpClient httpClient, IOptions<UsersApiSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseAddress);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var people = new List<UserDto>();

            int pageNumer = 1;
            int pageSize = 20;
            while (true)
            {
                var endpoint = $"{_settings.GetAllUsersEndpoint}?pageNumber={pageNumer}&pageSize={pageSize}";
                var result = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);

                result.EnsureSuccessStatusCode();

                var responseMessage = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                var persons = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(responseMessage);

                if (!persons.Any())
                    break;

                people.AddRange(persons);

                pageNumer++;
            }

            return people;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByAge(int age)
        {
            var endpoint = $"{_settings.GetUsersByAgeEndpoint}?age={age}";
            var result = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            var responseMessage = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var people = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(responseMessage);
            return people;
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var endpoint = $"{_settings.GetUserByIdEndpoint}{id}";
            var result = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);

            if (result.StatusCode == HttpStatusCode.NotFound)
                return null;

            result.EnsureSuccessStatusCode();

            var responseMessage = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var person = JsonConvert.DeserializeObject<UserDto>(responseMessage);
            return person;
        }
    }
}