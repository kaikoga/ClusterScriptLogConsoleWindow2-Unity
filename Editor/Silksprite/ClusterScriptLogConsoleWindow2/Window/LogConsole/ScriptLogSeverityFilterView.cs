using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using Silksprite.ClusterScriptLogConsoleWindow2.Window.Components;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSeverityFilterView : VisualElement, IDisposable
    {
        readonly SeverityFilter severityFilter;
            
        readonly Disposable disposables = new();

        public ScriptLogSeverityFilterView()
        {
            style.flexGrow = 0;
            style.flexShrink = 0;
            severityFilter = new SeverityFilter();
            hierarchy.Add(severityFilter);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            viewModel.InfoCount
                .Subscribe(severityFilter.SetInfoCount)
                .AddTo(disposables);
            viewModel.WarnCount
                .Subscribe(severityFilter.SetWarnCount)
                .AddTo(disposables);
            viewModel.ErrorCount
                .Subscribe(severityFilter.SetErrorCount)
                .AddTo(disposables);

            severityFilter.SeverityChanged += viewModel.SetSeverityFilter;

            disposables.Add(() =>
            {
                severityFilter.SeverityChanged -= viewModel.SetSeverityFilter;
            });
        }

        public void Dispose() => disposables.Dispose();
    }
}
