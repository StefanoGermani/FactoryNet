using System;

namespace FactoryNet.Core.Exceptions
{
    public class ExpressionTypeNotSupportedException : Exception
    {
        public ExpressionTypeNotSupportedException() : base("The expression type of the defination is not correct")
        {
        }
    }
}