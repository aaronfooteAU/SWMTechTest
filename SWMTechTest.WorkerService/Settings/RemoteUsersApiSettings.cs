#nullable disable

using System.Diagnostics.CodeAnalysis;

namespace SWMTechTest.WorkerService.Settings
{
    [ExcludeFromCodeCoverage]
    public class RemoteUsersApiSettings
    {
        public string BaseAddress { get; set; }
        public string GetUsersEndpoint { get; set; }
        public int DefaultPageSize { get; set; }
    }
}