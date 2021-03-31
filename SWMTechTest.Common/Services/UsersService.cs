using Microsoft.Extensions.Logging;
using SWMTechTest.Common.Data.Models;
using SWMTechTest.Common.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWMTechTest.Common.Services
{
    public interface IUsersService
    {
        Task<User> GetUserById(int id);

        Task<IEnumerable<User>> GetUsersByAge(int age);

        Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize);

        Task AddOrUpdateUser(User user);

        Task DeleteUserById(int id);
    }

    public class UsersService : IUsersService
    {
        private IUsersRepository _usersRepository;
        private ILogger<UsersService> _logger;

        public UsersService(IUsersRepository usersRepository, ILogger<UsersService> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task AddOrUpdateUser(User user)
        {
            //RDS charges for writes, only update a user if they are new or have changed
            var existingPerson = await _usersRepository.GetUserById(user.Id).ConfigureAwait(false);
            if (existingPerson == null)
            {
                _logger.LogInformation($"Adding user. Id:'{user.Id}'");
                await _usersRepository.UpsertUser(user).ConfigureAwait(false);
            }
            else if (!existingPerson.Equals(user))
            {
                _logger.LogInformation($"Updating user. Id:'{user.Id}'");
                await _usersRepository.UpsertUser(user).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"Not modifying user. Id:'{user.Id}'");
            }
        }

        public async Task DeleteUserById(int id)
        {
            _logger.LogInformation($"Deleting user. Id:'{id}'");
            await _usersRepository.DeleteUserById(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize)
        {
            return await _usersRepository.GetAllUsers(pageNumber, pageSize).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetUsersByAge(int age)
        {
            return await _usersRepository.GetUsersByAge(age).ConfigureAwait(false);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _usersRepository.GetUserById(id).ConfigureAwait(false);
        }
    }
}