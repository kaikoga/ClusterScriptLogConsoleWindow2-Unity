using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Extensions;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogStackView : VisualElement, IDisposable
    {
        readonly ListView listView;

        event Action<ScriptLogPosition> StackItemDoubleClicked;

        readonly Disposable disposables = new();

        public ScriptLogStackView()
        {
            listView = new ListView(Array.Empty<ScriptLogStackItemViewModel>(), EditorGUIUtility.singleLineHeight)
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
                var stackItem = (ScriptLogStackItemViewModel) listView.itemsSource[listView.selectedIndex];
                StackItemDoubleClicked?.Invoke(stackItem.StackItem.Position);
            };

            listView.RegisterCopyItemCallback((ScriptLogStackItemViewModel stackItem) => stackItem.Format());

            hierarchy.Add(listView);
        }

        public void Bind(ScriptLogStackViewModel viewModel)
        {
            viewModel.Items.Subscribe(items =>
            {
                listView.unbindItem = null;
                listView.bindItem = null;
                listView.makeItem = null;
                listView.selectedIndex = -1;
                listView.itemsSource = items;
                listView.makeItem = () => new ScriptLogStackItemView();
                listView.bindItem = (e, i) =>
                {
                    ((ScriptLogStackItemView)e).Bind((ScriptLogStackItemViewModel) listView.itemsSource[i]);
                };
                listView.unbindItem = (e, i) =>
                {
                }; 

            }).AddTo(disposables);
            
            StackItemDoubleClicked += viewModel.OpenPosition;
            disposables.Add(() =>
            {
                StackItemDoubleClicked -= viewModel.OpenPosition;
            });

        }

        public void Dispose() => disposables.Dispose();
    }
}
