using ManyConsole;
using System;
using System.Threading.Tasks;

namespace files_cli.Commands
{
    public abstract class BaseCommand : ConsoleCommand
    {
        protected const int Success = 0;
        protected const int Failure = 2;

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
