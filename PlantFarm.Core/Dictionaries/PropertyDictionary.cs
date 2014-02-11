using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PlantFarm.Core.Helpers;
using PlantFarm.Core.Impl;

namespace PlantFarm.Core.Dictionaries
{
    internal class PropertyDictionary
    {
        private readonly BluePrintKeyHelper _bluePrintKeyHelper;

        private readonly Dictionary<string, IDictionary<PropertyData, Expression>> _properties =
            new Dictionary<string, IDictionary<PropertyData, Expression>>();

        public PropertyDictionary(BluePrintKeyHelper bluePrintKeyHelper)
        {
            _bluePrintKeyHelper = bluePrintKeyHelper;
        }

        public void Add<T>(string variation, IEnumerable<MemberBinding> defaults)
        {
            _properties.Add(_bluePrintKeyHelper.GetBluePrintKey<T>(variation), ToPropertyList(defaults));
        }

        public IDictionary<PropertyData, Expression> Get<T>(string variation)
        {
            return _properties[_bluePrintKeyHelper.GetBluePrintKey<T>(variation)];
        }

        private IDictionary<PropertyData, Expression> ToPropertyList(IEnumerable<MemberBinding> defaults)
        {
            if (defaults == null) return new Dictionary<PropertyData, Expression>();

            return defaults.ToDictionary(memberBinding => new PropertyData((PropertyInfo) memberBinding.Member),
                                         memberBinding => ((MemberAssignment) memberBinding).Expression);
        }

        public bool ContainsKey<T>(string variant)
        {
            return _properties.ContainsKey(_bluePrintKeyHelper.GetBluePrintKey<T>(variant));
        }
    }
}