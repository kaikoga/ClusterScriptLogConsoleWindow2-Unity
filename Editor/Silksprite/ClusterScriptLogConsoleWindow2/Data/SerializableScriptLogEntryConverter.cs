using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Data
{
    public static class SerializableScriptLogEntryConverter
    {

        public static ScriptLogEntry ToLogEntry(SerializableScriptLogEntry serializable)
        {
            return new ScriptLogEntry
            {
                Type = (ScriptLogEntryType)serializable.Type,
                Severity = (ScriptLogSeverity)serializable.Severity,
                TypeString = serializable.TypeString,
                Timestamp = serializable.Timestamp,
                DeviceId = serializable.DeviceId,
                ItemName = serializable.ItemName,
                ItemId = serializable.ItemId,
                PlayerName = serializable.PlayerName,
                PlayerId = serializable.PlayerId,
                Message = serializable.Message,
                Kind = serializable.Kind,
                Position = ToPositionExt(serializable.Position),
                Stack = ToStackExt(serializable.Stack),
            };
        }

        static ScriptLogPosition ToPositionExt(SerializableScriptLogPosition serializable)
        {
            return new ScriptLogPosition(serializable.LineNumberOneBased, serializable.ColumnNumberOneBased);
        }

        static ScriptLogStackItem[] ToStackExt(SerializableScriptLogStackItem[] serializable)
        {
            return serializable.Select(s => new ScriptLogStackItem(
                ToPositionExt(s.Position), s.Info)
            ).ToArray();
        }

    }
}
