using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PlantFarm.Core.Helpers
{
    internal class ConstructorHelper
    {
        private object ExecuteExpression(Expression expression)
        {
            LambdaExpression lambda = Expression.Lambda(expression);
            Delegate compiled = lambda.Compile();

            return compiled.DynamicInvoke(null);
        }

        public T CreateInstance<T>(NewExpression newExpression)
        {
            return (T)newExpression.Constructor.Invoke(newExpression.Arguments.Select(a => ExecuteExpression(a)).ToArray());
        }
    }
}
