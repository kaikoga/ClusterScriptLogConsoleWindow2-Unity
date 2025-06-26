using System.IO;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Format
{
    public static class LogFileWatcherConstants
    {
        const string ClusterScriptLogFileName = "ClusterScriptLog.log";
        const string EditorPreviewLogFileName = "EditorPreviewLog.log";

        public static string ClusterScriptLogFilePath =>
#if UNITY_EDITOR_OSX
            new FileInfo(Path.Combine(Application.persistentDataPath, "../../mu.cluster", ClusterScriptLogFileName)).FullName;
#else
            new FileInfo(Path.Combine(Application.persistentDataPath, @"..\..\Cluster, Inc_\cluster", ClusterScriptLogFileName)).FullName;
#endif

        public static string EditorPreviewLogFilePath =>
#if UNITY_EDITOR_OSX
            new FileInfo(Path.Combine(Application.dataPath, "../Logs", EditorPreviewLogFileName)).FullName;
#else
            new FileInfo(Path.Combine(Application.dataPath, @"..\Logs", EditorPreviewLogFileName)).FullName;
#endif
    }
}
