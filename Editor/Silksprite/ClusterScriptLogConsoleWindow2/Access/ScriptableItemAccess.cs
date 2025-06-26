using System;
using System.Diagnostics.CodeAnalysis;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Access
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class ScriptableItemAccess : IDisposable
    {
        readonly SerializedObject _serializedObject;

        public JavaScriptAsset sourceCodeAsset
        {
            get
            {
                using var prop = _serializedObject.FindProperty("sourceCodeAsset");
                return (JavaScriptAsset) prop.objectReferenceValue;
            }
        }

        public string sourceCode
        {
            get
            {
                using var prop = _serializedObject.FindProperty("sourceCode");
                return prop.stringValue;
            }
        }

        public ScriptableItemAccess(ScriptableItem scriptableItem) => _serializedObject = new SerializedObject(scriptableItem);

        void IDisposable.Dispose() => _serializedObject.ApplyModifiedProperties();
    }
}