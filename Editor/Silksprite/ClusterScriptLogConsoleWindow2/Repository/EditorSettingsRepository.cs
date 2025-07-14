using System.Linq;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Repository
{
    public sealed class EditorSettingsRepository
    {
        public static readonly EditorSettingsRepository Instance = new();

        static readonly string[] JsExtensions = { "js", "ts" }; 

        public bool GenerateJsInProject
        {
            get => JsExtensions.All(ext => EditorSettings.projectGenerationUserExtensions.Contains(ext));
            set
            {
                if (value)
                {
                    EditorSettings.projectGenerationUserExtensions = EditorSettings.projectGenerationUserExtensions
                        .Concat(JsExtensions)
                        .Distinct().ToArray();
                }
                else
                {
                    EditorSettings.projectGenerationUserExtensions = EditorSettings.projectGenerationUserExtensions
                        .Where(ext => !JsExtensions.Contains(ext))
                        .ToArray();
                }
            }
        }
    }
}
