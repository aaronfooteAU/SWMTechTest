using SWMTechTest.Common.Data.Enums;
using SWMTechTest.Common.Data.Models;
using SWMTechTest.WorkerService.Clients.PeopleClient.Models;
using System;

namespace SWMTechTest.WorkerService.Extensions
{
    public static class RemoteUsersExtensions
    {
        public static bool IsValid(this RemoteUser user)
        {
            return
                user.Age != null &&
                !string.IsNullOrWhiteSpace(user.First) &&
                !string.IsNullOrWhiteSpace(user.Last) &&
                !string.IsNullOrWhiteSpace(user.Gender) &&
                user.Id != null;
        }

        public static User ToUser(this RemoteUser remoteUser)
        {
            if (!remoteUser.IsValid())
                throw new Exception("RemoteUser not valid");

#pragma warning disable CS8629, CS8601, CS8604 // Nullable value type may be null.
            return new User
            {
                Id = remoteUser.Id.Value,
                FirstName = remoteUser.First,
                LastName = remoteUser.Last,
                Age = remoteUser.Age.Value,
                Gender = GetGender(remoteUser.Gender)
            };
#pragma warning restore CS8629, CS8601, CS8604 // Nullable value type may be null.
        }

        private static Gender GetGender(string gender)
        {
            switch (gender)
            {
                case "M":
                    return Gender.Male;

                case "F":
                    return Gender.Female;

                default:
                    return Gender.NotSpecified;
            }
        }
    }
}