using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ClusterScriptLogConsoleWindow : EditorWindow
    {
        [MenuItem("Cluster/ClusterScript Log Console 2", priority = 341)]
        public static void ShowWindow()
        {
            GetWindow<ClusterScriptLogConsoleWindow>("ClusterScript Log Console 2").Show();
        }

        ScriptLogConsoleViewModel viewModel;
        readonly Disposable disposables = new();

        public void OnEnable()
        {
            viewModel = new();
            viewModel.AddTo(disposables);
            var logConsoleView = new ScriptLogConsoleView();
            logConsoleView.Bind(viewModel);
            logConsoleView.AddTo(disposables);
            rootVisualElement.Add(logConsoleView);
            LogFileWatcherSession.Instance.Enter();
        }

        public void OnDisable()
        {
            disposables.Dispose();
            LogFileWatcherSession.Instance.Exit();
        }
    }
}
