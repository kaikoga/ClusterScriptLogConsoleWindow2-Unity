using System;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Broker;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorViewModel : IDisposable
    {
        internal readonly ReactiveProperty<ScriptItemInspectorEntryViewModel[]> List = new();
        internal ReadonlyReactiveProperty<ScriptItemInspectorMode> ItemInspectorMode => ItemInspectorWindowSettingsRepository.Mode;
        internal ReadonlyReactiveProperty<ScriptItemInspectorOrder> ItemInspectorOrder => ItemInspectorWindowSettingsRepository.Order;

        Func<ScriptLogSummary, bool> severityFilter = AlwaysTrueFilter;

        static bool AlwaysTrueFilter(ScriptLogSummary summary) => true;

        ItemRepository ItemRepository => ItemRepository.Instance;
        LogSummaryRepository LogSummaryRepository => LogSummaryRepository.Instance;
        ItemInspectorWindowSettingsRepository ItemInspectorWindowSettingsRepository => ItemInspectorWindowSettingsRepository.Instance;

        SearchRequester SearchRequester => SearchRequester.Instance;
        WindowNavigationRequester WindowNavigationRequester => WindowNavigationRequester.Instance;

        readonly Disposable disposables = new();

        public ScriptItemInspectorViewModel()
        {
            LogSummaryRepository.OnUpdated += RepopulateList;
            disposables.Add(() => LogSummaryRepository.OnUpdated -= RepopulateList);
            RepopulateList();
        }

        internal void SetItemInspectorMode(ScriptItemInspectorMode newValue)
        {
            ItemInspectorWindowSettingsRepository.Mode.Value = newValue;
            RepopulateList();
        }

        internal void SetItemInspectorOrder(ScriptItemInspectorOrder newValue)
        {
            ItemInspectorWindowSettingsRepository.Order.Value = newValue;
            RepopulateList();
        }

        public void SetSeverityFilter(bool info, bool warn, bool error)
        {
            severityFilter = summary => info && summary.Info || warn && summary.Warn || error && summary.Error; 
            RepopulateList();
        }

        public void SelectItem(ScriptItemInspectorEntryViewModel entry)
        {
            var item = ItemRepository.FindItemSlow(entry.ItemId, entry.ItemName);
            if (item != null)
            {
                EditorGUIUtility.PingObject(item.gameObject);
            }
        }

        public void SearchItem(ScriptItemInspectorEntryViewModel entry)
        {
            WindowNavigationRequester.OnOpenLogConsoleWindow();
            SearchRequester.OnSetMatchString(entry.SortKey);
        }

        void RepopulateList()
        {
            var unorderedList = ItemInspectorMode.Value switch
            {
                ScriptItemInspectorMode.ShowItemId => LogSummaryRepository.ByItemIds
                    .Where(kv => severityFilter(kv.Value))
                    .Select(kv =>
                    {
                        var label = $"{LogSummaryRepository.ItemNames[kv.Key]}({kv.Key})";
                        var summary = kv.Value;
                        return new ScriptItemInspectorEntryViewModel(kv.Key.ToString(), label, summary);
                    }),
                ScriptItemInspectorMode.ShowItemName => LogSummaryRepository.ByItemNames
                    .Where(kv => severityFilter(kv.Value))
                    .Select(kv =>
                    {
                        var label = kv.Key;
                        var summary = kv.Value;
                        return new ScriptItemInspectorEntryViewModel(kv.Key, label, summary);
                    }),
                _ => throw new ArgumentOutOfRangeException()
            };
            List.Value = ItemInspectorOrder.Value switch
            {

                ScriptItemInspectorOrder.SortOrder => unorderedList.ToArray(),
                ScriptItemInspectorOrder.SortByItemId => unorderedList.OrderBy(item => item.ItemId).ToArray(),
                ScriptItemInspectorOrder.SortByItemName => unorderedList.OrderBy(item => item.ItemName).ToArray(),
                ScriptItemInspectorOrder.SortByCount => unorderedList.OrderByDescending(item => item.Count).ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
