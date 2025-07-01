using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Data
{
    public sealed class ScriptLogEntry
    {
        public ScriptLogEntryType Type;
        public ScriptLogSeverity Severity;
        public string TypeString;
        public DateTimeOffset Timestamp;
        public string DeviceId;
        public string ItemName;
        public ulong ItemId;
        public string PlayerName;
        public string PlayerId;
        public string Message;
        public string Kind;
        public ScriptLogPosition Position;
        public ScriptLogStackItem[] Stack = { };
        public bool IsPlayerScript() => Kind == "PlayerScript";

        public string ItemIdString() => ItemId.ToString();
        public string TimestampString() => Timestamp.LocalDateTime.ToString(CultureInfo.CurrentCulture);
    }
    
    public readonly struct ScriptLogPosition
    {
        public static readonly ScriptLogPosition None = new ScriptLogPosition(-1, -1);
        public readonly int LineNumberOneBased;
        public readonly int ColumnNumberOneBased;

        public ScriptLogPosition(int lineNumberOneBased, int columnNumberOneBased)
        {
            LineNumberOneBased = lineNumberOneBased;
            ColumnNumberOneBased = columnNumberOneBased;
        }

        public bool HasValue()
        {
            return LineNumberOneBased > -1;
        }
    }

    public static class ScriptLogStack
    {
        public static readonly ScriptLogStackItem[] Empty = 
        {
        };
        public static readonly ScriptLogStackItem[] None = 
        {
            ScriptLogStackItem.None
        };
    }

    public readonly struct ScriptLogStackItem
    {
        public static readonly ScriptLogStackItem None = new ScriptLogStackItem(ScriptLogPosition.None, ""); 

        public readonly ScriptLogPosition Position;
        public readonly string Info;
        
        public ScriptLogStackItem(ScriptLogPosition position, string info)
        {
            Position = position;
            Info = info;
        }
    } 

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ScriptLogEntryType
    {
        Unknown,
        ScriptLog_Dropped,
        ScriptLog_Information,
        ScriptLog_Warning,
        ScriptLog_Error,
        RoomProfileLog_Information,
        RoomProfileLog_Warning,
        RoomProfileLog_Error,
        PreviewLog_Information,
        PreviewLog_Warning,
        PreviewLog_Error,
        Error,
    }

    public enum ScriptLogSeverity
    {
        Unknown,
        Error,
        Warning,
        Information,
        Dropped,
    }

}
