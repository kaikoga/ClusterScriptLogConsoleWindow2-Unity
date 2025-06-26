using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Extensions;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSourceListView : VisualElement, IDisposable
    {
        readonly ListView listView;

        event Action<UnityEngine.Object, string> SourceItemDoubleClicked;

        readonly Disposable disposables = new();

        public ScriptLogSourceListView()
        {
            listView = new ListView(Array.Empty<ScriptLogSourceItemViewModel>(), EditorGUIUtility.singleLineHeight)
            {
                selectionType = SelectionType.Single,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder = true,
                style =
                {
                    flexGrow = 1.0f
                }
            };

            listView.onItemsChosen += _ =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                var sourceItem = (ScriptLogSourceItemViewModel) listView.itemsSource[listView.selectedIndex];
                SourceItemDoubleClicked?.Invoke(sourceItem.SourceAsset, sourceItem.SourcePath);
            };

            listView.RegisterCopyItemCallback((ScriptLogSourceItemViewModel sourceItem) => sourceItem.Format());

            hierarchy.Add(listView);
        }

        public void Bind(ScriptLogSourceListViewModel viewModel)
        {
            viewModel.Items.Subscribe(items =>
            {
                listView.unbindItem = null;
                listView.bindItem = null;
                listView.makeItem = null;
                listView.selectedIndex = -1;
                listView.itemsSource = items;
                listView.makeItem = () => new ScriptLogSourceItemView();
                listView.bindItem = (e, i) =>
                {
                    ((ScriptLogSourceItemView)e).Bind((ScriptLogSourceItemViewModel)listView.itemsSource[i]);
                };
                listView.unbindItem = (e, i) =>
                {
                }; 
            }).AddTo(disposables);
            
            SourceItemDoubleClicked += viewModel.OpenAsset;
            disposables.Add(() =>
            {
                SourceItemDoubleClicked -= viewModel.OpenAsset;
            });

        }

        public void Dispose() => disposables.Dispose();
    }
}
