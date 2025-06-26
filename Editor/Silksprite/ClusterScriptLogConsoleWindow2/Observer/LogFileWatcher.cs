using System;
using System.IO;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    public sealed class LogFileWatcher : IDisposable
    {
        public static LogFileWatcher Instance => new();

        FileSystemWatcher logFileWatcher;

        LogFileWatcherRepository LogFileWatcherRepository => LogFileWatcherRepository.Instance;

        readonly Disposable disposables = new();

        public void Bind()
        {
            LogFileWatcherRepository.ScriptLogFilePath.Subscribe(LoadAndWatchNewFile).AddTo(disposables);
            LogFileWatcherRepository.IsPaused.Subscribe(SetPaused).AddTo(disposables);
            disposables.Add(Stop);
        }

        void LoadAndWatchNewFile(string path)
        {
            Stop();
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                return;
            }
            Start(path);
        }
        
        void SetPaused(bool paused)
        {
            if (logFileWatcher != null)
            {
                logFileWatcher.EnableRaisingEvents = !paused;
            }
        }

        void Start(string filePath)
        {
#if UNITY_EDITOR_OSX
            Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "enabled");
#endif
            var fileInfo = new FileInfo(filePath);
            logFileWatcher = new FileSystemWatcher();
            logFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            logFileWatcher.Path = fileInfo.DirectoryName;
            logFileWatcher.Filter = fileInfo.Name;
            logFileWatcher.Changed += (_, _) =>
            {
                LogFileWatcherRepository.ScriptLogFileUpdateEvents.Enqueue(false);
            };
            logFileWatcher.Created += (_, _) =>
            {
                LogFileWatcherRepository.ScriptLogFileUpdateEvents.Enqueue(true);
            };
            SetPaused(LogFileWatcherRepository.IsPaused.Value);
        }

        void Stop()
        {
            if (logFileWatcher != null)
            {
                logFileWatcher.EnableRaisingEvents = false;
                logFileWatcher.Dispose();
                logFileWatcher = null;
            }
        }
        
        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
