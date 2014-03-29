using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        public void Add<T>(string variation, IEnumerable<MemberBinding> defaults)
        {
            base.Add<T>(variation, ToPropertyList(defaults));
        }

        private IDictionary<PropertyData, Expression> ToPropertyList(IEnumerable<MemberBinding> defaults)
        {
            if (defaults == null) return new Dictionary<PropertyData, Expression>();

            return defaults.ToDictionary(memberBinding => new PropertyData((PropertyInfo) memberBinding.Member),
                                         memberBinding => ((MemberAssignment) memberBinding).Expression);
        }
    }
}