using System;
using System.Collections.Generic;
using FilesCom;
using ManyConsole;

namespace files_cli
{
    class Files
    {
        private static log4net.ILog log;

        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(FilesCom));
        }

        public static int Main(string[] args)
        {
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Starting up");

            FilesClient filesClient = new FilesClient();

            var commands = GetCommands();
            int result = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);

            log.Info("Complete");
            return result;
        }
    }
}
