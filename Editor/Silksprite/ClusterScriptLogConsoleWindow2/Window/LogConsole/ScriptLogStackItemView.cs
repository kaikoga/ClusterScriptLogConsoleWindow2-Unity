using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogStackItemView : VisualElement
    {
        readonly Label label;

        public ScriptLogStackItemView()
        {
            label = new Label();
            hierarchy.Add(label);
        }

        public void Bind(ScriptLogStackItemViewModel viewModel)
        {
            label.text = viewModel.Format();
            label.style.color = viewModel.IsGeneratedStack ? Color.gray : EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }
    }
}
