using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorToolbarView : Toolbar, IDisposable
    {
        readonly ScriptItemInspectorSeverityFilterView severityFilterView;
        readonly EnumField itemInspectorModePopup;
        readonly EnumField itemInspectorOrderPopup;

        event Action<ScriptItemInspectorOrder> ItemInspectorOrderChanged;
        event Action<ScriptItemInspectorMode> ItemInspectorModeChanged;
        
        readonly Disposable disposables = new();

        public ScriptItemInspectorToolbarView()
        {
            itemInspectorModePopup = new EnumField(default(ScriptItemInspectorMode));
            itemInspectorModePopup.RegisterValueChangedCallback(evt => ItemInspectorModeChanged?.Invoke((ScriptItemInspectorMode)evt.newValue));
            itemInspectorOrderPopup = new EnumField(default(ScriptItemInspectorOrder));
            itemInspectorOrderPopup.RegisterValueChangedCallback(evt => ItemInspectorOrderChanged?.Invoke((ScriptItemInspectorOrder)evt.newValue));
                
            severityFilterView = new ScriptItemInspectorSeverityFilterView();

            hierarchy.Add(itemInspectorModePopup);
            hierarchy.Add(itemInspectorOrderPopup);
            hierarchy.Add(new VisualElement
            {
                style = {
                    flexGrow = 1
                }
            });
            hierarchy.Add(severityFilterView);
        }

        public void Bind(ScriptItemInspectorViewModel viewModel)
        {
            severityFilterView.Bind(viewModel);
            severityFilterView.AddTo(disposables);
            viewModel.ItemInspectorMode
                .Subscribe(value => itemInspectorModePopup.SetValueWithoutNotify(value))
                .AddTo(disposables);
            viewModel.ItemInspectorOrder
                .Subscribe(value => itemInspectorOrderPopup.SetValueWithoutNotify(value))
                .AddTo(disposables);

            ItemInspectorModeChanged += viewModel.SetItemInspectorMode;
            ItemInspectorOrderChanged += viewModel.SetItemInspectorOrder;
            disposables.Add(() =>
            {
                ItemInspectorModeChanged -= viewModel.SetItemInspectorMode;
                ItemInspectorOrderChanged -= viewModel.SetItemInspectorOrder;
            });
        }
        
        public void Dispose() => disposables.Dispose();
    }
}
