using System;
using System.Runtime.Serialization;

namespace Plant.Core.Exceptions
{
    public class DuplicateBlueprintException : Exception
    {
        public DuplicateBlueprintException(Type type, string variation)
            : base(string.Format("{0}{1} is already registered. You can only register one factory per type/variant.", type, string.IsNullOrEmpty(variation) ? string.Empty : " - " + variation))
        {
        }
    }
}