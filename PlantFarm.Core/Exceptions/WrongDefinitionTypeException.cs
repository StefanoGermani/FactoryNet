using System;

namespace PlantFarm.Core.Exceptions
{
    public class WrongDefinitionTypeException : Exception
    {
        public WrongDefinitionTypeException() : base("The definition type is wrong")
        {
        }
    }
}