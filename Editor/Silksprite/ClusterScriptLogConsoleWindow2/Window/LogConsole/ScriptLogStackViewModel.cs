using System;
using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogStackViewModel : IDisposable
    {
        readonly ScriptLogConsoleViewModel consoleViewModel;

        internal readonly ReactiveProperty<ScriptLogStackItemViewModel[]> Items = new();
        internal readonly ReactiveProperty<bool> HasGeneratedStackItems = new();

        LogConsoleWindowSettingsRepository LogConsoleWindowSettingsRepository => LogConsoleWindowSettingsRepository.Instance;

        readonly Disposable disposables = new();

        public ScriptLogStackViewModel(ScriptLogConsoleViewModel consoleViewModel)
        {
            this.consoleViewModel = consoleViewModel;
            
            consoleViewModel.Selection.Subscribe(_ => UpdateStackItems()).AddTo(disposables);
            LogConsoleWindowSettingsRepository.IgnoreGenerated.Subscribe(_ => UpdateStackItems()).AddTo(disposables);
        }

        void UpdateStackItems()
        {
            var logEntry = consoleViewModel.Selection.Value;
            JavaScriptAsset sourceCodeAsset = null;
            ScriptLogStackItem[] stackItems;
            if (logEntry != null)
            {
                sourceCodeAsset = ItemRepository.Instance.FindSourceCodeAssetSlow(logEntry.ItemId, logEntry.ItemName, logEntry.IsPlayerScript());
                stackItems = new[]
                {
                    new ScriptLogStackItem(ScriptLogEntryPositionExtractor.ExtractPosition(logEntry), "")
                }.Concat(logEntry.Stack ?? Array.Empty<ScriptLogStackItem>()).ToArray();
            }
            else
            {
                stackItems = ScriptLogStack.None;
            }
            var resolvedStackItems = stackItems.Select(stackItem =>
            {
                var lineNumber = stackItem.Position.LineNumberOneBased;
                var columnNumber = stackItem.Position.ColumnNumberOneBased;
                var isGenerated = !Deminifier.TryGetOriginalSourcePosition(sourceCodeAsset, ref lineNumber, ref columnNumber, out _);
                return (stackItem, isGenerated);
            }).ToArray();
            var hasGeneratedStackItems = resolvedStackItems.Any(item => item.isGenerated) && resolvedStackItems.Any(item => !item.isGenerated);
            if (hasGeneratedStackItems)
            {
                if (LogConsoleWindowSettingsRepository.IgnoreGenerated.Value)
                {
                    resolvedStackItems = resolvedStackItems.Where(item => !item.isGenerated).ToArray();
                }
            }
            else
            {
                resolvedStackItems = resolvedStackItems.Select(item => (item.stackItem, false)).ToArray();
            }
            HasGeneratedStackItems.Value = hasGeneratedStackItems;
            Items.Value = resolvedStackItems
                .Select(item => new ScriptLogStackItemViewModel(item.stackItem, item.isGenerated, sourceCodeAsset))
                .ToArray();
        }

        public void OpenPosition(ScriptLogPosition position)
        {
            consoleViewModel.OpenPosition(position);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }

}
