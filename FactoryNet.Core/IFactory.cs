using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FactoryNet.Core
{
    public interface IFactory
    {
        event BluePrintCreatedEventHandler BluePrintCreated;
        event ObjectDeletedEventHandler ObjectDeleted;

        IFactory LoadBlueprintsFromAssembly(Assembly assembly);
        IFactory LoadBlueprintsFromAssemblies();
        IFactory LoadBlueprintsFromCurrentAssembly();


        IList<object> CreatedObjects { get; }

        T Build<T>();
        T Build<T>(string variation);
        T Build<T>(Action<T> userSpecifiedProperties);
        T Build<T>(string variation, Action<T> userSpecifiedProperties);
        T Create<T>();
        T Create<T>(string variation);
        T Create<T>(Action<T> userSpecifiedProperties);
        //T Create<T>(Expression<Func<T>> definition);
        T Create<T>(string variation, Action<T> userSpecifiedProperties);

        void Define<T>(Expression<Func<T>> definition);
        void Define<T>(string variation, Expression<Func<T>> definition);
        void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation);
        void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation);

        void ClearCreatedObjects();
    }
}