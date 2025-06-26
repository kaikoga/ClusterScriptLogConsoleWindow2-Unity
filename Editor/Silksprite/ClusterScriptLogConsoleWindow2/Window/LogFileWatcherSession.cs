using Silksprite.ClusterScriptLogConsoleWindow2.Observer;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window
{
    public sealed class LogFileWatcherSession
    {
        public static LogFileWatcherSession Instance => new();

        int counter;

        readonly Disposable disposables = new();

        public void Enter()
        {
            if (counter++ == 0)
            {
                Start();
            }
        }

        public void Exit()
        {
            if (--counter == 0)
            {
                Stop();
            }
        }

        void Start()
        {
            LogFileWatcher.Instance.Bind();
            LogFileWatcher.Instance.AddTo(disposables);
            LogFileReader.Instance.Bind();
            LogFileReader.Instance.AddTo(disposables);
            ItemCacheInvalidator.Instance.Bind();
            ItemCacheInvalidator.Instance.AddTo(disposables);
        }

        void Stop()
        {
            ItemRepository.Instance.ClearCache();
            disposables.Dispose();
        }
    }
}
