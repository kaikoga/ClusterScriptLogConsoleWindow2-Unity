using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class LogSummaryRepository
    {
        public static readonly LogSummaryRepository Instance = new();

        public readonly ConcurrentDictionary<string, ScriptLogSummary> ByItemNames = new();
        public readonly ConcurrentDictionary<ulong, ScriptLogSummary> ByItemIds = new();
        public readonly Dictionary<ulong, string> ItemNames = new();

        public Action OnUpdated;

        public void Reset()
        {
            ByItemNames.Clear();
            ByItemIds.Clear();
            ItemNames.Clear();
        }

        public void Aggregate(ScriptLogEntry[] logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                ByItemNames.GetOrAdd(logEntry.ItemName, _ => new ScriptLogSummary(logEntry.ItemId, logEntry.ItemName)).Aggregate(logEntry);
                ByItemIds.GetOrAdd(logEntry.ItemId, _ => new ScriptLogSummary(0L, logEntry.ItemName)).Aggregate(logEntry);
                ItemNames[logEntry.ItemId] = logEntry.ItemName;
            }
            OnUpdated?.Invoke();
        }
        
    }

    public class ScriptLogSummary
    {
        public readonly ulong ItemId;
        public readonly string ItemName;

        public int Count;
        public bool Info;
        public bool Warn;
        public bool Error;

        public ScriptLogSummary(ulong itemId, string itemName)
        {
            ItemId = itemId;
            ItemName = itemName;
        }

        public void Aggregate(ScriptLogEntry logEntry)
        {
            Count++;
            switch (logEntry.Severity)
            {

                case ScriptLogSeverity.Error:
                    Error = true;
                    break;
                case ScriptLogSeverity.Warning:
                    Warn = true;
                    break;
                case ScriptLogSeverity.Information:
                    Info = true;
                    break;
            }
        }
    }
}
