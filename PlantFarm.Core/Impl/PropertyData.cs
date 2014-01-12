using System;
using System.Reflection;

namespace PlantFarm.Core.Impl
{
    public class PropertyData
    {
        private readonly Type _type;

        public PropertyData(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            _type = propertyInfo.PropertyType;
        }

        public string Name { get; private set; }

        public override bool Equals(object other)
        {
            if (other is PropertyData)
                return Equals((PropertyData) other);
            return false;
        }

        public bool Equals(PropertyData other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name) && _type == other._type;
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode() + Name.GetHashCode();
        }
    }
}