using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plant.Core.Exceptions
{
    public class WrongDefinitionTypeException : Exception
    {
        public WrongDefinitionTypeException() : base("The definition type is wrong") { }
    }
}
