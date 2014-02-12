using System;

namespace FactoryNet.Core.Exceptions
{
    public class WrongDefinitionTypeException : Exception
    {
        public WrongDefinitionTypeException() : base("The definition type is wrong")
        {
        }
    }
}