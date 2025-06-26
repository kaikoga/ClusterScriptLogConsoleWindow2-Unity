using System.Collections.Concurrent;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class LogFileWatcherRepository
    {
        const string LogFileKey = "net.kaikoga.cslcw2.LogFile";

        public static readonly LogFileWatcherRepository Instance = new();
        
        public readonly ConcurrentQueue<bool> ScriptLogFileUpdateEvents = new();

        internal readonly ReactivePropertyBase<string> ScriptLogFilePath = new ReactivePlayerPrefsString(LogFileKey, LogFileWatcherConstants.ClusterScriptLogFilePath);
        internal readonly ReactiveProperty<bool> IsPaused = new();
        internal readonly ReactiveProperty<long> LogFilePosition = new();
    }
}
