using System;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Data
{
    public static class OutputScriptLogEntryConverter
    {
        public static ScriptLogEntry ToLogEntry(OutputScriptableItemLogExt output)
        {
            var entryType = ToLogEntryType(output.type);
            return new ScriptLogEntry
            {
                Type = entryType,
                Severity = ToSeverity(entryType),
                TypeString = output.type,
                Timestamp = ToTimestamp((long)output.tsdv),
                DeviceId = output.dvid,
                ItemName = output.origin.name,
                ItemId = output.origin.id,
                PlayerName = output.player.userName,
                PlayerId = output.player.id,
                Message = output.message,
                Kind = output.kind,
                Position = ToPositionExt(output.pos),
                Stack = ToStackExt(output.stack)
            };
        }
        static DateTimeOffset ToTimestamp(long tsdv)
        {
            return tsdv == 0 ? DateTimeOffset.Now : DateTimeOffset.FromUnixTimeSeconds(tsdv);
        }
        static ScriptLogEntryType ToLogEntryType(string type)
        {
            return type switch
            {
                "ScriptLog_Dropped" => ScriptLogEntryType.ScriptLog_Dropped,
                "ScriptLog_Information" => ScriptLogEntryType.ScriptLog_Information,
                "ScriptLog_Warning" => ScriptLogEntryType.ScriptLog_Warning,
                "ScriptLog_Error" => ScriptLogEntryType.ScriptLog_Error,
                "RoomProfileLog_Information" => ScriptLogEntryType.RoomProfileLog_Information,
                "RoomProfileLog_Warning" => ScriptLogEntryType.RoomProfileLog_Warning,
                "RoomProfileLog_Error" => ScriptLogEntryType.RoomProfileLog_Error,
                "PreviewLog_Information" => ScriptLogEntryType.PreviewLog_Information,
                "PreviewLog_Warning" => ScriptLogEntryType.PreviewLog_Warning,
                "PreviewLog_Error" => ScriptLogEntryType.PreviewLog_Error,
                "Error" => ScriptLogEntryType.Error,
                _ => ScriptLogEntryType.Unknown,
            };
        }
        static ScriptLogSeverity ToSeverity(ScriptLogEntryType entryType)
        {
            return entryType switch
            {
                ScriptLogEntryType.Unknown => ScriptLogSeverity.Information,
                ScriptLogEntryType.ScriptLog_Dropped => ScriptLogSeverity.Dropped,
                ScriptLogEntryType.ScriptLog_Information => ScriptLogSeverity.Information,
                ScriptLogEntryType.ScriptLog_Warning => ScriptLogSeverity.Warning,
                ScriptLogEntryType.ScriptLog_Error => ScriptLogSeverity.Error,
                ScriptLogEntryType.RoomProfileLog_Information => ScriptLogSeverity.Information,
                ScriptLogEntryType.RoomProfileLog_Warning => ScriptLogSeverity.Warning,
                ScriptLogEntryType.RoomProfileLog_Error => ScriptLogSeverity.Error,
                ScriptLogEntryType.Error => ScriptLogSeverity.Error,
                ScriptLogEntryType.PreviewLog_Information => ScriptLogSeverity.Information,
                ScriptLogEntryType.PreviewLog_Warning => ScriptLogSeverity.Warning,
                ScriptLogEntryType.PreviewLog_Error => ScriptLogSeverity.Error,
                _ => ScriptLogSeverity.Unknown
            };
        }
        static ScriptLogPosition ToPositionExt(int[] outputPosition)
        {
            return outputPosition.Length == 2
                ? new ScriptLogPosition(outputPosition[0], outputPosition[1])
                : ScriptLogPosition.None;
        }
        static ScriptLogStackItem[] ToStackExt(OutputStackItemExt[] outputStack)
        {
            return outputStack.Select(s => new ScriptLogStackItem(
                new ScriptLogPosition(s.pos[0], s.pos[1]), s.info)
            ).ToArray();
        }
    }
}