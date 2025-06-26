using Silksprite.ClusterScriptLogConsoleWindow2.Repository;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorEntryViewModel
    {
        public readonly string SortKey;
        readonly string label;
        readonly ScriptLogSummary logSummary;

        public ulong ItemId => logSummary.ItemId;
        public string ItemName => logSummary.ItemName;

        public int Count => logSummary.Count;

        public bool Info => logSummary.Info;
        public bool Warn => logSummary.Warn;
        public bool Error => logSummary.Error;

        public ScriptItemInspectorEntryViewModel(string sortKey, string label, ScriptLogSummary logSummary)
        {
            SortKey = sortKey;
            this.label = label;
            this.logSummary = logSummary;
        }

        public string Format()
        {
            return $"[{Count}] {label}";
        }
    }
}
