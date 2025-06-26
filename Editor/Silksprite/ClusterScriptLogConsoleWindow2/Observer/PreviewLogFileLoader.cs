using JetBrains.Annotations;
using Silksprite.ClusterScriptLogConsoleWindow2.Format;
using Silksprite.ClusterScriptLogConsoleWindow2.Repository;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Observer
{
    [UsedImplicitly]
    public sealed class PreviewLogFileLoader
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        static void PlayModeStateChanged(PlayModeStateChange playMode)
        {
            switch (playMode)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    LogFileWatcherRepository.Instance.ScriptLogFilePath.Value = LogFileWatcherConstants.EditorPreviewLogFilePath;
                    break;
            }
        }
    }
}
