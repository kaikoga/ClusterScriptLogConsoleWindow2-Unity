using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Broker;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogConsoleViewModel : IDisposable
    {
        public readonly ScriptLogStackViewModel StackViewModel;
        public readonly ScriptLogSourceListViewModel SourceListViewModel;

        static readonly string[] WhiteSpaces = { " ", "　" };

        internal readonly ReactiveProperty<List<ScriptLogEntry>> MatchedItems = new(new());
        internal readonly ReactiveProperty<int> InfoCount = new();
        internal readonly ReactiveProperty<int> WarnCount = new();
        internal readonly ReactiveProperty<int> ErrorCount = new();

        internal readonly ReactiveProperty<ScriptLogFilterKind> FilterKind = new();
        
        internal readonly ReactiveProperty<ScriptLogEntry> Selection = new();
        internal readonly ReactiveProperty<string> ListViewMatchString = new("");

        internal ReadonlyReactiveProperty<ScriptLogEntryFormatFlags> FormatFlags => LogConsoleWindowSettingsRepository.FormatFlags;
        internal ReadonlyReactiveProperty<string> ScriptLogFilePath => LogFileWatcherRepository.ScriptLogFilePath;
        internal ReadonlyReactiveProperty<bool> IsPaused => LogFileWatcherRepository.IsPaused;

        Func<ScriptLogEntry, bool> matchStringFilter = AlwaysTrueFilter;
        Func<ScriptLogEntry, bool> severityFilter = AlwaysTrueFilter;

        static bool AlwaysTrueFilter(ScriptLogEntry entry) => true;

        LogRepository LogRepository => LogRepository.Instance;
        LogFileWatcherRepository LogFileWatcherRepository => LogFileWatcherRepository.Instance;
        LogConsoleWindowSettingsRepository LogConsoleWindowSettingsRepository => LogConsoleWindowSettingsRepository.Instance;
        ItemRepository ItemRepository => ItemRepository.Instance;

        SearchRequester SearchRequester => SearchRequester.Instance;

        readonly Disposable disposables = new();

        public ScriptLogConsoleViewModel()
        {
            SourceListViewModel = new(this);
            SourceListViewModel.AddTo(disposables);
            StackViewModel = new(this);
            StackViewModel.AddTo(disposables);

            LogRepository.ConsoleItems.Subscribe(RefreshMatchedItems).AddTo(disposables);
            ListViewMatchString.Subscribe(_ => RefreshMatchStringFilter()).AddTo(disposables);
            FilterKind.Subscribe(_ => RefreshMatchStringFilter()).AddTo(disposables);
            SearchRequester.SetMatchString += SetListViewMatchString; 
                
                disposables.Add(() => {
                    SearchRequester.SetMatchString -= SetListViewMatchString;
                });
        }

        void RefreshMatchStringFilter()
        {
            var matchStrings = ListViewMatchString.Value.Split(WhiteSpaces, StringSplitOptions.RemoveEmptyEntries);
            if (matchStrings.Length == 0)
            {
                matchStringFilter = AlwaysTrueFilter;
            }
            else
            {
                matchStringFilter = item =>
                {
                    var targets = FilterKind.Value switch
                    {
                        ScriptLogFilterKind.All => new[]
                        {
                            item.TimestampString(),
                            item.DeviceId,
                            item.TypeString,
                            item.Message,
                            item.ItemIdString(),
                            item.ItemName,
                            item.PlayerId,
                            item.PlayerName,
                        },
                        ScriptLogFilterKind.Message => new[]
                        {
                            item.Message,
                        },
                        ScriptLogFilterKind.Item => new[]
                        {
                            item.ItemIdString(),
                            item.ItemName,
                        },
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    return matchStrings.All(match => targets.Any(t => t?.Contains(match, StringComparison.OrdinalIgnoreCase) ?? false));
                };
            }
            OnFilterUpdated();
        }

        public void SetSeverityFilter(bool info, bool warn, bool error)
        {
            severityFilter = entry => entry.Severity switch
            {
                ScriptLogSeverity.Unknown => true,
                ScriptLogSeverity.Error => error,
                ScriptLogSeverity.Warning => warn,
                ScriptLogSeverity.Information => info,
                ScriptLogSeverity.Dropped => true,
                _ => throw new ArgumentOutOfRangeException()
            };
            OnFilterUpdated();
        }

        public void LoadClusterScriptLogFile()
        {
            LogFileWatcherRepository.ScriptLogFilePath.Value = LogFileWatcherConstants.ClusterScriptLogFilePath;
            ListViewMatchString.Value = "";
        }

        public void LoadEditorPreviewLogFile()
        {
            LogFileWatcherRepository.ScriptLogFilePath.Value = LogFileWatcherConstants.EditorPreviewLogFilePath;
            ListViewMatchString.Value = "";
        }

        public void LoadCustomLogFile()
        {
            var dataPath = new FileInfo(LogFileWatcherRepository.ScriptLogFilePath.Value);
            var path = EditorUtility.OpenFilePanel("Load", dataPath.DirectoryName, "log");
            if (string.IsNullOrEmpty(path))
            {
                LogFileWatcherRepository.ScriptLogFilePath.Notify();
                return;
            }

            LogFileWatcherRepository.ScriptLogFilePath.Value = path;
            ListViewMatchString.Value = "";
        }

        public void PauseToggleClicked(bool newValue)
        {
            LogFileWatcherRepository.IsPaused.Value = newValue;
        }

        public void IgnoreGeneratedToggleClicked(bool newValue)
        {
            LogConsoleWindowSettingsRepository.IgnoreGenerated.Value = newValue;
        }

        internal void SetListViewMatchType(ScriptLogFilterKind scriptLogFilterKind)
        {
            if (FilterKind.Value == scriptLogFilterKind)
            {
                return;
            }
            FilterKind.Value = scriptLogFilterKind;
            OnFilterUpdated();
        }

        public void SetListViewMatchString(string matchString)
        {
            ListViewMatchString.Value = matchString;
        }

        public void SetFormatFlags(ScriptLogEntryFormatFlags formatFlags)
        {
            LogConsoleWindowSettingsRepository.FormatFlags.Value = formatFlags;
            MatchedItems.Notify();
        }

        public void SelectLogItem(ScriptLogEntry entry)
        {
            Selection.Value = entry;
            var item = ItemRepository.FindItemSlow(entry.ItemId, entry.ItemName);
            if (item != null)
            {
                EditorGUIUtility.PingObject(item.gameObject);
            }
        }

        public void OpenPosition(ScriptLogPosition position)
        {
            if (Selection.Value != null)
            {
                OpenAsset(Selection.Value, position);
            }
        }

        public void OpenLogItem(ScriptLogEntry logEntry)
        {
            OpenAsset(logEntry, StackViewModel.Items.Value.Select(item => item.StackItem).DefaultIfEmpty(ScriptLogStackItem.None).First().Position);
        }

        void OpenAsset(ScriptLogEntry logEntry, ScriptLogPosition position)
        {
            var sourceCodeAsset = ItemRepository.FindSourceCodeAssetSlow(logEntry.ItemId, logEntry.ItemName, logEntry.IsPlayerScript());
            if (sourceCodeAsset != null)
            {
                AssetOpener.OpenAsset(sourceCodeAsset, position.LineNumberOneBased, position.ColumnNumberOneBased);
            }
        }

        void OnFilterUpdated()
        {
            RefreshMatchedItems(LogRepository.ConsoleItems.Value);
        }

        void RefreshMatchedItems(List<ScriptLogEntry> items)
        {
            InfoCount.Value = items.Count(item => item.Severity is ScriptLogSeverity.Information);
            WarnCount.Value = items.Count(item => item.Severity is ScriptLogSeverity.Warning);
            ErrorCount.Value = items.Count(item => item.Severity is ScriptLogSeverity.Error);
            var list = MatchedItems.Value; 
            list.Clear();
            list.AddRange(items.Where(item => severityFilter(item) && matchStringFilter(item)));
            MatchedItems.Value = list;
        }

        public void OpenLogFile()
        {
            AssetOpener.OpenLogFile(LogFileWatcherRepository.ScriptLogFilePath.Value);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
