using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogEntryView : VisualElement
    {
        readonly Image image;
        readonly Label label;

        public ScriptLogEntryView()
        {
            style.flexDirection = FlexDirection.Row;

            image = new Image();
            hierarchy.Add(image);

            label = new Label
            {
                style =
                {
                    overflow = Overflow.Hidden,
                    flexGrow = 1,
                    flexShrink = 1,
                }
            };
            hierarchy.Add(label);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel, ScriptLogEntry logEntry)
        {
            label.text = logEntry.Format(viewModel.FormatFlags.Value);

            image.style.flexShrink = 0;

            switch (logEntry.Severity)
            {
                case ScriptLogSeverity.Error:
                    label.style.color = Color.red;
                    image.image = UnityEditorIconLocator.ErrorIcon().image;
                    break;
                case ScriptLogSeverity.Warning:
                    label.style.color = Color.yellow;
                    image.image = UnityEditorIconLocator.WarnIcon().image;
                    break;
                case ScriptLogSeverity.Information:
                    label.style.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    image.image = UnityEditorIconLocator.InfoIcon().image;
                    break;
                case ScriptLogSeverity.Dropped:
                    label.style.color = Color.gray;
                    image.image = UnityEditorIconLocator.InfoIcon().image;
                    break;
                case ScriptLogSeverity.Unknown:
                default:
                    label.style.color = Color.gray;
                    image.image = UnityEditorIconLocator.ErrorIcon().image;
                    break;
            }
        }

    }
}
