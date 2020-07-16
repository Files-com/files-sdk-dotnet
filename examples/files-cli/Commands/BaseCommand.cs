using ManyConsole;
using System;
using System.Threading.Tasks;
using Files.Models;
using System.Collections.Generic;

namespace files_cli.Commands
{
    public abstract class BaseCommand : ConsoleCommand
    {
        protected const int Success = 0;
        protected const int Failure = 2;

        public string Username { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }

        public BaseCommand()
        {
            Username = null;
            Password = null;
            Otp = null;

            HasOption("u|user=", "The username to use for session authentication.", username => Username = username);
            HasOption("p|password=", "The password to use for session authentication.", password => Password = password);
            HasOption("o|otp=", "A One-Time Password if 2FA is enabled.", otp => Otp = otp);
        }

        public async Task<Dictionary<string, object>> GetOptions()
        { 
            Dictionary<string, object> options = new Dictionary<string, object>();

            if (Username != null) {
                if (Password == null) {
                    Console.WriteLine("username and password must both be present for session authentication");
                }

                var parameters = new Dictionary<string, object>();
                parameters.Add("username", Username);
                parameters.Add("password", Password);
                if (Otp != null)
                {
                    parameters.Add("otp", Otp);
                }

                Session session = await Session.Create(parameters);
                options.Add("session_id", session.Id);
            }

            return options;
        }

        public virtual async Task<int> RunAsync(string[] remainingArguments)
        {
            return await Task.Run(() => { return Failure; });
        }

        public override int Run(string[] remainingArguments)
        {
            int result = Failure;

            Task.Run(async () =>
            {
                try
                {
                    result = await RunAsync(remainingArguments);
                }
                   catch (Exception e)
                {
                   Console.Error.WriteLine(e.Message);
                   Console.Error.WriteLine(e.StackTrace);
                }
            }).GetAwaiter().GetResult();

            return result;
        }
    }
}

