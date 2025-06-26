namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    abstract class ReactivePropertyBase<T> : ReadonlyReactiveProperty<T>
    {
        public new T Value
        {
            get => value;
            set
            {
                this.value = value;
                Notify();
            }
        }

        protected ReactivePropertyBase(T initialValue = default) : base(initialValue)
        {
        }

        public void Notify() => OnChanged(value);
    }
}
