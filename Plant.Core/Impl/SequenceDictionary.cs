using System;
using System.Collections.Generic;
using System.Reflection;

namespace Plant.Core.Impl
{
    internal class SequenceDictionary
    {
        readonly Dictionary<Type, Dictionary<PropertyInfo, int>> _sequenceValues = new Dictionary<Type, Dictionary<PropertyInfo, int>>();

        public int GetSequenceValue<T>(PropertyInfo propertyInfo)
        {
            int value;

            if (_sequenceValues.ContainsKey(typeof(T)) && _sequenceValues[typeof(T)].ContainsKey(propertyInfo))
            {
                value = _sequenceValues[typeof(T)][propertyInfo];
            }
            else
            {
                _sequenceValues.Add(typeof(T), new Dictionary<PropertyInfo, int>());
                _sequenceValues[typeof(T)].Add(propertyInfo, value = 0);
            }


            _sequenceValues[typeof(T)][propertyInfo]++;

            return value;
        }
    }
}