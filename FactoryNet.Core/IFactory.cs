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

        T Build<T>(Action<T> userSpecifiedProperties) where T : class;
        T Build<T>(string variation = "", Action<T> userSpecifiedProperties = null) where T : class;
        T Create<T>(Action<T> userSpecifiedProperties) where T : class;
        T Create<T>(string variation = "", Action<T> userSpecifiedProperties = null) where T : class;
        T Create<T>(Expression<Func<T>> definition) where T : class;

        void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation = null) where T : class;
        void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation = null) where T : class;

        void ClearCreatedObjects();
    }
}