using FilesCom;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Linq;

namespace FilesTests
{
    // Records what the SDK logs, at every level, while it is in use.
    public sealed class LogCapture : IDisposable
    {
        private readonly MemoryAppender appender = new MemoryAppender { Fix = FixFlags.Message | FixFlags.Exception };
        private readonly Hierarchy repository = (Hierarchy)log4net.LogManager.GetRepository(typeof(FilesClient).Assembly);
        private readonly Level originalLevel;

        public LogCapture()
        {
            originalLevel = repository.Root.Level;
            repository.Root.Level = Level.Debug;
            log4net.Config.BasicConfigurator.Configure(repository, appender);
        }

        // What was logged above DEBUG, one event a line, with its exception.
        public string Normal => Render(e => e.Level > Level.Debug);

        public string Debug => Render(e => e.Level == Level.Debug);

        // The messages logged above DEBUG that contain text.
        public string[] Lines(string text)
        {
            return appender.GetEvents().Where(e => e.Level > Level.Debug && e.RenderedMessage.Contains(text)).Select(e => e.RenderedMessage).ToArray();
        }

        public void Dispose()
        {
            repository.Root.RemoveAppender(appender);
            repository.Root.Level = originalLevel;
        }

        private string Render(Func<LoggingEvent, bool> include)
        {
            return string.Join("\n", appender.GetEvents().Where(include).Select(e => e.RenderedMessage + e.GetExceptionString()));
        }
    }
}