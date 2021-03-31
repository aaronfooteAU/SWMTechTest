using SWMTechTest.Models.Response.Enum;

namespace SWMTechTest.Models.Response
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