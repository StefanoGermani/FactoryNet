using System;

namespace Plant.Core.Exceptions
{
  public class TypeNotSetupException : Exception
  {
    public TypeNotSetupException(string message) : base(message)
    {
    }
  }
}