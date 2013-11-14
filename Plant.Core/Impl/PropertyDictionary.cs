using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plant.Core.Impl
{
    internal class PropertyDictionary 
    {
        readonly Dictionary<string, IDictionary<PropertyData, Expression>> _properties = new Dictionary<string, IDictionary<PropertyData, Expression>>();

        public void Add<T>(string variation, IEnumerable<MemberBinding> defaults)
        {
            _properties.Add(BlueprintKeyGenerator.BluePrintKey<T>(variation), ToPropertyList(defaults));
        }

        public IDictionary<PropertyData, Expression> Get<T>()
        {
            return Get<T>(string.Empty);
        }

        public IDictionary<PropertyData, Expression> Get<T>(string variation)
        {
            return _properties[BlueprintKeyGenerator.BluePrintKey<T>(variation)];
        }

        private IDictionary<PropertyData, Expression> ToPropertyList(IEnumerable<MemberBinding> defaults)
        {
            if (defaults == null) return new Dictionary<PropertyData, Expression>();

            return defaults.ToDictionary(memberBinding => new PropertyData((PropertyInfo)memberBinding.Member),
                                         memberBinding => ((MemberAssignment)memberBinding).Expression);
        }

        public void Add<T>(ReadOnlyCollection<MemberBinding> defaults)
        {
            _properties.Add(BlueprintKeyGenerator.BluePrintKey<T>(), ToPropertyList(defaults));
        }

        public bool ContainsKey<T>()
        {
            return _properties.ContainsKey(BlueprintKeyGenerator.BluePrintKey<T>());
        }


    }
}