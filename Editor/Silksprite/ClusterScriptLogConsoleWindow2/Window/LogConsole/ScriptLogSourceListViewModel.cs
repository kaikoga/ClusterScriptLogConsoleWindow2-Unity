using System;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using Object = UnityEngine.Object;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSourceListViewModel : IDisposable
    {
        readonly ScriptLogConsoleViewModel consoleViewModel;
        
        internal readonly ReactiveProperty<ScriptLogSourceItemViewModel[]> Items = new();

        readonly Disposable disposables = new();

        public ScriptLogSourceListViewModel(ScriptLogConsoleViewModel consoleViewModel)
        {
            this.consoleViewModel = consoleViewModel;
            
            consoleViewModel.Selection.Subscribe(_ => UpdateSourceListItems()).AddTo(disposables);
        }

        void UpdateSourceListItems()
        {
            if (consoleViewModel.Selection.Value is { } logEntry)
            {
                var sourceCodeAsset = ItemRepository.Instance.FindSourceCodeAssetSlow(logEntry.ItemId, logEntry.ItemName, logEntry.IsPlayerScript());
                if (sourceCodeAsset != null)
                {
                    Items.Value = Deminifier.CollectOriginalSources(sourceCodeAsset)
                        .Select(sourceRef => new ScriptLogSourceItemViewModel(sourceRef.Asset, sourceRef.Path)).ToArray();
                }
                else
                {
                    Items.Value = Array.Empty<ScriptLogSourceItemViewModel>();
                }
            }
            else
            {
                Items.Value = Array.Empty<ScriptLogSourceItemViewModel>();
            }
        }

        public void OpenAsset(Object sourceAsset, string sourcePath)
        {
            AssetOpener.DirectOpenAsset(sourceAsset, sourcePath, 1, 1);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
