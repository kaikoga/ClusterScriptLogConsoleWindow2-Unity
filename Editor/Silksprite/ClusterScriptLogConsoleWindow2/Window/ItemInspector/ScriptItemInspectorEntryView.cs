using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ScriptItemInspectorEntryView : VisualElement, IDisposable
    {
        readonly Label label;
        readonly Image info;
        readonly Image warn;
        readonly Image error;

        public ScriptItemInspectorEntryView()
        {
            style.flexDirection = FlexDirection.Row;
            label = new Label
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1
                }
            };
            info = new Image
            {
                image = UnityEditorIconLocator.InfoIconSmall().image
            };
            warn = new Image
            {
                image = UnityEditorIconLocator.WarnIconSmall().image
            };
            error = new Image
            {
                image = UnityEditorIconLocator.ErrorIconSmall().image
            };
            hierarchy.Add(label);
            hierarchy.Add(info);
            hierarchy.Add(warn);
            hierarchy.Add(error);
        }

        public void Bind(ScriptItemInspectorEntryViewModel viewModel)
        {
            label.text = viewModel.Format();
            info.style.display = viewModel.Info ? DisplayStyle.Flex : DisplayStyle.None;
            warn.style.display = viewModel.Warn ? DisplayStyle.Flex : DisplayStyle.None;
            error.style.display = viewModel.Error ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void Dispose()
        {
        }
    }
}
