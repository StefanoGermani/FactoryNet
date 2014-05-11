using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FactoryNet.Core
{
    public interface IFactory
    {
        event ObjectCreatedEventHandler ObjectCreated;
        event ObjectDeletedEventHandler ObjectDeleted;

        IFactory LoadBlueprintsFromAssembly(Assembly assembly);
        IFactory LoadBlueprintsFromAssemblies();
        IFactory LoadBlueprintsFromCurrentAssembly();

        /// <summary>
        /// List of objects created by this factory
        /// </summary>
        IList<object> CreatedObjects { get; }

        T Build<T>(Action<T> userSpecifiedProperties);
        T Build<T>(string variation = "", Action<T> userSpecifiedProperties = null);
        T Create<T>(Action<T> userSpecifiedProperties);
        T Create<T>(string variation = "", Action<T> userSpecifiedProperties = null);
        T Create<T>(Expression<Func<T>> definition);

        void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation = null);
        void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation = null);

        void ClearCreatedObjects();
    }
}