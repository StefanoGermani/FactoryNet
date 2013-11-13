using System;

namespace Plant.Core.Helpers
{
    public interface ILazyProperty
    {
        Delegate Func { get; }
    }

    public class LazyProperty<TResult> : ILazyProperty
    {
        public Delegate Func { get; private set; }

        public LazyProperty(Func<TResult> func)
        {
            Func = func;
        }
    }
}