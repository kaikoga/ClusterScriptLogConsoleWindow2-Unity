using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class ItemInspectorWindowSettingsRepository
    {
        const string ModeKey = "net.kaikoga.cslcw2.itemInspector.Mode";
        const string OrderKey = "net.kaikoga.cslcw2.itemInspector.Order";

        public static readonly ItemInspectorWindowSettingsRepository Instance = new();

        internal readonly ReactivePropertyBase<ScriptItemInspectorMode> Mode = new ReactiveEditorPrefsEnum<ScriptItemInspectorMode>(ModeKey, ScriptItemInspectorMode.ShowItemId);
        internal readonly ReactivePropertyBase<ScriptItemInspectorOrder> Order = new ReactiveEditorPrefsEnum<ScriptItemInspectorOrder>(OrderKey, ScriptItemInspectorOrder.SortOrder);
    }
}
