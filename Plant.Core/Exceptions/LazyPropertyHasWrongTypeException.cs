using System;

namespace Plant.Core.Exceptions
{
  public class LazyPropertyHasWrongTypeException : Exception
  {
    public LazyPropertyHasWrongTypeException(string message) : base(message)
    {
    }
  }
}