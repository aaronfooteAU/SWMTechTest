using Microsoft.Extensions.Logging;
using Moq;
using SWMTechTest.Common.Data.Enums;
using SWMTechTest.Common.Data.Models;
using SWMTechTest.Common.Data.Repositories;
using SWMTechTest.Common.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SWMTechTest.Test.SWMTechTest.Common
{
    public class UsersServiceTest
    {
        private Mock<IUsersRepository> _usersRepository;
        private Mock<ILogger<UsersService>> _logger;

        private User _user1;
        private User _user2;
        private User _user2Clone;
        private User _user3;

        private UsersService CreatSut()
        {
            CreateUsers();

            _usersRepository = new Mock<IUsersRepository>();
            _logger = new Mock<ILogger<UsersService>>();

            return new UsersService(_usersRepository.Object, _logger.Object);
        }

        private void CreateUsers()
        {
            _user1 = new User
            {
                Id = 1,
                FirstName = "F1",
                LastName = "L1",
                Age = 1,
                Gender = Gender.Male
            };

            _user2 = new User
            {
                Id = 2,
                FirstName = "F2",
                LastName = "L2",
                Age = 2,
                Gender = Gender.Female
            };

            _user2Clone = new User
            {
                Id = 2,
                FirstName = "F2",
                LastName = "L2",
                Age = 2,
                Gender = Gender.Female
            };

            _user3 = new User
            {
                Id = 2,
                FirstName = "F2",
                LastName = "L2",
                Age = 3,
                Gender = Gender.Female
            };
        }

        private void VerifyRepository(User upsertValue, int? deletedId)
        {
            if (upsertValue != null)
                _usersRepository.Verify(x => x.UpsertUser(upsertValue), Times.Once);
            else
                _usersRepository.Verify(x => x.UpsertUser(It.IsAny<User>()), Times.Never);

            if (deletedId.HasValue)
                _usersRepository.Verify(x => x.DeleteUserById(deletedId.Value), Times.Once);
            else
                _usersRepository.Verify(x => x.DeleteUserById(It.IsAny<int>()), Times.Never);
        }

        private void VerifyLogger(string messageBeginsWith)
        {
            //_logger.LogInformation() is an extension method and cannot be moq'd
            // as .LogInformation calls the ILogger.Log method, which is moq'd
            //We can use the .Log method to verify
            _logger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith(messageBeginsWith)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateUser_WhenUserIsNew_ShouldAddUser()
        {
            //arrange
            var sut = CreatSut();
            _usersRepository.Setup(x => x.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            //act
            await sut.AddOrUpdateUser(_user1).ConfigureAwait(false);

            //assert
            VerifyRepository(_user1, null);
            VerifyLogger("Adding user.");
        }

        [Fact]
        public async Task AddOrUpdateUser_WhenUserIsUnchanged_ShouldNotUpdateUser()
        {
            //arrange
            var sut = CreatSut();
            _usersRepository.Setup(x => x.GetUserById(It.IsAny<int>())).ReturnsAsync(_user2Clone);

            //act
            await sut.AddOrUpdateUser(_user2).ConfigureAwait(false);

            //assert
            VerifyRepository(null, null);
            VerifyLogger("Not modifying user.");
        }

        [Fact]
        public async Task AddOrUpdateUser_WhenUserExisisAndIsChanged_ShouldUpdateUser()
        {
            //arrange
            var sut = CreatSut();
            _usersRepository.Setup(x => x.GetUserById(It.IsAny<int>())).ReturnsAsync(_user3);

            //act
            await sut.AddOrUpdateUser(_user2).ConfigureAwait(false);

            //assert
            VerifyRepository(_user2, null);
            VerifyLogger("Updating user.");
        }

        [Fact]
        public async Task DeleteUserById_CallsTheRepoistory()
        {
            //arrange
            var sut = CreatSut();

            //act
            await sut.DeleteUserById(2).ConfigureAwait(false);

            //assert
            VerifyRepository(null, 2);
            VerifyLogger("Deleting user.");
        }

        [Fact]
        public async Task GetAllUsers_CallsTheRepoistory()
        {
            //arrange
            var sut = CreatSut();

            //act
            await sut.GetAllUsers(3, 4).ConfigureAwait(false);

            //assert
            _usersRepository.Verify(x => x.GetAllUsers(3, 4), Times.Once);
        }

        [Fact]
        public async Task GetUsersByAge_CallsTheRepoistory()
        {
            //arrange
            var sut = CreatSut();

            //act
            await sut.GetUsersByAge(5).ConfigureAwait(false);

            //assert
            _usersRepository.Verify(x => x.GetUsersByAge(5), Times.Once);
        }

        [Fact]
        public async Task GetUserById_CallsTheRepoistory()
        {
            //arrange
            var sut = CreatSut();

            //act
            await sut.GetUserById(6).ConfigureAwait(false);

            //assert
            _usersRepository.Verify(x => x.GetUserById(6), Times.Once);
        }
    }
}