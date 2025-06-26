using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Format
{
    [PublicAPI]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class SerializableScriptLogEntry
    {
        public SerializableScriptLogEntryType Type;
        public SerializableScriptLogSeverity Severity;
        public string TypeString;
        public DateTimeOffset Timestamp;
        public string DeviceId;
        public string ItemName;
        public ulong ItemId;
        public string PlayerName;
        public string PlayerId;
        public string Message;
        public string Kind;
        public SerializableScriptLogPosition Position;
        public SerializableScriptLogStackItem[] Stack = { };

        public string ItemIdString() => ItemId.ToString();
        public string TimestampString() => Timestamp.LocalDateTime.ToString(CultureInfo.CurrentCulture);
    }
    
    [PublicAPI]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct SerializableScriptLogPosition
    {
        public int LineNumberOneBased;
        public int ColumnNumberOneBased;
    }

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct SerializableScriptLogStackItem
    {
        public SerializableScriptLogPosition Position;
        public string Info;
    } 

    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SerializableScriptLogEntryType
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

    [PublicAPI]
    public enum SerializableScriptLogSeverity
    {
        Unknown,
        Error,
        Warning,
        Information,
        Dropped,
    }
}
