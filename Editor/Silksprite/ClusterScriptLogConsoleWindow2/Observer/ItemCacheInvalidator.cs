using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    public sealed class ItemCacheInvalidator : IDisposable
    {
        public static ItemCacheInvalidator Instance => new();

        readonly Disposable disposables = new();

        public void Bind()
        {
            LogFileWatcherRepository.Instance.ScriptLogFilePath.Subscribe(_ => ClearItemCache()).AddTo(disposables);
            LogRepository.Instance.LogsDiscarded += ClearItemCache;
            disposables.Add(() =>
            {
                LogRepository.Instance.LogsDiscarded -= ClearItemCache;
            });
        }

        void ClearItemCache()
        {
            ItemRepository.Instance.ClearCache();
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
