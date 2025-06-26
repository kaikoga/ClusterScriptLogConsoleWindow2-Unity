namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    sealed class ReactiveProperty<T> : ReactivePropertyBase<T>
    {
        public ReactiveProperty(T initialValue = default) : base(initialValue) { }
    }
}
