using UnityEditor;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    public static class UnityEditorIconLocator
    {
        public static GUIContent InfoIcon() => EditorGUIUtility.IconContent("icons/console.infoicon.png");
        public static GUIContent WarnIcon() => EditorGUIUtility.IconContent("icons/console.warnicon.png");
        public static GUIContent ErrorIcon() => EditorGUIUtility.IconContent("icons/d_console.erroricon.png");
        public static GUIContent InfoIconSmall() => EditorGUIUtility.IconContent("icons/console.infoicon.sml.png");
        public static GUIContent WarnIconSmall() => EditorGUIUtility.IconContent("icons/console.warnicon.sml.png");
        public static GUIContent ErrorIconSmall() => EditorGUIUtility.IconContent("icons/d_console.erroricon.sml.png");
    }
}