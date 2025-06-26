using System;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    sealed class Disposable : IDisposable
    {
        Action action;
        public Disposable() { }
        public Disposable(Action action) => Add(action);
        public void Add(Action action) => this.action += action;
        public void Dispose()
        {
            action?.Invoke();
            action = null;
        }
    }

    static class DisposableExtension
    {
        internal static void AddTo(this IDisposable disposable, Disposable addTo) => addTo.Add(disposable.Dispose);
    }
}
