using StardewModdingAPI;

namespace QuestFramework.Framework
{
    internal static class Logger
    {
        private static IMonitor? _monitor;
        private static bool _verbose;

        public static void Setup(IMonitor monitor, bool verbose = false)
        {
            _monitor = monitor;
            _verbose = verbose;
        }

        public static void Verbose(string message)
        {
            if (!_verbose) { return; }

            _monitor?.Log(message, LogLevel.Trace);
        }

        public static void Trace(string message)
        {
            _monitor?.Log(message, LogLevel.Trace);
        }

        public static void Debug(string message)
        {
            _monitor?.Log(message, LogLevel.Debug);
        }

        public static void Info(string message)
        {
            _monitor?.Log(message, LogLevel.Info);
        }

        public static void Warn(string message)
        {
            _monitor?.Log(message, LogLevel.Warn);
        }

        public static void Error(string message, Exception? error = null, bool stack = true)
        {
            if (error == null)
            {
                _monitor?.Log(message, LogLevel.Error);
                return;
            }

            _monitor?.Log($"{message}: {error.Message}{(stack ? $"\n\n{error}" : "")}", LogLevel.Error);
        }

        public static void Error(Exception error, bool stack = true)
        {
            _monitor?.Log($"An error occured: {error.Message}{(stack ? $"\n\n{error}" : "")}", LogLevel.Error);
        }
    }
}
