using System;
using System.Runtime.Serialization;

namespace Plant.Core.Exceptions
{
    public class DuplicateBlueprintException : Exception
    {
        public DuplicateBlueprintException()
        {
        }

        public DuplicateBlueprintException(string message)
            : base(message)
        {
        }

        public DuplicateBlueprintException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DuplicateBlueprintException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}