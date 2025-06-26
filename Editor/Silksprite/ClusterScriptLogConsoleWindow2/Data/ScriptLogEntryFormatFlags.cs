using System;
using JetBrains.Annotations;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Data
{
    [Flags]
    public enum ScriptLogEntryFormatFlags
    {
        Timestamp = 1 << 0,
        DeviceId = 1 << 1,
        ItemId = 1 << 2,
        ItemName = 1 << 3,
        PlayerId = 1 << 4,
        PlayerName = 1 << 5,
        Type = 1 << 6,
        Message = 1 << 7,
        Kind = 1 << 8,
        Position = 1 << 9,
        All = (1 << 10) - 1,
        Default = 
            Timestamp |
            DeviceId |
            ItemId |
            ItemName |
            PlayerId |
            PlayerName |
            Type |
            Message
    }

    [PublicAPI, Flags]
    public enum ScriptLogEntryFormatDisplayFlags
    {
        Timestamp = 1 << 0,
        DeviceId = 1 << 1,
        ItemId = 1 << 2,
        ItemName = 1 << 3,
        PlayerId = 1 << 4,
        PlayerName = 1 << 5,
        Type = 1 << 6,
        Message = 1 << 7,
        Kind = 1 << 8,
        Position = 1 << 9,
    }

}
