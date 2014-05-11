using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FactoryNet.Core.Dictionaries;
using FactoryNet.Core.Exceptions;

namespace FactoryNet.Core.Helpers
{
    internal interface IPropertyHelper
    {
        IDictionary<PropertyData, Expression> ToPropertyList(IEnumerable<MemberBinding> defaults);

        void SetProperties<T>(IDictionary<PropertyData, Expression> properties, T instance) where T : class;
    }

    internal class PropertyHelper : IPropertyHelper
    {
        private readonly ISequences _sequenceValues;

        public PropertyHelper()
        {
            _sequenceValues = new Sequences();
        }

        public IDictionary<PropertyData, Expression> ToPropertyList(IEnumerable<MemberBinding> defaults)
        {
            if (defaults == null) return new Dictionary<PropertyData, Expression>();

            return defaults.ToDictionary(memberBinding => new PropertyData((PropertyInfo)memberBinding.Member),
                                         memberBinding => ((MemberAssignment)memberBinding).Expression);
        }

        public void SetProperties<T>(IDictionary<PropertyData, Expression> properties, T instance) where T : class
        {
            var type = instance.GetType();

            foreach (var property in properties.Keys)
            {
                PropertyInfo propertyInfo = type.GetProperty(property.Name);
                if (propertyInfo == null) throw new PropertyNotFoundException(property.Name, properties[property]);

                var expression = properties[property];

                MethodCallExpression callExpression;

                var unaryExpression = expression as UnaryExpression;

                if (unaryExpression != null)
                {
                    callExpression = unaryExpression.Operand as MethodCallExpression;
                }
                else
                {
                    callExpression = expression as MethodCallExpression;
                }

                if (callExpression != null && callExpression.Method.DeclaringType == typeof(Sequence))
                {
                    int sequenceNumber = _sequenceValues.GetSequenceValue<T>(propertyInfo);

                    Delegate compiled = ((LambdaExpression)callExpression.Arguments[0]).Compile();

                    var value = Convert.ChangeType(compiled.DynamicInvoke(sequenceNumber), propertyInfo.PropertyType);

                    propertyInfo.SetValue(instance, value, null);
                }
                else
                {
                    LambdaExpression lambda = Expression.Lambda(expression);
                    Delegate compiled = lambda.Compile();

                    propertyInfo.SetValue(instance, compiled.DynamicInvoke(null), null);
                }
            }
        }
    }
}
