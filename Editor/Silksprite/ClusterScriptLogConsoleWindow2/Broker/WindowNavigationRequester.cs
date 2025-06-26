using System;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Broker
{
    public sealed class WindowNavigationRequester
    {
        public static readonly WindowNavigationRequester Instance = new();

        public event Action OpenLogConsoleWindow;

        public void OnOpenLogConsoleWindow() => OpenLogConsoleWindow?.Invoke();
    }
}
