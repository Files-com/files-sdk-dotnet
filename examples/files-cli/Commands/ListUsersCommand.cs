using Files.Models;
using System;
using System.Threading.Tasks;

namespace files_cli.Commands
{
    public class ListUsersCommand : BaseCommand
    {
        public ListUsersCommand()
        {
            IsCommand("ListUsers", "Lists all users");

            HasLongDescription("This command lists all users that can be seen by the given API Key");
        }

        public override async Task<int> RunAsync(string[] remainingArguments)
        {
            User[] users = await User.All(null, await GetOptions());

            Console.WriteLine($"Found {users.Length} users");
            Console.WriteLine();
            Console.WriteLine($"User ID, Username, Name");

            foreach(User user in users)
            {
                Console.WriteLine($"{user.Id}, {user.Username}, {user.Name}");
            }
            return Success;
        }
    }
}

