using System.Diagnostics;
using System.IO;
using ClusterVR.CreatorKit.Item.Implements;
using ClusterVR.CreatorKit.Translation;
using Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    public static class AssetOpener
    {
        public static void OpenAsset(JavaScriptAsset sourceCodeAsset, int lineNumberOneBased, int columnNumberOneBased)
        {
            Deminifier.TryGetOriginalSourcePosition(sourceCodeAsset, ref lineNumberOneBased, ref columnNumberOneBased, out var sourceRef);
            DirectOpenAsset(sourceRef.Asset, sourceRef.Path, lineNumberOneBased, columnNumberOneBased);
        }
        
        public static void DirectOpenAsset(Object sourceAsset, string sourcePath, int lineNumberOneBased, int columnNumberOneBased)
        {
            if (sourceAsset)
            {
                AssetDatabase.OpenAsset(sourceAsset, lineNumberOneBased, columnNumberOneBased);
            }
            else
            {
                if (Path.IsPathRooted(sourcePath))
                {
                    sourcePath = Path.GetRelativePath(Path.GetDirectoryName(Application.dataPath), sourcePath);
                }
                InternalEditorUtility.OpenFileAtLineExternal(sourcePath, lineNumberOneBased, columnNumberOneBased);
            }
        }
        
        public static void OpenLogFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                EditorUtility.DisplayDialog("error", "File not found.", TranslationTable.cck_ok);
                return;
            }

            Process.Start(filePath);
        }
    }
}
