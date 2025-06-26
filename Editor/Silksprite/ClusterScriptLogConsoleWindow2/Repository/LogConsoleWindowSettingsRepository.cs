using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class LogConsoleWindowSettingsRepository
    {
        const string FormatKey = "net.kaikoga.cslcw2.Format";
        const string IgnoreGeneratedKey = "net.kaikoga.cslcw2.IgnoreGenerated";
        const string SplitViewSizeKey = "net.kaikoga.cslcw2.SplitViewSize";
        const string UpperSplitViewSizeKey = "net.kaikoga.cslcw2.UpperSplitViewSize";
        const string LowerSplitViewSizeKey = "net.kaikoga.cslcw2.LowerSplitViewSize";

        public static readonly LogConsoleWindowSettingsRepository Instance = new();

        internal readonly ReactivePropertyBase<ScriptLogEntryFormatFlags> FormatFlags = new ReactiveEditorPrefsEnum<ScriptLogEntryFormatFlags>(FormatKey, ScriptLogEntryFormatFlags.Default);
        internal readonly ReactivePropertyBase<bool> IgnoreGenerated = new ReactiveEditorPrefsBool(IgnoreGeneratedKey, false);
        internal readonly ReactivePropertyBase<float> SplitViewSize = new ReactiveEditorPrefsFloat(SplitViewSizeKey, 60f);
        internal readonly ReactivePropertyBase<float> UpperSplitViewSize = new ReactiveEditorPrefsFloat(UpperSplitViewSizeKey, 48f);
        internal readonly ReactivePropertyBase<float> LowerSplitViewSize = new ReactiveEditorPrefsFloat(LowerSplitViewSizeKey, 200f);
    }
}
