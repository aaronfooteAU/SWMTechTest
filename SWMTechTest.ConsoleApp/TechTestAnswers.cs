using SWMTechTest.ConsoleApp.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SWMTechTest.ConsoleApp
{
    public class TechTestAnswers
    {
        private readonly IUserServiceClient _client;

        public TechTestAnswers(IUserServiceClient client)
        {
            _client = client;
        }

        public async Task ProduceAnswers()
        {
            await TheUsersFullNameWithId42().ConfigureAwait(false);
            await TheFirstNameOfAllUsersWhoAre23().ConfigureAwait(false);
            await TheNumberOfGendersPerAge().ConfigureAwait(false);
        }

        private async Task TheUsersFullNameWithId42()
        {
            var id = 42;
            var user = await _client.GetUserById(id).ConfigureAwait(false);
            if (user == null)
                Console.WriteLine($"The user with Id {id} was not found");
            else
                Console.WriteLine($"The Full name for the user with Id {id} is: '{user.FirstName} {user.LastName}'");
        }

        private async Task TheFirstNameOfAllUsersWhoAre23()
        {
            var age = 23;
            var users = await _client.GetUsersByAge(age).ConfigureAwait(false);
            if (!users.Any())
            {
                Console.WriteLine($"There are no users aged {age}");
            }
            else
            {
                var firstNames = string.Join(",", users.Select(s => s.FirstName));
                Console.WriteLine($"The First name for all users aged {age} are: '{firstNames}'");
            }
        }

        private async Task TheNumberOfGendersPerAge()
        {
            var users = await _client.GetAllUsers().ConfigureAwait(false);

            if (!users.Any())
            {
                Console.WriteLine("No users were found");
                return;
            }

            Console.WriteLine("Genders by Age:");

            foreach (var ages in users.GroupBy(u => u.Age).OrderBy(a => a.Key))
            {
                var result = $"Age: {ages.Key}";

                foreach (var gender in ages.GroupBy(a => a.Gender).OrderBy(g => g.Key))
                {
                    result += $" {gender.Key}: {gender.Count()}";
                }
                Console.WriteLine(result);
            }
        }
    }
}