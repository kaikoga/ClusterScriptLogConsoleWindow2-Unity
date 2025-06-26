using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClusterVR.CreatorKit.Item.Implements;
using SourcemapToolkit.SourcemapParser;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.ClusterScriptLogConsoleWindow2.SourcemapSupport
{
    public record JavaScriptSourceReference
    {
        public readonly Object Asset;
        public readonly string Path;

        public JavaScriptSourceReference(Object asset, string path)
        {
            Asset = asset;
            Path = path;
        }

        public JavaScriptSourceReference(Object asset)
        {
            Asset = asset;
            Path = AssetDatabase.GetAssetPath(asset);
        }
    }

    public static class Deminifier
    {
        public static IEnumerable<JavaScriptSourceReference> CollectOriginalSources(JavaScriptAsset sourceCodeAsset)
        {
            if (!SourceMapRepository.TryGetSourceMap(sourceCodeAsset, out var sourceCodePath, out var sourcemap))
            {
                yield return new JavaScriptSourceReference(sourceCodeAsset);
                yield break;
            }

            foreach (var source in sourcemap.Sources)
            {
                yield return ResolveOriginalAsset(sourceCodePath, source);
            }
        }

        public static bool TryGetOriginalSourcePosition(JavaScriptAsset sourceCodeAsset, ref int lineNumberOneBased, ref int columnNumberOneBased, out JavaScriptSourceReference sourceRef)
        {
            if (!SourceMapRepository.TryGetSourceMap(sourceCodeAsset, out var sourceCodePath, out var sourcemap))
            {
                sourceRef = new JavaScriptSourceReference(sourceCodeAsset);
                return false;
            }
            var sourcePosition = new SourcePosition
            {
                ZeroBasedLineNumber = lineNumberOneBased - 1,
                ZeroBasedColumnNumber = columnNumberOneBased - 1
            };
            var mappingEntry = GetMappingEntryForGeneratedSourcePosition(sourcemap, sourcePosition);
            if (mappingEntry?.OriginalSourcePosition == null)
            {
                sourceRef = new JavaScriptSourceReference(sourceCodeAsset);
                return false;
            }
            
            sourceRef = ResolveOriginalAsset(sourceCodePath, mappingEntry.OriginalFileName);
            lineNumberOneBased = mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber + 1;
            // Ignore column number if it doesn't seem to be generated
            if (sourcemap.ParsedMappings.Any(mapping => mapping.OriginalSourcePosition is { ZeroBasedColumnNumber: > 0 }))
            {
                columnNumberOneBased = mappingEntry.OriginalSourcePosition.ZeroBasedColumnNumber + 1;
            }
            return true;
        }

        static JavaScriptSourceReference ResolveOriginalAsset(string sourceCodePath, string originalFileName)
        {
            // match by relative path from sourcemap
            if (originalFileName.StartsWith("file:"))
            {
                originalFileName = originalFileName.Remove(0, "file://".Length);
            }
            if (Path.IsPathRooted(originalFileName))
            {
                return new JavaScriptSourceReference(null, originalFileName);
            }
            var originalAssetPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceCodePath) ?? "", originalFileName));
            var projectPath = Path.GetFullPath("..", Application.dataPath);
            var relativeAssetPath = Path.GetRelativePath(projectPath, Path.GetFullPath(originalAssetPath)); 
            var originalAsset = AssetDatabase.LoadAssetAtPath<Object>(relativeAssetPath); 
            if (originalAsset != null)
            {
                return new JavaScriptSourceReference(originalAsset, relativeAssetPath);
            }
            // match by filename
            var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var guid = AssetDatabase.FindAssets($"t:JavaScriptAsset {originalFileNameWithoutExtension}")
                    .Concat(AssetDatabase.FindAssets(originalFileNameWithoutExtension))
                    .FirstOrDefault();
            if (guid != null)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<JavaScriptAsset>(assetPath);
                if (asset != null)
                {
                    return new JavaScriptSourceReference(asset, assetPath);
                }
            }
            // fail 
            return new JavaScriptSourceReference(null, relativeAssetPath);
        }
        
        /// <summary>
        /// Copy of SourceMap.GetMappingEntryForGeneratedSourcePosition(), but very lenient
        /// </summary>
        static MappingEntry GetMappingEntryForGeneratedSourcePosition(SourceMap sourceMap, SourcePosition generatedSourcePosition)
        {
            if (sourceMap.ParsedMappings == null)
            {
                return null;
            }

            var mappingEntryToFind = new MappingEntry
            {
                GeneratedSourcePosition = generatedSourcePosition                
            };

            var index = sourceMap.ParsedMappings.BinarySearch(mappingEntryToFind, Comparer);

            // If we didn't get an exact match, let's try to return the closest piece of code to the given line
            if (index < 0)
            {
                // The BinarySearch method returns the bitwise complement of the nearest element that is larger than the desired element when there isn't a match.
                // Based on tests with source maps generated with the Closure Compiler, we should consider the closest source position that is smaller than the target value when we don't have a match.
                var correctIndex = ~index - 1;

                // if (correctIndex >= 0 && sourceMap.ParsedMappings[correctIndex].GeneratedSourcePosition.IsEqualish(generatedSourcePosition))
                // {
                //     index = correctIndex;
                // }
                index = correctIndex;
            }

            return index >= 0 ? sourceMap.ParsedMappings[index] : null;
        }
        static readonly Comparer<MappingEntry> Comparer = Comparer<MappingEntry>.Create((a, b) => a.GeneratedSourcePosition.CompareTo(b.GeneratedSourcePosition));
    }
}
