using System;

namespace Plant.Core.Exceptions
{
    public class PersisterException : Exception
    {
        private const string DEFAULT_FAILURE_MESSAGE = "Failed to save in database";

        public PersisterException(Exception ex)
            : base(DEFAULT_FAILURE_MESSAGE, ex)
        {
        }

        public PersisterException()
            : base(DEFAULT_FAILURE_MESSAGE)
        {
        }

        public PersisterException(string message)
            : base(message)
        {
        }
    }
}