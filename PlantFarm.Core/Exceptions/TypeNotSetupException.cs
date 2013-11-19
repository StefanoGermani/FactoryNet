using System;

namespace Plant.Core.Exceptions
{
    public class TypeNotSetupException : Exception
    {
        public TypeNotSetupException(Type type) : base(string.Format("Type {0} not defined.", type.Name))
        {
        }
    }
}