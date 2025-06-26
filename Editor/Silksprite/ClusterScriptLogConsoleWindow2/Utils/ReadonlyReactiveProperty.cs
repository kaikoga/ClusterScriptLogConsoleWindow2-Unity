using System;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Utils
{
    abstract class ReadonlyReactiveProperty<T>
    {
        protected T value;
        public T Value => value;

        protected event Action<T> Changed;
        protected void OnChanged(T obj) => Changed?.Invoke(obj);

        protected ReadonlyReactiveProperty(T initialValue = default) => value = initialValue;

        public IDisposable Subscribe(Action<T> handler)
        {
            Changed += handler;
            handler(value);
            return new Disposable(() => Changed -= handler);
        }

    }
}
