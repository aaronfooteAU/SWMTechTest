using SWMTechTest.Common.Data.Models;
using SWMTechTest.Models.Response;
using SWMTechTest.Models.Response.Enum;
using System.Collections.Generic;
using System.Linq;

namespace SWMTechTest.Extensions
{
    public static class UsersExtensions
    {
        public static UserDto ToDto(this User person)
        {
            return new UserDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = person.Age,
                Gender = (Gender)person.Gender
            };
        }

        public static IEnumerable<UserDto> ToDto(this IEnumerable<User> people)
        {
            return people.Select(p => p.ToDto());
        }
    }
}