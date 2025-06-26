using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Extensions;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogListView : VisualElement, IDisposable
    {
        ListView listView;
        bool isLastItemVisible = true;

        LogRepository LogRepository => LogRepository.Instance;

        event Action<ScriptLogEntry> LogItemSelected;
        event Action<ScriptLogEntry> LogItemDoubleClicked;

        Disposable disposables = new();

        public ScriptLogListView()
        {
            const int itemHeight = 32;
            listView = new ListView(Array.Empty<ScriptLogEntry>(), itemHeight)
            {
                selectionType = SelectionType.Single,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBorder = true,
                style =
                {
                    flexGrow = 1.0f
                }
            };

            listView.onSelectionChange += _ =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                var logEntry = (ScriptLogEntry)listView.itemsSource[listView.selectedIndex];
                LogItemSelected?.Invoke(logEntry);
            };

            listView.onItemsChosen += _ =>
            {
                if (listView.selectedIndex == -1)
                {
                    return;
                }
                var logEntry = (ScriptLogEntry) listView.itemsSource[listView.selectedIndex];
                LogItemDoubleClicked?.Invoke(logEntry);
            };

            listView.RegisterCopyItemCallback((ScriptLogEntry logEntry) => logEntry.Format());

            hierarchy.Add(listView);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            viewModel.MatchedItems.Subscribe(list =>
            {
                listView.unbindItem = null;
                listView.bindItem = null;
                listView.makeItem = null;
                listView.itemsSource = list;
                listView.makeItem = () => new ScriptLogEntryView();
                listView.bindItem = (e, i) =>
                {
                    ((ScriptLogEntryView)e).Bind(viewModel, list[i]);
                    if (i == list.Count - 1)
                    {
                        isLastItemVisible = true;
                    }
                };
                listView.unbindItem = (e, i) =>
                {
                    if (i == list.Count - 1)
                    {
                        isLastItemVisible = false;
                    }
                }; 

                if (isLastItemVisible)
                {
                    listView.ScrollToItem(-1);
                }
            }).AddTo(disposables);
            LogItemSelected += viewModel.SelectLogItem;
            LogItemDoubleClicked += viewModel.OpenLogItem;
            LogRepository.LogsDiscarded += listView.ClearSelection;
            disposables.Add(() =>
            {
                LogItemSelected -= viewModel.SelectLogItem;
                LogItemDoubleClicked -= viewModel.OpenLogItem;
                LogRepository.LogsDiscarded -= listView.ClearSelection;
            });
        }

        public void Dispose() => disposables.Dispose();
    }
}
