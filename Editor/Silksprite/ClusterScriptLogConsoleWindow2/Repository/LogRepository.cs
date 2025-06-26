using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class LogRepository
    {
        public static readonly LogRepository Instance = new();

        internal readonly ReactiveProperty<List<ScriptLogEntry>> ConsoleItems = new(new());
        public event Action LogsDiscarded;
        public event Action<ScriptLogEntry[]> LogsAdded;

        public void AddLog(ScriptLogEntry logEntry)
        {
            AddLogWithoutNotify(logEntry);
            ConsoleItems.Notify();
            LogsAdded?.Invoke(new []{ logEntry });
        }

        public void AddLogs(IEnumerable<ScriptLogEntry> logEntries)
        {
            var logArray = logEntries as ScriptLogEntry[] ?? logEntries.ToArray();
            foreach (var logEntry in logArray)
            {
                AddLogWithoutNotify(logEntry);
            }
            ConsoleItems.Notify();
            LogsAdded?.Invoke(logArray);
        }

        public void AppendLog(ScriptLogEntry logEntry)
        {
            ConsoleItems.Value.Add(logEntry);
            ConsoleItems.Notify();
            LogsAdded?.Invoke(new []{ logEntry });
        }

        public void AppendLogs(IEnumerable<ScriptLogEntry> logEntries)
        {
            var logArray = logEntries as ScriptLogEntry[] ?? logEntries.ToArray();
            foreach (var logEntry in logArray)
            {
                ConsoleItems.Value.Add(logEntry);
            }
            ConsoleItems.Notify();
            LogsAdded?.Invoke(logArray);
        }

        void AddLogWithoutNotify(ScriptLogEntry item)
        {
            var list = ConsoleItems.Value;
            var index = list.Count - 1;
            while (index >= 0)
            {
                var target = list[index];
                if (target.Timestamp <= item.Timestamp)
                {
                    break;
                }
                index--;
            }
            list.Insert(index + 1, item);
        }

        public void DiscardLogs()
        {
            ConsoleItems.Value.Clear();
            ConsoleItems.Notify();
            LogsDiscarded?.Invoke();
        }
    }
}
