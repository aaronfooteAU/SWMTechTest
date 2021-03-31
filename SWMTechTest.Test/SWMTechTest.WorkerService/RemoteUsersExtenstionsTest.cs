using SWMTechTest.Common.Data.Enums;
using SWMTechTest.WorkerService.Clients.PeopleClient.Models;
using SWMTechTest.WorkerService.Extensions;
using Xunit;

namespace SWMTechTest.Test.SWMTechTest.WorkerService
{
    public class RemoteUsersExtenstionsTest
    {
        [Theory]
        [InlineData("M", Gender.Male)]
        [InlineData("F", Gender.Female)]
        [InlineData("T", Gender.NotSpecified)]
        [InlineData("Y", Gender.NotSpecified)]
        public void ToUser_When_ParsingGender_ResolvesCorrectly(string testGender, Gender expectedGender)
        {
            //arrange
            var remoteUser = new RemoteUser { Id = 1, First = "F", Last = "L", Age = 3, Gender = testGender };

            //act
            var user = RemoteUsersExtensions.ToUser(remoteUser);

            //assert
            Assert.Equal(expectedGender, user.Gender);
        }
    }
}