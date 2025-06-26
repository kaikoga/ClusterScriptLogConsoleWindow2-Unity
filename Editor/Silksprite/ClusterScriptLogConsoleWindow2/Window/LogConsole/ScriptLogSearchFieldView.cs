using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSearchFieldView : VisualElement, IDisposable
    {
        readonly ToolbarPopupSearchField searchField;
        
        ScriptLogFilterKind filterKind;

        event Action<ScriptLogFilterKind> DropdownMenuSelected;
        event Action<string> SearchFieldChanged;

        readonly Disposable disposables = new();
        public ScriptLogSearchFieldView()
        {
            style.flexGrow = 1;
            style.minWidth = 100;
            style.flexDirection = FlexDirection.Row;
            searchField = new ToolbarPopupSearchField
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    backgroundImage = new StyleBackground()
                },
                focusable = true
            };

            searchField.menu.AppendAction("All",
                _ => DropdownMenuSelected?.Invoke(ScriptLogFilterKind.All),
                _ => filterKind == ScriptLogFilterKind.All ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            searchField.menu.AppendAction("Message",
                _ => DropdownMenuSelected?.Invoke(ScriptLogFilterKind.Message),
                _ => filterKind == ScriptLogFilterKind.Message ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            searchField.menu.AppendAction("Item",
                _ => DropdownMenuSelected?.Invoke(ScriptLogFilterKind.Item),
                _ => filterKind == ScriptLogFilterKind.Item ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            searchField.RegisterValueChangedCallback(evt => SearchFieldChanged?.Invoke(evt.newValue));

            hierarchy.Add(searchField);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            viewModel.ListViewMatchString
                .Subscribe(matchString => searchField.SetValueWithoutNotify(matchString))
                .AddTo(disposables);

            viewModel.FilterKind.Subscribe(value => filterKind = value).AddTo(disposables);
            DropdownMenuSelected += viewModel.SetListViewMatchType;
            SearchFieldChanged += viewModel.SetListViewMatchString;
            disposables.Add(() =>
            {
                DropdownMenuSelected -= viewModel.SetListViewMatchType;
                SearchFieldChanged -= viewModel.SetListViewMatchString;
            });
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        public void ClearSearchField()
        {
            searchField.value = "";
        }
    }
}
