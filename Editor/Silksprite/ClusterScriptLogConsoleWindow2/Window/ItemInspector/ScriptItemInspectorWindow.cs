using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.ItemInspector
{
    public sealed class ItemInspectorWindow : EditorWindow
    {
        [MenuItem("Cluster/ClusterScript Log Console 2 Utils/ClusterScript Item Inspector", priority = 342)]
        static void ShowWindow()
        {
            GetWindow<ItemInspectorWindow>("ClusterScript Item Inspector").Show();
        }

        ScriptItemInspectorViewModel viewModel;
        readonly Disposable disposables = new();

        public void OnEnable()
        {
            viewModel = new();
            viewModel.AddTo(disposables);
            var itemInspectorView = new ScriptItemInspectorView();
            itemInspectorView.Bind(viewModel);
            itemInspectorView.AddTo(disposables);
            rootVisualElement.Add(itemInspectorView);
            LogFileWatcherSession.Instance.Enter();
        }

        public void OnDisable()
        {
            disposables.Dispose();
            LogFileWatcherSession.Instance.Exit();
        }
    }
}
