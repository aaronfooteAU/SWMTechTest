using SWMTechTest.ConsoleApp.Clients.Models.Enums;

namespace SWMTechTest.ConsoleApp.Clients.Model
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public Gender Gender { get; set; }
    }
}