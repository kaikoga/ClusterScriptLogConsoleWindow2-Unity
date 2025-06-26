using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    public sealed class LogFileReader : IDisposable
    {
        public static LogFileReader Instance => new();

        LogFileReaderMode mode;

        LogRepository LogRepository => LogRepository.Instance;
        LogFileWatcherRepository LogFileWatcherRepository => LogFileWatcherRepository.Instance;
        readonly Disposable disposables = new();
        CancellationTokenSource cancellationTokenSource = new();

        public void Bind()
        {
            LogFileWatcherRepository.ScriptLogFilePath.Subscribe(LoadAndWatchNewFile).AddTo(disposables);
        }

        void LoadAndWatchNewFile(string filePath)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            ResetLogs();
            _ = ReadLogFileAsync(filePath, cancellationTokenSource.Token);
        }
        
        async Task ReadLogFileAsync(string filePath, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (LogFileWatcherRepository.ScriptLogFileUpdateEvents.TryDequeue(out var discard))
                {
                    if (discard)
                    {
                        ResetLogs();
                    }
                }
                ReadLogFile(filePath);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                while (LogFileWatcherRepository.ScriptLogFileUpdateEvents.IsEmpty)
                {
                   await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        void ResetLogs()
        {
            LogFileWatcherRepository.LogFilePosition.Value = 0;
            LogRepository.DiscardLogs();
            mode = LogFileReaderMode.Unknown;
        }

        void ReadLogFile(string filePath)
        {
            var logEntries = new List<ScriptLogEntry>();
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek(LogFileWatcherRepository.LogFilePosition.Value, SeekOrigin.Begin);
                using var stream = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                while (!stream.EndOfStream)
                {
                    var text = stream.ReadLine();
                    LogFileWatcherRepository.LogFilePosition.Value = fs.Position;
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }

                    try
                    {
                        switch (mode)
                        {
                            case LogFileReaderMode.Unknown:
                            default:
                            {
                                var output = JsonUtility.FromJson<OutputScriptableItemLogExt>(text);
                                if (!string.IsNullOrEmpty(output.type))
                                {
                                    var item = OutputScriptLogEntryConverter.ToLogEntry(output);
                                    logEntries.Add(item);
                                }
                                else
                                {
                                    var output2 = JsonUtility.FromJson<SerializableScriptLogEntry>(text);
                                    var item = SerializableScriptLogEntryConverter.ToLogEntry(output2);
                                    logEntries.Add(item);
                                }
                                break;
                            }
                            case LogFileReaderMode.OutputScriptableItemLogExt:
                            {
                                var output = JsonUtility.FromJson<OutputScriptableItemLogExt>(text);
                                var item = OutputScriptLogEntryConverter.ToLogEntry(output);
                                logEntries.Add(item);
                                break;
                            }
                            case LogFileReaderMode.SerializableScriptLogEntry:
                            {
                                var output = JsonUtility.FromJson<SerializableScriptLogEntry>(text);
                                var item = SerializableScriptLogEntryConverter.ToLogEntry(output);
                                logEntries.Add(item);
                                break;
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        var item = new ScriptLogEntry
                        {
                            DeviceId = "",
                            Timestamp = DateTimeOffset.Now,
                            Message = text + "\n" + ex.Message,
                            Type = ScriptLogEntryType.Error,
                            TypeString = "Error"
                        };
                        logEntries.Add(item);
                        throw;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            
            LogRepository.AddLogs(logEntries.ToArray());
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            disposables.Dispose();
        }

        enum LogFileReaderMode
        {
            Unknown,
            OutputScriptableItemLogExt,
            SerializableScriptLogEntry,
        }
    }
}
