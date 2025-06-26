using System.Text;
using ClusterVR.CreatorKit.Item.Implements;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public static class ScriptLogEntryFormat
    {
        public static string Format(this ScriptLogEntry e, ScriptLogEntryFormatFlags f = ScriptLogEntryFormatFlags.All)
        {
            return e.Type switch
            {
                ScriptLogEntryType.ScriptLog_Information or ScriptLogEntryType.ScriptLog_Warning or ScriptLogEntryType.ScriptLog_Error
                    => BuildScriptLogString(e, f),
                ScriptLogEntryType.PreviewLog_Information or ScriptLogEntryType.PreviewLog_Warning or ScriptLogEntryType.PreviewLog_Error
                    => BuildScriptLogString(e, f),
                ScriptLogEntryType.RoomProfileLog_Information or ScriptLogEntryType.RoomProfileLog_Warning or ScriptLogEntryType.RoomProfileLog_Error
                    => BuildRoomProfileLogString(e, f),
                ScriptLogEntryType.ScriptLog_Dropped => $"{e.TypeString,-32} some logs have been dropped",
                ScriptLogEntryType.Error => $"{e.TypeString,-32} {e.Message}",
                _ => $"{e.TypeString,-32} unknown type",
            };
        }

        static string BuildScriptLogString(this ScriptLogEntry e, ScriptLogEntryFormatFlags f)
        {
            var sb = new StringBuilder();
            if ((f & ScriptLogEntryFormatFlags.Timestamp) != 0)
            {
                sb.Append($"[{e.Timestamp.LocalDateTime}] ");
            }
            if ((f & ScriptLogEntryFormatFlags.DeviceId) != 0)
            {
                sb.Append($"devid:{e.DeviceId} ");
            }
            if ((f & (ScriptLogEntryFormatFlags.ItemId | ScriptLogEntryFormatFlags.ItemName)) != 0)
            {
                sb.Append("item:");
                if ((f & ScriptLogEntryFormatFlags.ItemId) != 0)
                {
                    sb.Append(e.ItemId);
                    sb.Append(" ");
                }
                if ((f & ScriptLogEntryFormatFlags.ItemName) != 0)
                {
                    sb.Append($"[{e.ItemName}] ");
                }
            }
            if ((f & (ScriptLogEntryFormatFlags.PlayerId | ScriptLogEntryFormatFlags.PlayerName)) != 0)
            {
                sb.Append("player:");
                if ((f & ScriptLogEntryFormatFlags.PlayerId) != 0)
                {
                    sb.Append(e.PlayerId);
                    sb.Append(" ");
                }
                if ((f & ScriptLogEntryFormatFlags.PlayerName) != 0)
                {
                    sb.Append($"[{e.PlayerName}] ");
                }
            }
            if ((f & (ScriptLogEntryFormatFlags.Type | ScriptLogEntryFormatFlags.Message | ScriptLogEntryFormatFlags.Kind)) != 0)
            {
                sb.Append("\n");
            }
            if ((f & ScriptLogEntryFormatFlags.Type) != 0)
            {
                sb.Append($"{e.TypeString,-32}");
            }
            if ((f & ScriptLogEntryFormatFlags.Message) != 0)
            {
                sb.Append(e.Message);
            }
            if ((f & ScriptLogEntryFormatFlags.Kind) != 0)
            {
                sb.Append(" ");
                sb.Append(e.Kind);
            }
            if ((f & ScriptLogEntryFormatFlags.Position) != 0)
            {
                sb.Append(" ");
                sb.Append(Format(e.Position));
                if (e.Stack != null)
                {
                    foreach (var stack in e.Stack)
                    {
                        sb.Append($"\n  at {Format(stack)}");
                    }
                }
            }
            return sb.ToString();
        }

        public static string Format(this ScriptLogStackItem stack, JavaScriptAsset sourceCodeAsset = null)
        {
            return $"{stack.Info} {Format(stack.Position, sourceCodeAsset)}";
        }

        static string Format(this ScriptLogPosition position, JavaScriptAsset sourceCodeAsset = null)
        {
            if (!position.HasValue())
            {
                return "<>";
            }
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(position.LineNumberOneBased);
            sb.Append(":");
            sb.Append(position.ColumnNumberOneBased);
            if (sourceCodeAsset != null)
            {
                var lineNumber = position.LineNumberOneBased;
                var columnNumber = position.ColumnNumberOneBased;
                if (Deminifier.TryGetOriginalSourcePosition(sourceCodeAsset, ref lineNumber, ref columnNumber, out var sourceRef))
                {
                    sb.Append("@");
                    sb.Append(sourceRef.Path);
                    sb.Append(":");
                    sb.Append(lineNumber);
                    sb.Append(":");
                    sb.Append(columnNumber);
                }
            }
            sb.Append(">");
            return sb.ToString();
        }

        static string BuildRoomProfileLogString(this ScriptLogEntry e, ScriptLogEntryFormatFlags f)
        {
            return BuildScriptLogString(e, f & (ScriptLogEntryFormatFlags.Timestamp | ScriptLogEntryFormatFlags.Type | ScriptLogEntryFormatFlags.Message));
        }
    }
}
