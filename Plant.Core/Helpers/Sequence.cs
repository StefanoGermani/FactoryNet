using System;

namespace Plant.Core.Helpers
{
    public class Sequence
    {
        public static T Evaluate<T>(Func<object, T> func)
        {
            throw new NotSupportedException("This method is a place holder to get the function and should never be executed");
        }
    }
}
