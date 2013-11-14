using System;

namespace Plant.Core.Exceptions
{
    public class WrongDefinitionTypeException : Exception
    {
        public WrongDefinitionTypeException() : base("The definition type is wrong")
        {
        }
    }
}