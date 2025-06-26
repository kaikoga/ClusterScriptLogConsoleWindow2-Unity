using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorView : VisualElement, IDisposable
    {
        readonly ScriptItemInspectorToolbarView toolbar;
        ListView listView;

        event Action<ScriptItemInspectorEntryViewModel> ItemSelected;
        event Action<ScriptItemInspectorEntryViewModel> ItemDoubleClicked;

        Disposable disposables = new();

        public ScriptItemInspectorView()
        {
            toolbar = new ScriptItemInspectorToolbarView();
            listView = new ListView(Array.Empty<ScriptItemInspectorEntryViewModel>(), EditorGUIUtility.singleLineHeight)
            {
                selectionType = SelectionType.Single,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder = true
            };
            
            listView.onSelectionChange += _ =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                var logEntry = (ScriptItemInspectorEntryViewModel)listView.itemsSource[listView.selectedIndex];
                ItemSelected?.Invoke(logEntry);
            };

            listView.onItemsChosen += _ =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                var logEntry = (ScriptItemInspectorEntryViewModel) listView.itemsSource[listView.selectedIndex];
                ItemDoubleClicked?.Invoke(logEntry);
            };

            hierarchy.Add(toolbar);
            hierarchy.Add(listView);
            
        }

        public void Bind(ScriptItemInspectorViewModel viewModel)
        {
            toolbar.Bind(viewModel);
            toolbar.AddTo(disposables);
            viewModel.List.Subscribe(items =>
            {
                listView.unbindItem = null;
                listView.bindItem = null;
                listView.makeItem = null;
                listView.selectedIndex = -1;
                listView.itemsSource = items;
                listView.makeItem = () => new ScriptItemInspectorEntryView();
                listView.bindItem = (e, i) =>
                {
                    ((ScriptItemInspectorEntryView)e).Bind((ScriptItemInspectorEntryViewModel)listView.itemsSource[i]);
                };
                listView.unbindItem = (e, i) =>
                {
                    ((ScriptItemInspectorEntryView)e).Dispose();
                };
            }).AddTo(disposables);
            
            ItemSelected += viewModel.SelectItem;
            ItemDoubleClicked += viewModel.SearchItem;
            disposables.Add(() =>
            {
                ItemSelected -= viewModel.SelectItem;
                ItemDoubleClicked -= viewModel.SearchItem;
            });
        }

        public void Dispose() => disposables.Dispose();
    }
}
