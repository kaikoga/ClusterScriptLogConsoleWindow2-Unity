using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
using SourcemapToolkit.SourcemapParser;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport
{
    public static class SourceMapRepository
    {
        static Dictionary<string, SourceMap> cache = new();

        public static bool TryGetSourceMap(JavaScriptAsset sourceCodeAsset, out string sourceCodePath, out SourceMap sourceMap)
        {
            if (sourceCodeAsset == null)
            {
                sourceCodePath = null;
                sourceMap = null;
                return false;
            }
            sourceCodePath = AssetDatabase.GetAssetPath(sourceCodeAsset);
            if (string.IsNullOrEmpty(sourceCodePath))
            {
                sourceMap = null;
                return false;
            }
            if (!cache.TryGetValue(sourceCodePath, out sourceMap))
            {
                sourceMap = LoadSourceMap(sourceCodePath);
                cache.Add(sourceCodePath, sourceMap);
            }
            return sourceMap != null;
        }

        static SourceMap LoadSourceMap(string sourceCodePath)
        {
            return SourcemapPaths()
                .Where(File.Exists)
                .Select(sourcemapPath => new SourceMapParser().ParseSourceMap(File.OpenText(sourcemapPath)))
                .FirstOrDefault();

            IEnumerable<string> SourcemapPaths()
            {
                yield return $"{sourceCodePath}.map";
                var guidSourcemaps = AssetDatabase.FindAssets($"{AssetDatabase.AssetPathToGUID(sourceCodePath)}.map")
                    .Select(AssetDatabase.GUIDToAssetPath);
                foreach (var guidSourcemap in guidSourcemaps)
                {
                    yield return guidSourcemap;
                }
            }
        }

        class SourceMapCacheInvalidator : AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                var assets = new HashSet<string>();
                assets.UnionWith(importedAssets);
                assets.UnionWith(deletedAssets);
                assets.UnionWith(movedAssets);
                assets.UnionWith(movedFromAssetPaths);
                foreach (var asset in assets)
                {
                    cache.Remove(asset);
                }
            }
        }

    }
}
