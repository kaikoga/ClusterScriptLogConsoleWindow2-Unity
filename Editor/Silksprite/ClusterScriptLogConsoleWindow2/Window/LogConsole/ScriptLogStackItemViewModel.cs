using ClusterVR.CreatorKit.Item.Implements;
using Silksprite.ClusterScriptLogConsoleWindow2.Data;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Window.LogConsole
{
    public sealed class ScriptLogStackItemViewModel
    {
        public readonly ScriptLogStackItem StackItem;
        public readonly bool IsGeneratedStack;
        readonly JavaScriptAsset sourceCodeAsset;

        public ScriptLogStackItemViewModel(ScriptLogStackItem stackItem, bool isGeneratedStack, JavaScriptAsset sourceCodeAsset)
        {
            StackItem = stackItem;
            IsGeneratedStack = isGeneratedStack;
            this.sourceCodeAsset = sourceCodeAsset;
        }

        public string Format() => StackItem.Format(sourceCodeAsset); 
    }
}
