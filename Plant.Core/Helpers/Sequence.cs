using System;

namespace Plant.Core.Helpers
{
    public interface ISequence
    {
        Delegate Func { get; }
    }
    public class Sequence<TResult> : ISequence
    {
        public Sequence(Func<int, TResult> func)
        {
            Func = func;
        }

        public Delegate Func { get; private set; }
    }
}
