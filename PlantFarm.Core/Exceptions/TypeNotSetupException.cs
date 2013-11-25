using System;

namespace PlantFarm.Core.Exceptions
{
    public class TypeNotSetupException : Exception
    {
        public TypeNotSetupException(Type type) : base(string.Format("Type {0} not defined.", type.Name))
        {
        }
    }
}