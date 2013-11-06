using System;

namespace Plant.Core.Exceptions
{
    public class ConstructorNotFoundException : Exception
    {
        public ConstructorNotFoundException(int paramsCount) :
            base(string.Format("Constructor with {0} parameters not found", paramsCount))
        {
            ParamsCount = paramsCount;

        }

        public int ParamsCount { get; set; }
    }
}