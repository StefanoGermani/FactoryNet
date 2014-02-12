using System;

namespace FactoryNet.Core.Exceptions
{
    public class TypeNotSetupException : Exception
    {
        public TypeNotSetupException(Type type) : base(string.Format("Type {0} not defined.", type.Name))
        {
        }
    }
}