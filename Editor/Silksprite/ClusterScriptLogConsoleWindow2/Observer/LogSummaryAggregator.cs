using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    public sealed class LogSummaryAggregator : IDisposable
    {
        public static LogSummaryAggregator Instance => new();

        readonly Disposable disposables = new();

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Instance.Bind();
        }

        public void Bind()
        {
            LogRepository.Instance.LogsAdded += LogsAdded;
            LogRepository.Instance.LogsDiscarded += LogsDiscarded;
            disposables.Add(() =>
            {
                LogRepository.Instance.LogsAdded -= LogsAdded;
                LogRepository.Instance.LogsDiscarded -= LogsDiscarded;
            });
        }

        void LogsAdded(ScriptLogEntry[] logEntries)
        {
            LogSummaryRepository.Instance.Aggregate(logEntries);
        }
        
        void LogsDiscarded()
        {
            LogSummaryRepository.Instance.Reset();
        }
                
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
