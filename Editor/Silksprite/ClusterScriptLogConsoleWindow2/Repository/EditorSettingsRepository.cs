using System.Linq;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class EditorSettingsRepository
    {
        public static readonly EditorSettingsRepository Instance = new();

        public bool GenerateJsInProject
        {
            get => EditorSettings.projectGenerationUserExtensions.Contains("js");
            set
            {
                if (value)
                {
                    EditorSettings.projectGenerationUserExtensions = EditorSettings.projectGenerationUserExtensions
                        .Concat(new[] { "js" })
                        .Distinct().ToArray();
                }
                else
                {
                    EditorSettings.projectGenerationUserExtensions = EditorSettings.projectGenerationUserExtensions
                        .Where(ex => ex != "js")
                        .ToArray();
                }
            }
        }
    }
}
