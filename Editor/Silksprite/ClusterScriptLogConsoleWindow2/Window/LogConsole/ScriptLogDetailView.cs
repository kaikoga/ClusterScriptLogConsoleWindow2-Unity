using System;
using Silksprite.ClusterScriptLogConsoleWindow2.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogDetailView : VisualElement, IDisposable
    {
        readonly TextField textField;

        readonly Disposable disposables = new();

        public ScriptLogDetailView()
        {
            textField = new TextField
            {
                multiline = true,
                isReadOnly = true,
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    flexGrow = 1f,
                    unityTextAlign = TextAnchor.UpperLeft
                }
            };
            hierarchy.Add(textField);
        }

        public void Bind(ScriptLogConsoleViewModel viewModel)
        {
            viewModel.Selection.Subscribe(entry =>
            {
                textField.value = entry?.Format() ?? string.Empty;
            }).AddTo(disposables);
        }

        public void Dispose() => disposables.Dispose();
    }
}
