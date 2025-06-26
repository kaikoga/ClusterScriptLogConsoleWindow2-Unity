using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using Silksprite.ClusterScriptLogConsoleWindow2.Window.Components;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorSeverityFilterView : VisualElement, IDisposable
    {
        readonly SeverityFilter severityFilter;
            
        readonly Disposable disposables = new();

        public ScriptItemInspectorSeverityFilterView()
        {
            severityFilter = new SeverityFilter();
            hierarchy.Add(severityFilter);
        }

        public void Bind(ScriptItemInspectorViewModel viewModel)
        {
            severityFilter.SeverityChanged += viewModel.SetSeverityFilter;

            disposables.Add(() =>
            {
                severityFilter.SeverityChanged -= viewModel.SetSeverityFilter;
            });
        }

        public void Dispose() => disposables.Dispose();
    }
}
