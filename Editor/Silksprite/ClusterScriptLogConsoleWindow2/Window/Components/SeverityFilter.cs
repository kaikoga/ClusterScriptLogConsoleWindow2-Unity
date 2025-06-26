using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.Components
{
    public sealed class SeverityFilter : VisualElement
    {
        readonly Toggle infoToggle;
        readonly Toggle warnToggle;
        readonly Toggle errorToggle;

        public event Action<bool, bool, bool> SeverityChanged;

        public SeverityFilter()
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 0;
            style.flexShrink = 0;
            infoToggle = new EditorToolbarToggle
            {
                icon = (Texture2D) UnityEditorIconLocator.InfoIconSmall().image,
                value = true
            };
            warnToggle = new EditorToolbarToggle
            {
                icon = (Texture2D) UnityEditorIconLocator.WarnIconSmall().image,
                value = true
            };
            errorToggle = new EditorToolbarToggle
            {
                icon = (Texture2D) UnityEditorIconLocator.ErrorIconSmall().image,
                value = true
            };
            infoToggle.RegisterValueChangedCallback(_ => OnSeverityToggleClicked());
            warnToggle.RegisterValueChangedCallback(_ => OnSeverityToggleClicked());
            errorToggle.RegisterValueChangedCallback(_ => OnSeverityToggleClicked());

            void OnSeverityToggleClicked()
            {
                SeverityChanged?.Invoke(infoToggle.value, warnToggle.value, errorToggle.value);
            }
            hierarchy.Add(infoToggle);
            hierarchy.Add(warnToggle);
            hierarchy.Add(errorToggle);
        }
        
        public void SetInfoCount(int count) => infoToggle.text = count.ToString();
        public void SetWarnCount(int count) => warnToggle.text = count.ToString();
        public void SetErrorCount(int count) => errorToggle.text = count.ToString();
    }
}
