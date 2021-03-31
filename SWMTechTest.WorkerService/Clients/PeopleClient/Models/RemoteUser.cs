using System.Diagnostics.CodeAnalysis;

namespace SWMTechTest.WorkerService.Clients.PeopleClient.Models
{
    [ExcludeFromCodeCoverage]
    public class RemoteUser
    {
        public int? Id { get; set; }
        public string? First { get; set; }
        public string? Last { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
    }
}