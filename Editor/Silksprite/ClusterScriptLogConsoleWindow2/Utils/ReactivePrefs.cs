using System;
using UnityEditor;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    sealed class ReactiveEditorPrefsEnum<T> : ReactivePropertyBase<T>
        where T : struct, Enum
    {
        public ReactiveEditorPrefsEnum(string key, T defaultValue) : base(InitialValue(key, defaultValue))
        {
            Subscribe(value => EditorPrefs.SetString(key, value.ToString()));
        }
        
        static T InitialValue(string key, T defaultValue)
        {
            return Enum.TryParse<T>(EditorPrefs.GetString(key, ""), out var v) ? v : defaultValue;
        }
    }

    sealed class ReactiveEditorPrefsFloat : ReactivePropertyBase<float>
    {
        public ReactiveEditorPrefsFloat(string key, float defaultValue) : base(InitialValue(key, defaultValue))
        {
            Subscribe(value => EditorPrefs.SetFloat(key, value));
        }

        static float InitialValue(string key, float defaultValue)
        {
            return EditorPrefs.GetFloat(key, defaultValue);
        }
    }

    sealed class ReactiveEditorPrefsBool : ReactivePropertyBase<bool>
    {
        public ReactiveEditorPrefsBool(string key, bool defaultValue) : base(InitialValue(key, defaultValue))
        {
            Subscribe(value => EditorPrefs.SetBool(key, value));
        }
        
        static bool InitialValue(string key, bool defaultValue)
        {
            return EditorPrefs.GetBool(key, defaultValue);
        }
    }

    sealed class ReactivePlayerPrefsString : ReactivePropertyBase<string>
    {
        public ReactivePlayerPrefsString(string key, string defaultValue) : base(InitialValue(key, defaultValue))
        {
            Subscribe(value => PlayerPrefs.SetString(key, value.ToString()));
        }

        static string InitialValue(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
    }
}
