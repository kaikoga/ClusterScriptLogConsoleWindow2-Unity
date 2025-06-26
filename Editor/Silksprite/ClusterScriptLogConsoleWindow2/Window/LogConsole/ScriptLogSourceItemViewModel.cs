using System.IO;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogSourceItemViewModel
    {
        public readonly Object SourceAsset;
        public readonly string SourcePath;
        
        readonly string displaySourcePath;

        public ScriptLogSourceItemViewModel(Object sourceAsset, string sourcePath)
        {
            SourceAsset = sourceAsset;
            SourcePath = sourcePath;
            if (string.IsNullOrEmpty(sourcePath))
            {
                displaySourcePath = "(inline script)";   
            }
            else
            {
                displaySourcePath = $"{Path.GetFileName(sourcePath)} ({Path.GetDirectoryName(sourcePath)})";
            }
        }

        public string Format() => displaySourcePath;
    }
}
