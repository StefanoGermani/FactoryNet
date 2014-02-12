using System;
using System.Reflection;

namespace FactoryNet.Core
{
    internal class PropertyData
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
            var propertyData = other as PropertyData;
            return propertyData != null && Equals(propertyData);
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