using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plant.Core.Utils
{
    class FastActivator
    {
        public delegate T ObjectActivator<out T>(params object[] args);

        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        public static T CreateInstance<T>()
        {
            var ctor = typeof(T).GetConstructors().First(info => !info.GetParameters().Any());
            ObjectActivator<T> createdActivator = GetActivator<T>(ctor);

            return createdActivator();
        }
    }
}
