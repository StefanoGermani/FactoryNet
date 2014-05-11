using System.Collections.Generic;
using System.Linq.Expressions;
using FactoryNet.Core.Helpers;

namespace FactoryNet.Core.Dictionaries
{
    internal interface IProperties
    {
        void Add<T>(string variation, IEnumerable<MemberBinding> defaults);
        IDictionary<PropertyData, Expression> Get<T>(string variation);
        bool ContainsKey<T>(string variant);
    }

    internal class Properties : BaseList<IDictionary<PropertyData, Expression>>, IProperties
    {
        private readonly IPropertyHelper _propertyHelper;

        public Properties(IPropertyHelper propertyHelper)
        {
            _propertyHelper = propertyHelper;
        }

        public void Add<T>(string variation, IEnumerable<MemberBinding> defaults)
        {
            base.Add<T>(variation, _propertyHelper.ToPropertyList(defaults));
        }

       
    }
}