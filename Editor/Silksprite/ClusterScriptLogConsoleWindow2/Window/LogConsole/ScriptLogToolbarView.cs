using System;
using System.Linq;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogToolbarView : Toolbar, IDisposable
    {
        readonly Disposable disposables = new();

        readonly ScriptLogSearchFieldView searchField;
        readonly Toggle pauseToggle;
        readonly Toggle hideGeneratedToggle;
        readonly PopupField<SelectLogFilePopupItem> selectLogFilePopup;
        readonly EnumFlagsField formatPopup;
        readonly Toggle infoToggle;
        readonly ScriptLogSeverityFilterView severityFilters;

        LogRepository LogRepository => LogRepository.Instance;
        EditorSettingsRepository EditorSettingsRepository => EditorSettingsRepository.Instance;

        event Action<bool> PauseToggleClicked;
        event Action<bool> IgnoreGeneratedToggleClicked;
        event Action LoadClusterScriptLogFile;
        event Action LoadEditorPreviewLogFile;
        event Action LoadCustomLogFile;
        event Action<ScriptLogEntryFormatFlags> FormatChanged;
        event Action OpenLogFileClicked;

        public ScriptLogToolbarView()
        {
            searchField = new ScriptLogSearchFieldView
            {
                style =
                {
                    flexGrow = 1
                }
            };

            var clearLogButton = new Button
            {
                text = "Clear"
            };
            clearLogButton.clicked += ClearLog;
            var loadItem = new SelectLogFilePopupItem("Load");
            selectLogFilePopup = new PopupField<SelectLogFilePopupItem>
            {
                value = loadItem,
                choices = 
                {
                    new("ClusterScript",
                        () => LoadClusterScriptLogFile?.Invoke(),
                        path => path == LogFileWatcherConstants.ClusterScriptLogFilePath),
                    new("Editor Preview",
                        () => LoadEditorPreviewLogFile?.Invoke(),
                        path => path == LogFileWatcherConstants.EditorPreviewLogFilePath),
                    new(""),
                    new("Custom...",
                        () => LoadCustomLogFile?.Invoke(),
                        path => path != LogFileWatcherConstants.ClusterScriptLogFilePath && path != LogFileWatcherConstants.EditorPreviewLogFilePath)
                },
                formatListItemCallback = item => item.Format(),
                formatSelectedValueCallback = item => item.Format(),
            };
            selectLogFilePopup.RegisterValueChangedCallback(evt =>
            {
                evt.newValue?.Invoke();
            });
            var openLogFileButton = new Button
            {
                text = "Open Log"
            };
            openLogFileButton.clicked += () => OpenLogFileClicked?.Invoke();

            pauseToggle = new Toggle
            {
                text = "Pause Log"
            };
            pauseToggle.RegisterValueChangedCallback(evt => PauseToggleClicked?.Invoke(evt.newValue));

            hideGeneratedToggle = new Toggle
            {
                text = "Hide Generated Stack"
            };
            hideGeneratedToggle.RegisterValueChangedCallback(evt => IgnoreGeneratedToggleClicked?.Invoke(evt.newValue));

            formatPopup = new EnumFlagsField((ScriptLogEntryFormatDisplayFlags) 0);
            formatPopup.RegisterValueChangedCallback(evt => FormatChanged?.Invoke((ScriptLogEntryFormatFlags)evt.newValue));

            severityFilters = new ScriptLogSeverityFilterView();

            hierarchy.Add(clearLogButton);
            hierarchy.Add(selectLogFilePopup);
            hierarchy.Add(searchField);
            hierarchy.Add(formatPopup);
            hierarchy.Add(pauseToggle);
            hierarchy.Add(openLogFileButton);
            hierarchy.Add(hideGeneratedToggle);
            if (!EditorSettingsRepository.GenerateJsInProject)
            {
                var button = new Button
                {
                    text = "Open .js in C# Project",
                };
                button.clicked += () =>
                {
                    EditorSettingsRepository.GenerateJsInProject = true;
                    button.style.display = DisplayStyle.None;
                };
                hierarchy.Add(button);
            }
            hierarchy.Add(severityFilters);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            searchField.Bind(viewModel);
            searchField.AddTo(disposables);
            severityFilters.Bind(viewModel);
            severityFilters.AddTo(disposables);
            
            viewModel.StackViewModel.HasGeneratedStackItems
                .Subscribe(value => hideGeneratedToggle.style.display = value ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(disposables);

            viewModel.IsPaused
                .Subscribe(paused => pauseToggle.SetValueWithoutNotify(paused))
                .AddTo(disposables);
            viewModel.ScriptLogFilePath
                .Subscribe(logPath => selectLogFilePopup.SetValueWithoutNotify(
                    selectLogFilePopup.choices.First(choice => choice.IsSelected(logPath))
                ))
                .AddTo(disposables);

            PauseToggleClicked += viewModel.PauseToggleClicked;
            IgnoreGeneratedToggleClicked += viewModel.IgnoreGeneratedToggleClicked;
            FormatChanged += viewModel.SetFormatFlags;
            LoadClusterScriptLogFile += viewModel.LoadClusterScriptLogFile;
            LoadEditorPreviewLogFile += viewModel.LoadEditorPreviewLogFile;
            LoadCustomLogFile += viewModel.LoadCustomLogFile;
            OpenLogFileClicked += viewModel.OpenLogFile;

            formatPopup.SetValueWithoutNotify(viewModel.FormatFlags.Value);

            disposables.Add(() =>
            {
                PauseToggleClicked -= viewModel.PauseToggleClicked;
                IgnoreGeneratedToggleClicked -= viewModel.IgnoreGeneratedToggleClicked;
                FormatChanged -= viewModel.SetFormatFlags;
                LoadClusterScriptLogFile -= viewModel.LoadClusterScriptLogFile;
                LoadEditorPreviewLogFile -= viewModel.LoadEditorPreviewLogFile;
                LoadCustomLogFile -= viewModel.LoadCustomLogFile;
                OpenLogFileClicked -= viewModel.OpenLogFile;
            });
        }

        void ClearLog()
        {
            LogRepository.DiscardLogs();
            searchField.ClearSearchField();
        }
        
        public void Dispose() => disposables.Dispose();

        class SelectLogFilePopupItem
        {
            readonly string stringValue;
            readonly Action handler;
            readonly Func<string, bool> selectedHandler;

            public SelectLogFilePopupItem(string stringValue, Action handler = null, Func<string, bool> selectedHandler = null)
            {
                this.stringValue = stringValue;
                this.handler = handler;
                this.selectedHandler = selectedHandler;
            }
            public void Invoke() => handler?.Invoke();
            public bool IsSelected(string logPath) => selectedHandler?.Invoke(logPath) ?? false;
            public string Format() => stringValue;
        }
    }
}
