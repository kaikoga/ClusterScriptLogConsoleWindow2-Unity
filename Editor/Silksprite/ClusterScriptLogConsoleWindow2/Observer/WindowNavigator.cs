using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Broker;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    public sealed class WindowNavigator : IDisposable
    {
        public static WindowNavigator Instance => new();

        readonly Disposable disposables = new();

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Instance.Bind();
        }

        public void Bind()
        {
            WindowNavigationRequester.Instance.OpenLogConsoleWindow += OnOpenLogConsoleWindow;
            disposables.Add(() =>
            {
                WindowNavigationRequester.Instance.OpenLogConsoleWindow += OnOpenLogConsoleWindow;
            });
        }

        void OnOpenLogConsoleWindow()
        {
            ClusterScriptLogConsoleWindow.ShowWindow();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
