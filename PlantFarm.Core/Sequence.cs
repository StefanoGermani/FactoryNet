using System;

namespace Plant.Core
{
    public class Sequence
    {
        public static T Evaluate<T>(Func<int, T> func)
        {
            throw new NotSupportedException(
                "This method is a place holder to get the function and should never be executed");
        }
    }
}