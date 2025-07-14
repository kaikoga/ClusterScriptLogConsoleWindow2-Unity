using System.Text.RegularExpressions;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public static class ScriptLogEntryPositionExtractor
    {
        static readonly Regex JintPositionPattern = new Regex(@"^.*<\[?([0-9]+)[,:]([0-9]+)(?:\.\.[0-9]+[,:]?[0-9]?)?[\]\)]?>.*?$", RegexOptions.Singleline);

        public static ScriptLogPosition ExtractPosition(ScriptLogEntry logEntry)
        {
            if (logEntry.Position.HasValue())
            {
                return logEntry.Position;
            }
            if (logEntry.Message == null)
            {
                return logEntry.Position;
            }
            var match = JintPositionPattern.Match(logEntry.Message);
            if (match.Success)
            {
                return new ScriptLogPosition(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value));
            }
            return ScriptLogPosition.None;
        }
    }
}