using System;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Broker
{
    public sealed class SearchRequester
    {
        public static readonly SearchRequester Instance = new();

        public event Action<string> SetMatchString;

        public void OnSetMatchString(string value) => SetMatchString?.Invoke(value);
    }
}
