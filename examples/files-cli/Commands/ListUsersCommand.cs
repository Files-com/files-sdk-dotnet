using FilesCom;
using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCli.Commands
{
    internal sealed class ListUsersCommand : FilesCommand
    {
        public ListUsersCommand() : base("list-users", "ListUsers", "Lists the users these credentials can see.")
        {
        }

        protected override async Task ExecuteAsync(ParseResult parseResult, FilesClient client, Dictionary<string, object> credentials, CancellationToken cancellationToken)
        {
            // Loads every page of users.
            List<User> users = await client.Users.List(null, credentials).AllAsync(cancellationToken);

            Console.WriteLine($"Found {users.Count} users");
            Console.WriteLine();
            Console.WriteLine("User ID, Username, Name");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Id}, {user.Username}, {user.Name}");
            }
        }
    }
}