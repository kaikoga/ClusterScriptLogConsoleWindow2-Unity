using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSourceItemView : VisualElement
    {
        readonly Label label;

        public ScriptLogSourceItemView()
        {
            label = new Label();
            hierarchy.Add(label);
        }

        public void Bind(ScriptLogSourceItemViewModel viewModel)
        {
            label.text = viewModel.Format();
        }
    }
}
