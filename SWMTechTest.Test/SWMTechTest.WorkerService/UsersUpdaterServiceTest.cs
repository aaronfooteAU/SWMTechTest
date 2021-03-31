using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SWMTechTest.Common.Data.Enums;
using SWMTechTest.Common.Data.Models;
using SWMTechTest.Common.Services;
using SWMTechTest.WorkerService.Clients.PeopleClient;
using SWMTechTest.WorkerService.Clients.PeopleClient.Models;
using SWMTechTest.WorkerService.Extensions;
using SWMTechTest.WorkerService.Services;
using SWMTechTest.WorkerService.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SWMTechTest.Test.SWMTechTest.WorkerService
{
    public class UsersUpdaterServiceTest
    {
        private Mock<IRemoteUsersClient> _remoteUsersClient;
        private Mock<IUsersService> _usersService;
        private Mock<ILogger<UsersUpdaterService>> _logger;

        public UsersUpdaterService CreateSut()
        {
            _remoteUsersClient = new Mock<IRemoteUsersClient>();
            _usersService = new Mock<IUsersService>();
            _logger = new Mock<ILogger<UsersUpdaterService>>();

            return new UsersUpdaterService(_remoteUsersClient.Object, _usersService.Object, _logger.Object, Options.Create(new RemoteUsersApiSettings()));
        }

        private IEnumerable<RemoteUser> ConfigureRemoteUsersClient(int numOfUsers, bool includeInvalid = false)
        {
            var remoteUsers = new List<RemoteUser>();

            for (var x = 1; x <= numOfUsers; x++)
                remoteUsers.Add(new RemoteUser { Id = x, First = "F" + x, Last = "L" + x, Age = x, Gender = "M" });

            if (includeInvalid)
                remoteUsers.Add(new RemoteUser { Id = remoteUsers.Count() + 1 });

            _remoteUsersClient.Setup(x => x.GetUsers(It.IsAny<CancellationToken>())).ReturnsAsync(remoteUsers);

            return remoteUsers;
        }

        private IEnumerable<User> ConfigureUsersRepository(int numOfUsers)
        {
            var users = new List<User>();

            for (var x = 1; x <= numOfUsers; x++)
                users.Add(new User { Id = x, FirstName = "F" + x, LastName = "L" + x, Age = x, Gender = Gender.Male });

            _usersService.Setup(x => x.GetAllUsers(1, It.IsAny<int>())).ReturnsAsync(users);

            return users;
        }

        private void VerifyUsersServiceUpdateCalled(IEnumerable<int> ids)
        {
            foreach (var id in ids)
                _usersService.Verify(s => s.AddOrUpdateUser(It.Is<User>(u => u.Id == id)));

            _usersService.Verify(s => s.AddOrUpdateUser(It.IsAny<User>()), Times.Exactly(ids.Count()));
        }

        private void VerifyUsersServiceDeleteNotCalled()
        {
            _usersService.Verify(s => s.DeleteUserById(It.IsAny<int>()), Times.Never);
        }

        private void VerifyUsersServiceDeleteCalled(IEnumerable<int> ids)
        {
            foreach (var id in ids)
                _usersService.Verify(s => s.DeleteUserById(id));

            _usersService.Verify(s => s.DeleteUserById(It.IsAny<int>()), Times.Exactly(ids.Count()));
        }

        [Fact]
        public async Task UsersUpdaterService_When_NewUsers_Should_CallUpadte()
        {
            //arrange
            var sut = CreateSut();
            var remoteUsers = ConfigureRemoteUsersClient(2);

            //act
            await sut.UpdateUsers(new CancellationToken()).ConfigureAwait(false);

            //assert
            VerifyUsersServiceUpdateCalled(remoteUsers.Select(x => (int)x.Id));
            VerifyUsersServiceDeleteNotCalled();
        }

        [Fact]
        public async Task UsersUpdaterService_When_UserInvalid_ShouldNot_CallUpadte()
        {
            //arrange
            var sut = CreateSut();
            var remoteUsers = ConfigureRemoteUsersClient(2, includeInvalid: true);

            //act
            await sut.UpdateUsers(new CancellationToken()).ConfigureAwait(false);

            //assert
            VerifyUsersServiceUpdateCalled(remoteUsers.Where(x => x.IsValid()).Select(x => (int)x.Id));
            VerifyUsersServiceDeleteNotCalled();
        }

        [Fact]
        public async Task UsersUpdaterService_When_UserInvalidAndExistsInDb_Should_CallDelete()
        {
            //arrange
            var sut = CreateSut();
            var remoteUsers = ConfigureRemoteUsersClient(2, includeInvalid: true);
            ConfigureUsersRepository(3);

            //act
            await sut.UpdateUsers(new CancellationToken()).ConfigureAwait(false);

            //assert
            VerifyUsersServiceUpdateCalled(remoteUsers.Where(r => r.IsValid()).Select(r => (int)r.Id));
            VerifyUsersServiceDeleteCalled(remoteUsers.Where(r => !r.IsValid()).Select(r => (int)r.Id));
        }
    }
}