using SWMTechTest.Common.Data.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SWMTechTest.Common.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is User user &&
                   Id == user.Id &&
                   FirstName == user.FirstName &&
                   LastName == user.LastName &&
                   Age == user.Age &&
                   Gender == user.Gender;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, Age, Gender);
        }
    }
}