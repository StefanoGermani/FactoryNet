using System;
using System.Linq;
using System.Linq.Expressions;

namespace FactoryNet.Core.Helpers
{
    internal interface IConstructorHelper
    {
        T CreateInstance<T>(NewExpression newExpression);
    }

    internal class ConstructorHelper : IConstructorHelper
    {
        public T CreateInstance<T>(NewExpression newExpression)
        {
            return (T)newExpression.Constructor.Invoke(newExpression.Arguments.Select(a => ExecuteExpression(a)).ToArray());
        }

        private object ExecuteExpression(Expression expression)
        {
            LambdaExpression lambda = Expression.Lambda(expression);
            Delegate compiled = lambda.Compile();

            return compiled.DynamicInvoke(null);
        }
    }
}
